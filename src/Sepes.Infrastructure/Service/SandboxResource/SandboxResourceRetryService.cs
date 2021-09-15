using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Common.Constants;
using Sepes.Common.Constants.CloudResource;
using Sepes.Common.Dto.Sandbox;
using Sepes.Common.Exceptions;
using Sepes.Common.Response.Sandbox;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class SandboxResourceRetryService : ISandboxResourceRetryService
    {
        readonly IMapper _mapper;
        readonly ILogger _logger;
        
        readonly ISandboxModelService _sandboxModelService;
        
        readonly ICloudResourceReadService _cloudResourceReadService;
        readonly IResourceOperationModelService _resourceOperationModelService;
        readonly IProvisioningQueueService _provisioningQueueService;

        public SandboxResourceRetryService(IMapper mapper, ILogger<SandboxResourceRetryService> logger, ISandboxModelService sandboxModelService,
            ICloudResourceReadService cloudResourceReadService,
            IResourceOperationModelService resourceOperationModelService,
            IProvisioningQueueService provisioningQueueService)
        {
            _mapper = mapper;
            _logger = logger;

            _sandboxModelService = sandboxModelService;
                
            _cloudResourceReadService = cloudResourceReadService;
            _resourceOperationModelService = resourceOperationModelService;
            _provisioningQueueService = provisioningQueueService;
        }


        public async Task<SandboxResourceLight> RetryResourceFailedOperation(int resourceId)
        {
            var resource = await _cloudResourceReadService.GetByIdAsync(resourceId, UserOperation.Study_Crud_Sandbox);          

            var operationToRetry = FindOperationToRetry(resource);

            if (operationToRetry == null)
            {
                throw new BadRequestException(ReScheduleResourceLogPrefix(resource, "Could not locate any relevant operation to retry"));
            }

            if (resource.ResourceType == AzureResourceType.VirtualMachine)
            {
                var sandbox = await _sandboxModelService.GetByIdForResourcesAsync(resource.SandboxId.Value);

                if (!AllSandboxResourcesOkay(sandbox))
                {
                    throw new NullReferenceException(ReScheduleResourceLogPrefix(resource, $"Cannot retry VM creation for {resource.ResourceName} when Sandbox is not setup properly", operationToRetry));
                }

                await EnsureOperationIsReadyForRetryAndEnqueue(resource, operationToRetry);
            }
            else if (resource.SandboxControlled)
            {
                if (operationToRetry.OperationType == CloudResourceOperationType.CREATE)
                {
                    //Must re-start all succeeding operations
                    await ReScheduleSandboxResourceCreation(resource.Sandbox.Id);
                }

                else
                {
                    await EnsureOperationIsReadyForRetryAndEnqueue(resource, operationToRetry);
                }
            }
            else
            {
                throw new ArgumentException($"Retry is not supported for resource type: {resource.ResourceType} ");
            }

            return _mapper.Map<SandboxResourceLight>(resource);
        }

        async Task ReScheduleSandboxResourceCreation(int sandboxId)
        {
            var sandbox = await _sandboxModelService.GetByIdForReScheduleCreateAsync(sandboxId);

            EnsureHasAllRequiredResourcesThrowIfNot(sandbox);

            var queueParentItem = QueueItemFactory.CreateParent($"Create basic resources for Sandbox (re-scheduled): {sandbox.Id}");

            foreach (var currentSandboxResource in sandbox.Resources.Where(r => r.SandboxControlled).OrderBy(r => r.Id))
            {
                var resourceCreateOperation = CloudResourceOperationUtil.GetCreateOperation(currentSandboxResource);

                if (resourceCreateOperation == null)
                {
                    throw new Exception(ReScheduleLogPrefix(sandbox.StudyId, sandbox.Id, $"Could not locate create operation for resource {currentSandboxResource.Id} - {currentSandboxResource.ResourceName}"));
                }

                if (resourceCreateOperation.Status == CloudResourceOperationState.ABANDONED)
                {
                    throw new Exception(ReScheduleLogPrefix(sandbox.StudyId, sandbox.Id, $"Create operation for resource {currentSandboxResource.Id} - {currentSandboxResource.ResourceName} was abandoned. Cannot proceed"));
                }

                await EnsureOperationIsReadyForRetryAndAddToQueueItem(currentSandboxResource, resourceCreateOperation, queueParentItem);
            }

            if (queueParentItem.Children.Count == 0)
            {
                throw new Exception(ReScheduleLogPrefix(sandbox.StudyId, sandbox.Id, $"Could not re-shedule creation. No relevant resource items found"));
            }
            else
            {
                await _provisioningQueueService.SendMessageAsync(queueParentItem);
            }
        }

        void EnsureHasAllRequiredResourcesThrowIfNot(Sandbox sandbox)
        {
            EnsureSandboxHasResourceTypeThrowIfNot(sandbox, AzureResourceType.ResourceGroup);
            EnsureSandboxHasResourceTypeThrowIfNot(sandbox, AzureResourceType.StorageAccount);
            EnsureSandboxHasResourceTypeThrowIfNot(sandbox, AzureResourceType.NetworkSecurityGroup);
            EnsureSandboxHasResourceTypeThrowIfNot(sandbox, AzureResourceType.VirtualNetwork);
            EnsureSandboxHasResourceTypeThrowIfNot(sandbox, AzureResourceType.Bastion);
        }

        void EnsureSandboxHasResourceTypeThrowIfNot(Sandbox sandbox, string resourceType)
        {
            var resource = CloudResourceUtil.GetResourceByType(sandbox.Resources, resourceType, true);

            if (resource == null)
            {
                throw new Exception($"Unable to find sandbox resource of type {resourceType}");
            }
        }

        CloudResourceOperation FindOperationToRetry(CloudResource resource)
        {
            CloudResourceOperation lastOperation = null;

            foreach (var currentOperation in resource.Operations.OrderByDescending(o => o.Created))
            {
                if (CloudResourceOperationUtil.HasValidStateForRetry(currentOperation))
                {
                    lastOperation = currentOperation;
                }
                else if (currentOperation.Status == CloudResourceOperationState.DONE_SUCCESSFUL)
                {
                    return lastOperation;
                }
            }

            return lastOperation;
        }

        bool AllSandboxResourcesOkay(Sandbox sandbox)
        {
            if (sandbox.Resources == null)
            {
                throw new Exception("Missing include for Sandbox.Resources");
            }

            var sandboxControlledResources = CloudResourceUtil.GetSandboxControlledResources(sandbox.Resources);
            return !sandboxControlledResources.Any(r => !CloudResourceOperationUtil.HasSuccessfulCreateOperation(r));       
        }

        async Task EnsureOperationIsReadyForRetryAndEnqueue(CloudResource resource, CloudResourceOperation operationToRetry)
        {
            var queueParentItem = QueueItemFactory.CreateParent(operationToRetry.Id, $"{operationToRetry.Description} (re-scheduled)");
            await EnsureOperationIsReadyForRetryAndAddToQueueItem(resource, operationToRetry, queueParentItem);
            await _provisioningQueueService.SendMessageAsync(queueParentItem);
        }

        async Task EnsureOperationIsReadyForRetryAndAddToQueueItem(CloudResource resource, CloudResourceOperation operationToRetry, ProvisioningQueueParentDto queueParentItem)
        {
            await EnsureOperationIsReadyForRetry(resource, operationToRetry);

            _logger.LogInformation(ReScheduleResourceLogPrefix(resource, $"Re-queing item", operationToRetry));

            queueParentItem.Children.Add(new ProvisioningQueueChildDto() { ResourceOperationId = operationToRetry.Id });

            _logger.LogInformation(ReScheduleResourceLogPrefix(resource, $"Item re-queued", operationToRetry));
        }

        async Task EnsureOperationIsReadyForRetry(CloudResource resource, CloudResourceOperation operationToRetry)
        {
            _logger.LogInformation(ReScheduleResourceLogPrefix(resource, $"Increasing MAX try count"), operationToRetry);

           await  _resourceOperationModelService.EnsureReadyForRetry(operationToRetry);
        }

        string ReScheduleResourceLogPrefix(CloudResource resource, string logText, CloudResourceOperation operation = null)
        {
            var logMessage = $"Re-schedule resource operation";

            if (resource.StudyId.HasValue)
            {
                logMessage += $" | Study {resource.StudyId.Value}";
            }

            if (resource.SandboxId.HasValue)
            {
                logMessage += $" | Sandbox {resource.SandboxId.Value}";
            }

            logMessage += $" | Resource: {resource.Id}";

            if (operation != null)
            {
                logMessage += $" | Operation {operation.Id} | {operation.Description}";
            }

            logMessage += $" | {logText}";

            return logMessage;
        }

        string ReScheduleLogPrefix(int studyId, int sandboxId, string logText, int resourceId = 0)
        {
            var logMessage = $"Re-schedule entire sandbox creation | Study {studyId} | Sandbox {sandboxId}";

            if (resourceId > 0)
            {
                logMessage += $" | Resource: {resourceId}";
            }

            logMessage += $" | {logText}";

            return logMessage;
        }
    }
}
