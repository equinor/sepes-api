using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using Sepes.Infrastructure.Util.Auth;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class SandboxResourceCreateService : SandboxServiceBase, ISandboxResourceCreateService
    {
        readonly ICloudResourceCreateService _cloudResourceCreateService;
        readonly ICloudResourceOperationCreateService _cloudResourceOperationCreateService;       
        readonly IProvisioningQueueService _provisioningQueueService;

        public SandboxResourceCreateService(IConfiguration config,
            SepesDbContext db,
            IMapper mapper,
            ILogger<SandboxResourceDeleteService> logger,
            IUserService userService,
            ISandboxModelService sandboxModelService,
            ICloudResourceCreateService cloudResourceCreateService,
            ICloudResourceOperationCreateService cloudResourceOperationCreateService,
            IProvisioningQueueService provisioningQueueService)
              : base(config, db, mapper, logger, userService, sandboxModelService)
        {

            _cloudResourceCreateService = cloudResourceCreateService;
            _cloudResourceOperationCreateService = cloudResourceOperationCreateService;
            _provisioningQueueService = provisioningQueueService;
        }

        public async Task CreateBasicSandboxResourcesAsync(Sandbox sandbox)
        {
            _logger.LogInformation($"Creating basic sandbox resources for sandbox: {sandbox.Name}. First creating Resource Group, other resources are created by worker");

            try
            {
                var tags = AzureResourceTagsFactory.SandboxResourceTags(_configuration, sandbox.Study, sandbox);

                var creationAndSchedulingDto =
                    new SandboxResourceCreationAndSchedulingDto()
                    {
                        StudyId = sandbox.Study.Id,
                        SandboxId = sandbox.Id,
                        StudyName = sandbox.Study.Name,
                        SandboxName = sandbox.Name,
                        Region = sandbox.Region,
                        Tags = tags,
                        BatchId = Guid.NewGuid().ToString()
                    };

                var queueParentItem = new ProvisioningQueueParentDto
                {                  
                    Description = $"Create basic resources for Sandbox: {creationAndSchedulingDto.SandboxId}"
                };

                await ScheduleCreationOfSandboxResourceGroup(creationAndSchedulingDto, queueParentItem);
                await ScheduleCreationOfSandboxResourceGroupRoleAssignments(creationAndSchedulingDto, queueParentItem);
                await ScheduleCreationOfDiagStorageAccount(creationAndSchedulingDto, queueParentItem);
                await ScheduleCreationOfNetworkSecurityGroup(creationAndSchedulingDto, queueParentItem);
                await ScheduleCreationOfVirtualNetwork(creationAndSchedulingDto, queueParentItem);
                await ScheduleCreationOfBastion(creationAndSchedulingDto, queueParentItem);

                await _provisioningQueueService.SendMessageAsync(queueParentItem);

                _logger.LogInformation($"Done ordering creation of basic resources for sandbox: {creationAndSchedulingDto.SandboxName}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to create basic sandbox resources.", ex);
            }        
        }

        async Task ScheduleCreationOfSandboxResourceGroup(SandboxResourceCreationAndSchedulingDto dto, ProvisioningQueueParentDto queueParentItem)
        {
            dto.ResourceGroupName = AzureResourceNameUtil.SandboxResourceGroup(dto.StudyName, dto.SandboxName);
            dto.ResourceGroup = await CreateResourceGroupEntryAndAddToQueue(dto, queueParentItem, dto.ResourceGroupName);
        }

        async Task ScheduleCreationOfSandboxResourceGroupRoleAssignments(SandboxResourceCreationAndSchedulingDto dto, ProvisioningQueueParentDto queueParentItem)
        {
            var participants = await _db.StudyParticipants.Include(sp=> sp.User).Where(p => p.StudyId == dto.StudyId).ToListAsync();
            var desiredRoles = ParticipantRoleToAzureRoleTranslator.CreateDesiredRolesForSandboxResourceGroup(participants);
            var desiredRolesSerialized = CloudResourceConfigStringSerializer.Serialize(desiredRoles);
            var resourceGroupCreateOperation = dto.ResourceGroup.Operations.FirstOrDefault().Id;
            var updateOpId = await _cloudResourceOperationCreateService.CreateUpdateOperationAsync(dto.ResourceGroup.Id, CloudResourceOperationType.ENSURE_ROLES, dependsOn: resourceGroupCreateOperation, desiredState: desiredRolesSerialized);
            queueParentItem.Children.Add(new ProvisioningQueueChildDto() { ResourceOperationId = updateOpId.Id });
        }

        async Task ScheduleCreationOfDiagStorageAccount(SandboxResourceCreationAndSchedulingDto dto, ProvisioningQueueParentDto queueParentItem)
        {
            var resourceName = AzureResourceNameUtil.DiagnosticsStorageAccount(dto.StudyName, dto.SandboxName);
            var resourceGroupCreateOperation = dto.ResourceGroup.Operations.FirstOrDefault().Id;
            var resourceEntry = await CreateResourceEntryAndAddToQueue(dto, queueParentItem, AzureResourceType.StorageAccount, resourceName: resourceName, dependsOn: resourceGroupCreateOperation);
            dto.DiagnosticsStorage = resourceEntry;
        }

        async Task ScheduleCreationOfNetworkSecurityGroup(SandboxResourceCreationAndSchedulingDto dto, ProvisioningQueueParentDto queueParentItem)
        {
            var nsgName = AzureResourceNameUtil.NetworkSecGroupSubnet(dto.StudyName, dto.SandboxName);
            var diagStorageAccountCreateOperation = dto.DiagnosticsStorage.Operations.FirstOrDefault().Id;
            var resourceEntry = await CreateResourceEntryAndAddToQueue(dto, queueParentItem, AzureResourceType.NetworkSecurityGroup, resourceName: nsgName, dependsOn: diagStorageAccountCreateOperation);
            dto.NetworkSecurityGroup = resourceEntry;
        }

        async Task ScheduleCreationOfVirtualNetwork(SandboxResourceCreationAndSchedulingDto dto, ProvisioningQueueParentDto queueParentItem)
        {
            var networkName = AzureResourceNameUtil.VNet(dto.StudyName, dto.SandboxName);
            var sandboxSubnetName = AzureResourceNameUtil.SubNet(dto.StudyName, dto.SandboxName);

            var networkSettings = new NetworkSettingsDto() { SandboxSubnetName = sandboxSubnetName };
            var networkSettingsString = CloudResourceConfigStringSerializer.Serialize(networkSettings);

            var nsgCreateOperation = dto.NetworkSecurityGroup.Operations.FirstOrDefault().Id;

            var resourceEntry = await CreateResourceEntryAndAddToQueue(dto, queueParentItem, AzureResourceType.VirtualNetwork, resourceName: networkName, configString: networkSettingsString, dependsOn: nsgCreateOperation);
            dto.Network = resourceEntry;
        }

        async Task ScheduleCreationOfBastion(SandboxResourceCreationAndSchedulingDto dto, ProvisioningQueueParentDto queueParentItem, string configString = null)
        {
            var vNetCreateOperation = dto.Network.Operations.FirstOrDefault().Id;

            var bastionName = AzureResourceNameUtil.Bastion(dto.StudyName, dto.SandboxName);

            _ = await CreateResourceEntryAndAddToQueue(dto, queueParentItem, AzureResourceType.Bastion, resourceName: bastionName, configString: configString, dependsOn: vNetCreateOperation);
          
        }

        async Task<CloudResource> CreateResourceGroupEntryAndAddToQueue(SandboxResourceCreationAndSchedulingDto dto, ProvisioningQueueParentDto queueParentItem, string resourceGroupName)
        {
            var resourceEntry = await _cloudResourceCreateService.CreateSandboxResourceGroupEntryAsync(dto, resourceGroupName);
            queueParentItem.Children.Add(new ProvisioningQueueChildDto() { ResourceOperationId = resourceEntry.Operations.FirstOrDefault().Id });
            return resourceEntry;
        }

        async Task<CloudResource> CreateResourceEntryAndAddToQueue(SandboxResourceCreationAndSchedulingDto dto, ProvisioningQueueParentDto queueParentItem, string resourceType, string resourceName = AzureResourceNameUtil.AZURE_RESOURCE_INITIAL_ID_OR_NAME, string configString = null, int dependsOn = 0)
        {
            var resourceEntry = await _cloudResourceCreateService.CreateSandboxResourceEntryAsync(dto, resourceType, resourceName: resourceName, configString: configString, dependsOn: dependsOn);
            queueParentItem.Children.Add(new ProvisioningQueueChildDto() { ResourceOperationId = resourceEntry.Operations.FirstOrDefault().Id });
            return resourceEntry;
        }
    }
}
