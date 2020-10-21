using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Interface;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class SandboxResourceProvisioningService : ISandboxResourceProvisioningService
    {
        readonly ILogger _logger;
        readonly IServiceProvider _serviceProvider;
        readonly IRequestIdService _requestIdService;
        readonly ISandboxResourceService _sandboxResourceService;
        readonly ISandboxResourceOperationService _sandboxResourceOperationService;
        readonly IProvisioningQueueService _workQueue;
        readonly ISandboxResourceMonitoringService _monitoringService;

        public static readonly string UnitTestPrefix = "unit-test";

        public SandboxResourceProvisioningService(ILogger<SandboxResourceProvisioningService> logger, IServiceProvider serviceProvider, IRequestIdService requestIdService, ISandboxResourceService sandboxResourceService, ISandboxResourceOperationService sandboxResourceOperationService, IProvisioningQueueService workQueue, ISandboxResourceMonitoringService monitoringService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            _requestIdService = requestIdService;
            _sandboxResourceService = sandboxResourceService ?? throw new ArgumentNullException(nameof(sandboxResourceService));
            _sandboxResourceOperationService = sandboxResourceOperationService ?? throw new ArgumentNullException(nameof(sandboxResourceOperationService));
            _workQueue = workQueue ?? throw new ArgumentNullException(nameof(workQueue));
            _monitoringService = monitoringService;
        }

        public async Task DequeueWorkAndPerformIfAny()
        {
            var work = await _workQueue.RecieveMessageAsync();

            while (work != null)
            {
                await HandleQueueItem(work);
                work = await _workQueue.RecieveMessageAsync();
            }
        }

        public async Task HandleQueueItem(ProvisioningQueueParentDto queueParentItem)
        {
            _logger.LogInformation($"Handling message: {queueParentItem.MessageId}. Message description: {queueParentItem.Description}");

            //One per child item in queue item
            SandboxResourceOperationDto currentResourceOperation = null;

            //Get's re-used amonong child elements because the operations might share variables
            var currentCrudInput = new CloudResourceCRUDInput();

            CloudResourceCRUDResult currentCrudResult = null;

            var deleteFromQueueAfterCompletion = true;

            try
            {
                foreach (var queueChildItem in queueParentItem.Children)
                {
                    try
                    {
                        currentResourceOperation = await _sandboxResourceOperationService.GetByIdAsync(queueChildItem.SandboxResourceOperationId);

                        _logger.LogInformation($"{CreateOperationLogMessagePrefix(currentResourceOperation)}Starting operation");

                        if (currentResourceOperation.OperationType != CloudResourceOperationType.DELETE && currentResourceOperation.Resource.Deleted.HasValue)
                        {
                            //cannot recover from this
                            await _workQueue.DeleteMessageAsync(queueParentItem);
                            throw new Exception($"{CreateOperationLogMessagePrefix(currentResourceOperation)}Resource is marked for deletion in database, Aborting!");
                        }
                        else if (currentResourceOperation.TryCount >= currentResourceOperation.MaxTryCount)
                        {
                            _logger.LogWarning($"{CreateOperationLogMessagePrefix(currentResourceOperation)}Max retry count exceeded: {currentResourceOperation.TryCount}, Aborting!");

                            currentResourceOperation = await _sandboxResourceOperationService.UpdateStatusAsync(currentResourceOperation.Id.Value, CloudResourceOperationState.FAILED);
                            await _workQueue.DeleteMessageAsync(queueParentItem);
                            deleteFromQueueAfterCompletion = false; //Has allready been done
                            break;
                        }
                        else if (currentResourceOperation.OperationType != CloudResourceOperationType.DELETE && MightBeInProgressByAnotherThread(currentResourceOperation))
                        {
                            //cannot recover from this
                            throw new Exception($"{CreateOperationLogMessagePrefix(currentResourceOperation)}In danger of picking up work in progress, Aborting!");
                        }
                        else if (currentResourceOperation.DependsOnOperationId.HasValue)
                        {
                            _logger.LogInformation($"{CreateOperationLogMessagePrefix(currentResourceOperation)}Operation is dependent on {currentResourceOperation.DependsOnOperationId.Value}, verifying if dependent operation is finished ");

                            if (await _sandboxResourceOperationService.OperationIsFinishedAndSucceededAsync(currentResourceOperation.DependsOnOperationId.Value) == false)
                            {
                                _logger.LogWarning($"{CreateOperationLogMessagePrefix(currentResourceOperation)}. Dependant operation {currentResourceOperation.DependsOnOperationId.Value} not finished. Queue item invisibility increased. Now aborting");

                                var invisibilityIncrease = currentResourceOperation.DependsOnOperation != null ? AzureResourceProivisoningTimeoutResolver.GetTimeoutForOperationInSeconds(currentResourceOperation.DependsOnOperation.Resource.ResourceType) : CloudResourceConstants.INCREASE_QUEUE_INVISIBLE_WHEN_DEPENDENT_ON_NOT_FINISHED;

                                await _workQueue.IncreaseInvisibilityAsync(queueParentItem, invisibilityIncrease);
                                deleteFromQueueAfterCompletion = false;
                                break;
                            }
                        }

                        currentCrudResult = await HandleCRUD(queueParentItem, queueChildItem, currentResourceOperation, currentCrudInput, currentCrudResult);
                    }
                    catch (Exception ex)
                    {
                        if (currentResourceOperation != null)
                        {
                            await _sandboxResourceOperationService.UpdateStatusAndIncreaseTryCountAsync(currentResourceOperation.Id.Value, CloudResourceOperationState.FAILED, AzureResourceUtil.CreateResourceOperationErrorMessage(ex));
                        }

                        throw;
                    }
                }

                _logger.LogInformation($"Finished handling queue message: {queueParentItem.MessageId}");

                if (deleteFromQueueAfterCompletion)
                {
                    await _workQueue.DeleteMessageAsync(queueParentItem);
                }

            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Error occured while processing message {queueParentItem.MessageId}, message description: {queueParentItem.Description}. See exception info for details ");
            }
        }       

        async Task<CloudResourceCRUDResult> HandleCRUD(ProvisioningQueueParentDto queueParentItem, ProvisioningQueueChildDto queueChildItem, SandboxResourceOperationDto currentResourceOperation, CloudResourceCRUDInput currentCrudInput, CloudResourceCRUDResult currentCrudResult)
        {

            var resource = currentResourceOperation.Resource;
            var resourceType = currentResourceOperation.Resource.ResourceType;

            //Update operation with request id and "in progress" state

            _logger.LogInformation($"{CreateOperationLogMessagePrefix(currentResourceOperation)}Setting operation to In Progress");
          

            var service = AzureResourceServiceResolver.GetCRUDService(_serviceProvider, resourceType);

            if (service == null)
            {
                throw new NullReferenceException($"ResourceOperation {queueChildItem.SandboxResourceOperationId}Unable to resolve CRUD service for type {resourceType}!");
            }

            currentCrudInput.ResetButKeepSharedVariables(currentCrudResult != null ? currentCrudResult.NewSharedVariables : null);
            currentCrudInput.Name = resource.ResourceName;
            currentCrudInput.StudyName = resource.StudyName;
            currentCrudInput.SandboxId = resource.SandboxId;
            currentCrudInput.SandboxName = resource.SandboxName;
            currentCrudInput.ResourceGrupName = resource.ResourceGroupName;
            currentCrudInput.Region = RegionStringConverter.Convert(resource.Region);
            currentCrudInput.Tags = resource.Tags;
            currentCrudInput.CustomConfiguration = resource.ConfigString;

            currentCrudResult = null;

            var increaseQueueItemInvisibilityBy = AzureResourceProivisoningTimeoutResolver.GetTimeoutForOperationInSeconds(resourceType, currentResourceOperation.OperationType);
            await _workQueue.IncreaseInvisibilityAsync(queueParentItem, increaseQueueItemInvisibilityBy);

            if (currentResourceOperation.OperationType == CloudResourceOperationType.CREATE || currentResourceOperation.OperationType == CloudResourceOperationType.UPDATE)
            {
                if (AllreadyCompleted(currentResourceOperation))
                {
                    _logger.LogInformation($"{CreateOperationLogMessagePrefix(currentResourceOperation)}Operation allready completed. Resource should exist. Getting provsioning state");
                    await ThrowIfUnexpectedProvisioningStateAsync(currentResourceOperation);     //cannot recover from this                  
                    currentCrudResult = await service.GetSharedVariables(currentCrudInput);
                }
                else
                {
                    currentResourceOperation = await _sandboxResourceOperationService.SetInProgressAsync(currentResourceOperation.Id.Value, _requestIdService.GetRequestId(), CloudResourceOperationState.IN_PROGRESS);
                    _logger.LogInformation($"{CreateOperationLogMessagePrefix(currentResourceOperation)}Initial checks succeeded. Proceeding with create");
                    currentCrudResult = await service.EnsureCreatedAndConfigured(currentCrudInput);
                    await _sandboxResourceService.UpdateResourceIdAndName(currentResourceOperation.Resource.Id.Value, currentCrudResult.IdInTargetSystem, currentCrudResult.NameInTargetSystem);
                }
            }          
            else if (currentResourceOperation.OperationType == CloudResourceOperationType.DELETE)
            {
                currentResourceOperation = await _sandboxResourceOperationService.SetInProgressAsync(currentResourceOperation.Id.Value, _requestIdService.GetRequestId(), CloudResourceOperationState.IN_PROGRESS);

                if (currentResourceOperation.Resource.ResourceType == AzureResourceType.ResourceGroup)
                {
                    currentCrudResult = await service.Delete(currentCrudInput);
                }
                else if (currentResourceOperation.Resource.ResourceType == AzureResourceType.VirtualMachine)
                {
                    currentCrudResult = await service.Delete(currentCrudInput);
                }
                else
                {
                    _logger.LogCritical($"{CreateOperationLogMessagePrefix(currentResourceOperation)}Attempted to delete resource with type {currentCrudResult.Resource.Type}. Only deleting resource groups are supprorted.");
                    throw new ArgumentException($"ResourceOperation {queueChildItem.SandboxResourceOperationId}Unable to resolve CRUD service for type {resourceType}!");
                }
            }

            _logger.LogInformation($"{CreateOperationLogMessagePrefix(currentResourceOperation)}Finished with provisioningState: {currentCrudResult.CurrentProvisioningState}");

            await _sandboxResourceOperationService.UpdateStatusAsync(currentResourceOperation.Id.Value, CloudResourceOperationState.DONE_SUCCESSFUL, currentCrudResult.CurrentProvisioningState);

            return currentCrudResult;
        }

        string CreateOperationLogMessagePrefix(SandboxResourceOperationDto currentResourceOperation)
        {
            return $"{currentResourceOperation.Id} | {currentResourceOperation.Resource.ResourceType} | {currentResourceOperation.OperationType} | ";
        }

        bool MightBeInProgressByAnotherThread(SandboxResourceOperationDto currentResourceOperation)
        {
            if (currentResourceOperation.Status == CloudResourceOperationState.IN_PROGRESS)
            {
                //Todo: Check if allready created

                if (currentResourceOperation.Updated.AddMinutes(20) < DateTime.UtcNow)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        bool AllreadyCompleted(SandboxResourceOperationDto currentResourceOperation)
        {
            if (currentResourceOperation.Status == CloudResourceOperationState.DONE_SUCCESSFUL)
            {
                return true;
            }

            return false;
        }

        async Task ThrowIfUnexpectedProvisioningStateAsync(SandboxResourceOperationDto currentResourceOperation)
        {
            var currentProvisioningState = await _monitoringService.GetProvisioningState(currentResourceOperation.Resource);

            if (!String.IsNullOrWhiteSpace(currentProvisioningState))
            {
                if (currentResourceOperation.OperationType == CloudResourceOperationType.CREATE || currentResourceOperation.OperationType == CloudResourceOperationType.UPDATE)
                {
                    if (currentProvisioningState == "Succeeded")
                    {
                        return;
                    }
                }
            }

            throw new Exception($"{CreateOperationLogMessagePrefix(currentResourceOperation)}Aborting! Comnponent should have been created, but provisioning state is not as expexted: {currentProvisioningState}");
        }




        //public async Task NukeUnitTestSandboxes()
        //{
        //    var deleteTasks = new List<Task>();

        //    //Get list of resource groups
        //    var resourceGroups = await _resourceGroupService.GetResourceGroupsAsList();
        //    foreach (var resourceGroup in resourceGroups)
        //    {
        //        //If resource group has unit-test prefix, nuke it
        //        if (resourceGroup.Name.Contains(SandboxResourceProvisioningService.UnitTestPrefix))
        //        {
        //            // TODO: Mark as deleted in SEPES DB
        //            deleteTasks.Add(_resourceGroupService.Delete(resourceGroup.Name));
        //        }
        //    }

        //    await Task.WhenAll(deleteTasks);
        //    return;
        //}


    }
}
