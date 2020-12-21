using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Azure;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Interface;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class SandboxResourceCreateService : SandboxServiceBase, ISandboxResourceCreateService
    {
        readonly IRequestIdService _requestIdService;
        readonly IAzureResourceGroupService _azureResourceGroupService;
        readonly ICloudResourceCreateService _cloudResourceCreateService;
        readonly ICloudResourceUpdateService _cloudResourceUpdateService;        
        readonly ICloudResourceOperationUpdateService _cloudResourceOperationUpdateService;
        readonly IProvisioningQueueService _provisioningQueueService;

        public SandboxResourceCreateService(IConfiguration config, SepesDbContext db, IMapper mapper, ILogger<SandboxResourceDeleteService> logger, IUserService userService,
                IRequestIdService requestIdService,
                ICloudResourceCreateService cloudResourceCreateService,
                ICloudResourceUpdateService cloudResourceUpdateService,
           ICloudResourceOperationUpdateService cloudResourceOperationUpdateService,
           IProvisioningQueueService provisioningQueueService,
             IAzureResourceGroupService resourceGroupService)
              : base(config, db, mapper, logger, userService)
        {

            _requestIdService = requestIdService;
            _azureResourceGroupService = resourceGroupService;
            _cloudResourceCreateService = cloudResourceCreateService;
            _cloudResourceUpdateService = cloudResourceUpdateService;            
            _cloudResourceOperationUpdateService = cloudResourceOperationUpdateService ?? throw new ArgumentNullException(nameof(cloudResourceOperationUpdateService));
            _provisioningQueueService = provisioningQueueService;
        }

        public async Task<SandboxResourceCreationAndSchedulingDto> CreateBasicSandboxResourcesAsync(SandboxResourceCreationAndSchedulingDto dto)
        {
            _logger.LogInformation($"Creating basic sandbox resources for sandbox: {dto.SandboxName}. First creating Resource Group, other resources are created by worker");

            try
            {
                await _cloudResourceCreateService.CreateSandboxResourceGroup(dto);
                await CreateSandboxResourceGroup(dto);

                _logger.LogInformation($"Done creating Resource Group for sandbox: {dto.SandboxName}. Scheduling creation of other resources");

                var queueParentItem = new ProvisioningQueueParentDto
                {
                    SandboxId = dto.SandboxId,
                    Description = $"Create basic resources for Sandbox: {dto.SandboxId}"
                };

                await ScheduleCreationOfDiagStorageAccount(dto, queueParentItem);
                await ScheduleCreationOfNetworkSecurityGroup(dto, queueParentItem);
                await ScheduleCreationOfVirtualNetwork(dto, queueParentItem);
                await ScheduleCreationOfBastion(dto, queueParentItem);

                await _provisioningQueueService.SendMessageAsync(queueParentItem);

                _logger.LogInformation($"Done ordering creation of basic resources for sandbox: {dto.SandboxName}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to create basic sandbox resources.", ex);
            }

            return dto;
        }

        public async Task CreateSandboxResourceGroup(SandboxResourceCreationAndSchedulingDto dto)
        {
            _logger.LogInformation($"Creating Resource Group for sandbox: {dto.SandboxId}");

            var resourceCreateOperation = dto.ResourceGroup.Operations.FirstOrDefault();
            await _cloudResourceOperationUpdateService.SetInProgressAsync(resourceCreateOperation.Id, _requestIdService.GetRequestId(), CloudResourceOperationState.IN_PROGRESS);

            var azureResourceGroup = await _azureResourceGroupService.Create(dto.ResourceGroup.ResourceName, dto.Region, dto.Tags);
            ApplyPropertiesFromResourceGroup(azureResourceGroup, dto.ResourceGroup);

            _ = await _cloudResourceUpdateService.UpdateResourceGroup(dto.ResourceGroup.Id, dto.ResourceGroup);
            _ = await _cloudResourceOperationUpdateService.UpdateStatusAsync(dto.ResourceGroup.Operations.FirstOrDefault().Id, CloudResourceOperationState.DONE_SUCCESSFUL);
        }

        async Task ScheduleCreationOfDiagStorageAccount(SandboxResourceCreationAndSchedulingDto dto, ProvisioningQueueParentDto queueParentItem)
        {
            var resourceName = AzureResourceNameUtil.DiagnosticsStorageAccount(dto.StudyName, dto.SandboxName);
            var resourceGroupCreateOperation = dto.ResourceGroup.Operations.FirstOrDefault().Id;
            var resourceEntry = await CreateResource(dto, queueParentItem, AzureResourceType.StorageAccount, sandboxControlled: true, resourceName: resourceName, dependsOn: resourceGroupCreateOperation);
            dto.DiagnosticsStorage = resourceEntry;
        }

        async Task ScheduleCreationOfNetworkSecurityGroup(SandboxResourceCreationAndSchedulingDto dto, ProvisioningQueueParentDto queueParentItem)
        {
            var nsgName = AzureResourceNameUtil.NetworkSecGroupSubnet(dto.StudyName, dto.SandboxName);
            var diagStorageAccountCreateOperation = dto.DiagnosticsStorage.Operations.FirstOrDefault().Id;
            var resourceEntry = await CreateResource(dto, queueParentItem, AzureResourceType.NetworkSecurityGroup, sandboxControlled: true, resourceName: nsgName, dependsOn: diagStorageAccountCreateOperation);
            dto.NetworkSecurityGroup = resourceEntry;
        }

        async Task ScheduleCreationOfVirtualNetwork(SandboxResourceCreationAndSchedulingDto dto, ProvisioningQueueParentDto queueParentItem)
        {            
            var networkName = AzureResourceNameUtil.VNet(dto.StudyName, dto.SandboxName);
            var sandboxSubnetName = AzureResourceNameUtil.SubNet(dto.StudyName, dto.SandboxName);

            var networkSettings = new NetworkSettingsDto() { SandboxSubnetName = sandboxSubnetName };
            var networkSettingsString = CloudResourceConfigStringSerializer.Serialize(networkSettings);

            var nsgCreateOperation = dto.NetworkSecurityGroup.Operations.FirstOrDefault().Id;

            var resourceEntry = await CreateResource(dto, queueParentItem, AzureResourceType.VirtualNetwork, sandboxControlled: true, resourceName: networkName, configString: networkSettingsString, dependsOn: nsgCreateOperation);
            dto.Network = resourceEntry;
        }

        async Task ScheduleCreationOfBastion(SandboxResourceCreationAndSchedulingDto dto, ProvisioningQueueParentDto queueParentItem, string configString = null)
        {
            var vNetCreateOperation = dto.Network.Operations.FirstOrDefault().Id;

            var bastionName = AzureResourceNameUtil.Bastion(dto.StudyName, dto.SandboxName);

            var resourceEntry = await CreateResource(dto, queueParentItem, AzureResourceType.Bastion, sandboxControlled: true, resourceName: bastionName, configString: configString, dependsOn: vNetCreateOperation);
            dto.Bastion = resourceEntry;
        }

        async Task<CloudResourceDto> CreateResource(SandboxResourceCreationAndSchedulingDto dto, ProvisioningQueueParentDto queueParentItem, string resourceType, bool sandboxControlled = true, string resourceName = AzureResourceNameUtil.AZURE_RESOURCE_INITIAL_ID_OR_NAME, string configString = null, int dependsOn = 0)
        {
            var resourceEntry = await _cloudResourceCreateService.Create(dto, resourceType, sandboxControlled: sandboxControlled, resourceName: resourceName, configString: configString, dependsOn: dependsOn);
            queueParentItem.Children.Add(new ProvisioningQueueChildDto() { ResourceOperationId = resourceEntry.Operations.FirstOrDefault().Id });

            return resourceEntry;
        }

        public void ApplyPropertiesFromResourceGroup(AzureResourceGroupDto source, CloudResourceDto target)
        {
            target.ResourceId = source.Id;
            target.ResourceName = source.Name;
            target.ResourceGroupId = source.Id;
            target.ResourceGroupName = source.Name;
            target.ProvisioningState = source.ProvisioningState;
            target.ResourceKey = source.Key;
        } 
    }
}
