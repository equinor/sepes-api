using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Response.Sandbox;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util.Provisioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class SandboxResourceRetryService : SandboxServiceBase, ISandboxResourceRetryService
    {       
        readonly ICloudResourceReadService _cloudResourceService;
        readonly IProvisioningQueueService _provisioningQueueService;

        public SandboxResourceRetryService(IConfiguration config, SepesDbContext db, IMapper mapper, ILogger<SandboxResourceDeleteService> logger, IUserService userService,               
            ICloudResourceReadService cloudResourceService,            
            IProvisioningQueueService provisioningQueueService)
              : base(config, db, mapper, logger, userService)
        {          
            _cloudResourceService = cloudResourceService;            
            _provisioningQueueService = provisioningQueueService;
        }       

       
        public async Task ReScheduleSandboxResourceCreation(int sandboxId)
        {
            var sandboxFromDb = await GetOrThrowAsync(sandboxId, UserOperation.Study_Crud_Sandbox, true);

            var queueParentItem = QueueItemFactory.CreateParent($"Create basic resources for Sandbox (re-scheduled): {sandboxFromDb.Id}");

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
                    queueParentItem.Children.Add(new ProvisioningQueueChildDto() { ResourceOperationId = relevantOperation.Id });
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
        public async Task<SandboxResourceLight> RetryLastOperation(int resourceId)
        {
            var resource = await _cloudResourceService.GetByIdAsync(resourceId);

            var sandboxFromDb = await GetOrThrowAsync(resource.SandboxId.Value, UserOperation.Study_Crud_Sandbox, true);

            if (resource.ResourceType != AzureResourceType.VirtualMachine)
            {
                throw new ArgumentException("Retry is only supported for Virtual Machines");
            }

            var relevantOperation = resource.Operations.OrderByDescending(o => o.Created).FirstOrDefault();

            if (relevantOperation == null)
            {
                throw new NullReferenceException(ReScheduleLogPrefix(sandboxFromDb.StudyId, sandboxFromDb.Id, "Could not locate ANY database entry for VM", resourceId));
            }
            else if (String.IsNullOrWhiteSpace(relevantOperation.Status) || relevantOperation.Status == CloudResourceOperationState.NEW || relevantOperation.Status == CloudResourceOperationState.IN_PROGRESS || relevantOperation.Status == CloudResourceOperationState.FAILED || relevantOperation.Status == CloudResourceOperationState.DONE_SUCCESSFUL)
            {
                _logger.LogInformation(ReScheduleLogPrefix(sandboxFromDb.StudyId, sandboxFromDb.Id, $"Increasing MAX try count", resourceId));

                relevantOperation.MaxTryCount += CloudResourceConstants.RESOURCE_MAX_TRY_COUNT; //Increase max try count  

                _logger.LogInformation(ReScheduleLogPrefix(sandboxFromDb.StudyId, sandboxFromDb.Id, $"Re-queing item. Previous status was {relevantOperation.Status}", resourceId));                              

                await _db.SaveChangesAsync();

                var queueParentItem = QueueItemFactory.CreateParent(relevantOperation.Id, $"{relevantOperation.Description} (re-scheduled)");
              
                await _provisioningQueueService.SendMessageAsync(queueParentItem);
            }
            else
            {
                throw new Exception(ReScheduleLogPrefix(sandboxFromDb.StudyId, sandboxFromDb.Id, $"Could not locate RELEVANT database entry for ResourceGroupOperation", resourceId));
            }

            return _mapper.Map<SandboxResourceLight>(resource);
        }
    }
}
