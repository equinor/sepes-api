using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Azure;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Interface;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class SandboxCloudResourceService : SandboxServiceBase, ISandboxCloudResourceService
    {
        readonly IRequestIdService _requestIdService;
        readonly IAzureResourceGroupService _resourceGroupService;
        readonly ISandboxResourceService _sandboxResourceService;
        readonly ISandboxResourceOperationService _sandboxResourceOperationService;
        readonly IProvisioningQueueService _provisioningQueueService;

        public SandboxCloudResourceService(IConfiguration config, SepesDbContext db, IMapper mapper, ILogger<SandboxCloudResourceService> logger, IUserService userService,
                IRequestIdService requestIdService,
                ISandboxResourceService sandboxResourceService,
           ISandboxResourceOperationService sandboxResourceOperationService,
           IProvisioningQueueService provisioningQueueService,
             IAzureResourceGroupService resourceGroupService)
              : base(config, db, mapper, logger, userService)
        {

            _requestIdService = requestIdService;
            _resourceGroupService = resourceGroupService;
            _sandboxResourceService = sandboxResourceService;
            _sandboxResourceOperationService = sandboxResourceOperationService ?? throw new ArgumentNullException(nameof(sandboxResourceOperationService));
            _provisioningQueueService = provisioningQueueService;
        }

        public async Task<SandboxResourceCreationAndSchedulingDto> CreateBasicSandboxResourcesAsync(SandboxResourceCreationAndSchedulingDto dto)
        {
            _logger.LogInformation($"Creating basic sandbox resources for sandbox: {dto.SandboxName}. First creating Resource Group, other resources are created by worker");

            try
            {
                await _sandboxResourceService.CreateSandboxResourceGroup(dto);
                await ScheduleCreateOfSandboxResourceGroup(dto);

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

        public async Task ScheduleCreateOfSandboxResourceGroup(SandboxResourceCreationAndSchedulingDto dto)
        {
            _logger.LogInformation($"Scheduling Resource Group creation for sandbox: {dto.SandboxId}");

            var resourceCreateOperation = dto.ResourceGroup.Operations.FirstOrDefault();
            await _sandboxResourceOperationService.SetInProgressAsync(resourceCreateOperation.Id, _requestIdService.GetRequestId(), CloudResourceOperationState.IN_PROGRESS);

            var azureResourceGroup = await _resourceGroupService.Create(dto.ResourceGroup.ResourceName, dto.Region, dto.Tags);
            ApplyPropertiesFromResourceGroup(azureResourceGroup, dto.ResourceGroup);

            _ = await _sandboxResourceService.UpdateResourceGroup(dto.ResourceGroup.Id, dto.ResourceGroup);
            _ = await _sandboxResourceOperationService.UpdateStatusAsync(dto.ResourceGroup.Operations.FirstOrDefault().Id, CloudResourceOperationState.DONE_SUCCESSFUL);
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
            //TODO: Add special network rules to resource
            var networkName = AzureResourceNameUtil.VNet(dto.StudyName, dto.SandboxName);
            var sandboxSubnetName = AzureResourceNameUtil.SubNet(dto.StudyName, dto.SandboxName);

            var networkSettings = new NetworkSettingsDto() { SandboxSubnetName = sandboxSubnetName };
            var networkSettingsString = SandboxResourceConfigStringSerializer.Serialize(networkSettings);

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

        async Task<SandboxResourceDto> CreateResource(SandboxResourceCreationAndSchedulingDto dto, ProvisioningQueueParentDto queueParentItem, string resourceType, bool sandboxControlled = true, string resourceName = AzureResourceNameUtil.AZURE_RESOURCE_INITIAL_ID_OR_NAME, string configString = null, int dependsOn = 0)
        {
            var resourceEntry = await _sandboxResourceService.Create(dto, resourceType, sandboxControlled: sandboxControlled, resourceName: resourceName, configString: configString, dependsOn: dependsOn);
            queueParentItem.Children.Add(new ProvisioningQueueChildDto() { SandboxResourceOperationId = resourceEntry.Operations.FirstOrDefault().Id });

            return resourceEntry;
        }

        public void ApplyPropertiesFromResourceGroup(AzureResourceGroupDto source, SandboxResourceDto target)
        {
            target.ResourceId = source.Id;
            target.ResourceName = source.Name;
            target.ResourceGroupId = source.Id;
            target.ResourceGroupName = source.Name;
            target.ProvisioningState = source.ProvisioningState;
            target.ResourceKey = source.Key;
        }

        public async Task ReScheduleSandboxResourceCreation(int sandboxId)
        {
            var sandboxFromDb = await GetOrThrowAsync(sandboxId, UserOperation.Study_Crud_Sandbox, true);

            var queueParentItem = new ProvisioningQueueParentDto();
            queueParentItem.SandboxId = sandboxFromDb.Id;
            queueParentItem.Description = $"Create basic resources for Sandbox (re-scheduled): {sandboxFromDb.Id}";

            //Check state of sandbox resource creation: Resource group shold be success, rest should be not started or failed

            var resourceGroupResource = sandboxFromDb.Resources.SingleOrDefault(r => r.ResourceType == AzureResourceType.ResourceGroup);

            if (resourceGroupResource == null)
            {
                throw new NullReferenceException(ReScheduleLogPrefix(sandboxFromDb.StudyId, sandboxId, "Could not locate database entry for ResourceGroup"));
            }

            var resourceGroupResourceOperation = resourceGroupResource.Operations.OrderByDescending(o => o.Created).FirstOrDefault();

            if (resourceGroupResourceOperation == null)
            {
                throw new NullReferenceException(ReScheduleLogPrefix(sandboxFromDb.StudyId, sandboxId, "Could not locate ANY database entry for ResourceGroupOperation"));
            }
            else if (resourceGroupResourceOperation.OperationType != CloudResourceOperationType.CREATE && resourceGroupResourceOperation.Status == CloudResourceOperationState.DONE_SUCCESSFUL)
            {
                throw new Exception(ReScheduleLogPrefix(sandboxFromDb.StudyId, sandboxId, "Could not locate RELEVANT database entry for ResourceGroupOperation"));
            }

            var operations = new List<SandboxResourceOperation>();

            foreach (var curResource in sandboxFromDb.Resources)
            {
                if (curResource.Id == resourceGroupResource.Id)
                {
                    //allready covered this above
                    continue;
                }

                var relevantOperation = curResource.Operations.OrderByDescending(o => o.Created).FirstOrDefault();

                if (relevantOperation == null)
                {
                    throw new NullReferenceException(ReScheduleLogPrefix(sandboxFromDb.StudyId, sandboxId, "Could not locate ANY database entry for resource", curResource.Id));
                }
                else if (String.IsNullOrWhiteSpace(relevantOperation.Status) || relevantOperation.Status == CloudResourceOperationState.NEW || relevantOperation.Status == CloudResourceOperationState.IN_PROGRESS || relevantOperation.Status == CloudResourceOperationState.FAILED || relevantOperation.Status == CloudResourceOperationState.DONE_SUCCESSFUL)
                {
                    _logger.LogInformation(ReScheduleLogPrefix(sandboxFromDb.StudyId, sandboxId, $"Re-queing item. Previous status was {relevantOperation.Status}", curResource.Id));
                    relevantOperation.MaxTryCount += CloudResourceConstants.RESOURCE_MAX_TRY_COUNT; //Increase max try count               
                    queueParentItem.Children.Add(new ProvisioningQueueChildDto() { SandboxResourceOperationId = relevantOperation.Id });
                }
                else
                {
                    throw new Exception(ReScheduleLogPrefix(sandboxFromDb.StudyId, sandboxId, $"Could not locate RELEVANT database entry for resource", curResource.Id));
                }
            }

            await _db.SaveChangesAsync();

            if (queueParentItem.Children.Count == 0)
            {
                throw new Exception(ReScheduleLogPrefix(sandboxFromDb.StudyId, sandboxId, $"Could not re-shedule creation. No relevant resource items found"));
            }
            else
            {
                await _provisioningQueueService.SendMessageAsync(queueParentItem);
            }
        }

        string ReScheduleLogPrefix(int studyId, int sandboxId, string logText, int resourceId = 0)
        {
            var logMessage = $"ReScheduleSandboxCreation | Study {studyId} | Sandbox {sandboxId}";

            if (resourceId > 0)
            {
                logMessage += $" | Resource: {resourceId}";
            }

            logMessage += $" | {logText}";

            return logMessage;
        }


        public Task<SandboxResourceLightDto> RetryLastOperation(int resourceId)
        {
            throw new NotImplementedException();
        }

        public async Task HandleSandboxDeleteAsync(int sandboxId)
        {
            var user = await _userService.GetCurrentUserAsync();
            var sandboxFromDb = await GetOrThrowAsync(sandboxId, UserOperation.Study_Crud_Sandbox, true);

            SandboxResource sandboxResourceGroup = null;

            if (sandboxFromDb.Resources.Count > 0)
            {
                //Mark all resources as deleted
                foreach (var curResource in sandboxFromDb.Resources)
                {
                    if (curResource.ResourceType == AzureResourceType.ResourceGroup)
                    {
                        sandboxResourceGroup = curResource;
                    }

                    curResource.Deleted = DateTime.UtcNow;
                    curResource.DeletedBy = user.UserName;

                    SetAllOperationsToAborted(user, curResource);

                    _logger.LogInformation(SepesEventId.SandboxDelete, "Study {0}, Sandbox {1}: Marking resource {2} for deletion", sandboxFromDb.StudyId, sandboxId, curResource.Id);
                }

                if (sandboxResourceGroup == null)
                {
                    throw new Exception($"Unable to find ResourceGroup record in DB for Sandbox {sandboxId}, StudyId: {sandboxFromDb.StudyId}.");
                }

                _logger.LogInformation(SepesEventId.SandboxDelete, $"Creating delete operation for resource group {sandboxResourceGroup.ResourceGroupName}");

                var deleteOperation = new SandboxResourceOperation()
                {
                    CreatedBy = user.UserName,
                    BatchId = Guid.NewGuid().ToString(),
                    CreatedBySessionId = _requestIdService.GetRequestId(),
                    OperationType = CloudResourceOperationType.DELETE,
                    SandboxResourceId = sandboxResourceGroup.Id,
                    Description = AzureResourceUtil.CreateDescriptionForResourceOperation(sandboxResourceGroup.ResourceType, CloudResourceOperationType.DELETE, sandboxResourceGroup.SandboxId) + ". (Delete of SandBox resource group and all resources within)",
                    MaxTryCount = CloudResourceConstants.RESOURCE_MAX_TRY_COUNT
                };

                sandboxResourceGroup.Operations.Add(deleteOperation);

                await _db.SaveChangesAsync();

                _logger.LogInformation(SepesEventId.SandboxDelete, "Study {0}, Sandbox {1}: Queuing operation", sandboxFromDb.StudyId, sandboxId);

                //Create queue item
                var queueParentItem = new ProvisioningQueueParentDto();
                queueParentItem.SandboxId = sandboxId;
                queueParentItem.Description = $"Delete resources for Sandbox: {sandboxId}";
                queueParentItem.Children.Add(new ProvisioningQueueChildDto() { SandboxResourceOperationId = deleteOperation.Id });
                await _provisioningQueueService.SendMessageAsync(queueParentItem, visibilityTimeout: TimeSpan.FromSeconds(10));
            }
            else
            {
                _logger.LogCritical(SepesEventId.SandboxDelete, "Study {0}, Sandbox {1}: Unable to find any resources for Sandbox", sandboxFromDb.StudyId, sandboxId);
                await _db.SaveChangesAsync();
            }
        }


        void SetAllOperationsToAborted(UserDto currentUser, SandboxResource resource)
        {
            foreach (var curOp in resource.Operations)
            {
                curOp.Status = CloudResourceOperationState.ABORTED;
                curOp.Updated = DateTime.UtcNow;
                curOp.UpdatedBy = currentUser.UserName;
            }
        }
    }
}
