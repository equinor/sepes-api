using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class SandboxResourceCreateService : SandboxServiceBase, ISandboxResourceCreateService
    {
        readonly ICloudResourceCreateService _cloudResourceCreateService;
        readonly IProvisioningQueueService _provisioningQueueService;

        public SandboxResourceCreateService(IConfiguration config,
            SepesDbContext db,
            IMapper mapper,
            ILogger<SandboxResourceDeleteService> logger,
            IUserService userService,
            ICloudResourceCreateService cloudResourceCreateService,
            IProvisioningQueueService provisioningQueueService)
              : base(config, db, mapper, logger, userService)
        {


            _cloudResourceCreateService = cloudResourceCreateService;
            _provisioningQueueService = provisioningQueueService;
        }

        public async Task<SandboxResourceCreationAndSchedulingDto> CreateBasicSandboxResourcesAsync(SandboxResourceCreationAndSchedulingDto dto)
        {
            _logger.LogInformation($"Creating basic sandbox resources for sandbox: {dto.SandboxName}. First creating Resource Group, other resources are created by worker");

            try
            {
                var queueParentItem = new ProvisioningQueueParentDto
                {
                    SandboxId = dto.SandboxId,
                    Description = $"Create basic resources for Sandbox: {dto.SandboxId}"
                };

                await ScheduleCreationOfSandboxResourceGroup(dto, queueParentItem);
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

        async Task ScheduleCreationOfSandboxResourceGroup(SandboxResourceCreationAndSchedulingDto dto, ProvisioningQueueParentDto queueParentItem)
        {
            dto.ResourceGroupName = AzureResourceNameUtil.SandboxResourceGroup(dto.StudyName, dto.SandboxName);
            var resourceEntry = await CreateResourceGroupEntryAndAddToQueue(dto, queueParentItem, dto.ResourceGroupName);
            dto.ResourceGroup = resourceEntry;
        }

        async Task ScheduleCreationOfDiagStorageAccount(SandboxResourceCreationAndSchedulingDto dto, ProvisioningQueueParentDto queueParentItem)
        {
            var resourceName = AzureResourceNameUtil.DiagnosticsStorageAccount(dto.StudyName, dto.SandboxName);
            var resourceGroupCreateOperation = dto.ResourceGroup.Operations.FirstOrDefault().Id;
            var resourceEntry = await CreateResourceEntryAndAddToQueue(dto, queueParentItem, AzureResourceType.StorageAccount, sandboxControlled: true, resourceName: resourceName, dependsOn: resourceGroupCreateOperation);
            dto.DiagnosticsStorage = resourceEntry;
        }

        async Task ScheduleCreationOfNetworkSecurityGroup(SandboxResourceCreationAndSchedulingDto dto, ProvisioningQueueParentDto queueParentItem)
        {
            var nsgName = AzureResourceNameUtil.NetworkSecGroupSubnet(dto.StudyName, dto.SandboxName);
            var diagStorageAccountCreateOperation = dto.DiagnosticsStorage.Operations.FirstOrDefault().Id;
            var resourceEntry = await CreateResourceEntryAndAddToQueue(dto, queueParentItem, AzureResourceType.NetworkSecurityGroup, sandboxControlled: true, resourceName: nsgName, dependsOn: diagStorageAccountCreateOperation);
            dto.NetworkSecurityGroup = resourceEntry;
        }

        async Task ScheduleCreationOfVirtualNetwork(SandboxResourceCreationAndSchedulingDto dto, ProvisioningQueueParentDto queueParentItem)
        {
            var networkName = AzureResourceNameUtil.VNet(dto.StudyName, dto.SandboxName);
            var sandboxSubnetName = AzureResourceNameUtil.SubNet(dto.StudyName, dto.SandboxName);

            var networkSettings = new NetworkSettingsDto() { SandboxSubnetName = sandboxSubnetName };
            var networkSettingsString = CloudResourceConfigStringSerializer.Serialize(networkSettings);

            var nsgCreateOperation = dto.NetworkSecurityGroup.Operations.FirstOrDefault().Id;

            var resourceEntry = await CreateResourceEntryAndAddToQueue(dto, queueParentItem, AzureResourceType.VirtualNetwork, sandboxControlled: true, resourceName: networkName, configString: networkSettingsString, dependsOn: nsgCreateOperation);
            dto.Network = resourceEntry;
        }

        async Task ScheduleCreationOfBastion(SandboxResourceCreationAndSchedulingDto dto, ProvisioningQueueParentDto queueParentItem, string configString = null)
        {
            var vNetCreateOperation = dto.Network.Operations.FirstOrDefault().Id;

            var bastionName = AzureResourceNameUtil.Bastion(dto.StudyName, dto.SandboxName);

            var resourceEntry = await CreateResourceEntryAndAddToQueue(dto, queueParentItem, AzureResourceType.Bastion, sandboxControlled: true, resourceName: bastionName, configString: configString, dependsOn: vNetCreateOperation);
            dto.Bastion = resourceEntry;
        }

        async Task<CloudResourceDto> CreateResourceGroupEntryAndAddToQueue(SandboxResourceCreationAndSchedulingDto dto, ProvisioningQueueParentDto queueParentItem, string resourceGroupName)
        {
            var resourceEntry = await _cloudResourceCreateService.Create(dto, AzureResourceType.ResourceGroup, sandboxControlled: true, resourceName: resourceGroupName);
            queueParentItem.Children.Add(new ProvisioningQueueChildDto() { ResourceOperationId = resourceEntry.Operations.FirstOrDefault().Id });
            return resourceEntry;
        }

        async Task<CloudResourceDto> CreateResourceEntryAndAddToQueue(SandboxResourceCreationAndSchedulingDto dto, ProvisioningQueueParentDto queueParentItem, string resourceType, bool sandboxControlled = true, string resourceName = AzureResourceNameUtil.AZURE_RESOURCE_INITIAL_ID_OR_NAME, string configString = null, int dependsOn = 0)
        {
            var resourceEntry = await _cloudResourceCreateService.Create(dto, resourceType, sandboxControlled: sandboxControlled, resourceName: resourceName, configString: configString, dependsOn: dependsOn);
            queueParentItem.Children.Add(new ProvisioningQueueChildDto() { ResourceOperationId = resourceEntry.Operations.FirstOrDefault().Id });
            return resourceEntry;
        }
    }
}
