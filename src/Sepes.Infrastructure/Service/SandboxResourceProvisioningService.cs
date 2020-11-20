using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Interface;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Threading;
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
                            throw new Exception($"{CreateOperationLogMessagePrefix(currentResourceOperation)}In danger of picking up work in progress, Aborting!");
                        }
                        else if (currentResourceOperation.DependsOnOperationId.HasValue)
                        {
                            _logger.LogInformation($"{CreateOperationLogMessagePrefix(currentResourceOperation)}Operation is dependent on {currentResourceOperation.DependsOnOperationId.Value}, verifying if dependent operation is finished ");

                            if (await _sandboxResourceOperationService.OperationIsFinishedAndSucceededAsync(currentResourceOperation.DependsOnOperationId.Value) == false)
                            {
                                _logger.LogWarning($"{CreateOperationLogMessagePrefix(currentResourceOperation)}. Dependant operation {currentResourceOperation.DependsOnOperationId.Value} not finished. Queue item invisibility increased. Now aborting");

                              
                                var invisibilityIncrease = currentResourceOperation.DependsOnOperation != null ? AzureResourceProivisoningTimeoutResolver.GetTimeoutForOperationInSeconds(currentResourceOperation.DependsOnOperation.Resource.ResourceType) : CloudResourceConstants.INCREASE_QUEUE_INVISIBLE_WHEN_DEPENDENT_ON_NOT_FINISHED;
                              
                                if (invisibilityIncrease > 180) invisibilityIncrease = 180;                              
                              
                                await _workQueue.IncreaseInvisibilityAsync(queueParentItem, invisibilityIncrease);

                                //Storing queue message details in db, so that the worker who is processing the dependent operation can pick up this task when the dependent is complete. 
                                if (queueParentItem.Children.Count == 1 && queueParentItem.VisibleAt != DateTime.MinValue) //Only safe if there is only one child item/operation 
                                {
                                    currentResourceOperation = await _sandboxResourceOperationService.SaveQueueMessageDetails(currentResourceOperation.Id.Value, queueParentItem.MessageId, queueParentItem.PopReceipt, queueParentItem.VisibleAt);
                                }

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

            _logger.LogInformation($"{CreateOperationLogMessagePrefix(currentResourceOperation)}Setting operation to In Progress");

            var service = AzureResourceServiceResolver.GetCRUDService(_serviceProvider, resourceType);

            if (service == null)
            {
                throw new NullReferenceException($"ResourceOperation {queueChildItem.SandboxResourceOperationId}Unable to resolve CRUD service for type {resourceType}!");
            }

            var nsg = SandboxResourceUtil.GetSibilingResource(await _sandboxResourceService.GetByIdAsync(resource.Id.Value), AzureResourceType.NetworkSecurityGroup);

            currentCrudInput.ResetButKeepSharedVariables(currentCrudResult != null ? currentCrudResult.NewSharedVariables : null);
            currentCrudInput.Name = resource.ResourceName;
            currentCrudInput.StudyName = resource.StudyName;
            currentCrudInput.DatabaseId = resource.Id.Value;
            currentCrudInput.SandboxId = resource.SandboxId;
            currentCrudInput.SandboxName = resource.SandboxName;
            currentCrudInput.ResourceGroupName = resource.ResourceGroupName;
            currentCrudInput.Region = RegionStringConverter.Convert(resource.Region);
            currentCrudInput.NetworkSecurityGroupName = nsg != null ? nsg.ResourceName : null;
            currentCrudInput.Tags = resource.Tags;
            currentCrudInput.ConfigurationString = resource.ConfigString;
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

                    var cancellationTokenSource = new CancellationTokenSource();
                    Task<CloudResourceCRUDResult> currentCrudResultTask = null;

                    if (currentResourceOperation.OperationType == CloudResourceOperationType.CREATE)
                    {
                        _logger.LogInformation($"{CreateOperationLogMessagePrefix(currentResourceOperation)}Initial checks succeeded. Proceeding with CREATE operation");
                        currentCrudResultTask = service.EnsureCreated(currentCrudInput, cancellationTokenSource.Token);
                    }
                    else
                    {
                        _logger.LogInformation($"{CreateOperationLogMessagePrefix(currentResourceOperation)}Initial checks succeeded. Proceeding with UPDATE operation");
                        currentCrudResultTask = service.Update(currentCrudInput, cancellationTokenSource.Token);
                    }

                    DateTime updatedStatusAt = DateTime.UtcNow;

                    while (!currentCrudResultTask.IsCompleted)
                    {
                        if (await _sandboxResourceService.ResourceIsDeleted(resource.Id.Value))
                        {
                            _logger.LogInformation($"{CreateOperationLogMessagePrefix(currentResourceOperation)}Operation canceled!");
                            cancellationTokenSource.Cancel();
                            break;
                        }

                        Thread.Sleep((int)TimeSpan.FromSeconds(3).TotalMilliseconds);

                        if((DateTime.UtcNow - updatedStatusAt).TotalSeconds > 60)
                        {
                            updatedStatusAt = DateTime.UtcNow;
                            await _sandboxResourceOperationService.SetUpdatedTimestampAsync(currentResourceOperation.Id.Value);
                        }
                    }

                    currentCrudResult = currentCrudResultTask.Result;

                    if (currentResourceOperation.OperationType == CloudResourceOperationType.CREATE)
                    {
                        _logger.LogInformation($"{CreateOperationLogMessagePrefix(currentResourceOperation)}Setting resource id and name!");
                        await _sandboxResourceService.UpdateResourceIdAndName(currentResourceOperation.Resource.Id.Value, currentCrudResult.IdInTargetSystem, currentCrudResult.NameInTargetSystem);
                    }
                }
            }
            else if (currentResourceOperation.OperationType == CloudResourceOperationType.DELETE)
            {
                currentResourceOperation = await _sandboxResourceOperationService.SetInProgressAsync(currentResourceOperation.Id.Value, _requestIdService.GetRequestId(), CloudResourceOperationState.IN_PROGRESS);

                if (currentResourceOperation.Resource.ResourceType == AzureResourceType.ResourceGroup)
                {
                    _logger.LogInformation($"{CreateOperationLogMessagePrefix(currentResourceOperation)}Deleting ResourceGroup");

                    currentCrudResult = await service.Delete(currentCrudInput);
                }
                else if (currentResourceOperation.Resource.ResourceType == AzureResourceType.VirtualMachine)
                {
                    _logger.LogInformation($"{CreateOperationLogMessagePrefix(currentResourceOperation)}Deleting Virtual Machine");
                    currentCrudResult = await service.Delete(currentCrudInput);
                }
                else
                {
                    _logger.LogCritical($"{CreateOperationLogMessagePrefix(currentResourceOperation)}Attempted to delete resource with type {currentCrudResult.Resource.Type}. Only deleting resource groups are supprorted.");
                    throw new ArgumentException($"ResourceOperation {queueChildItem.SandboxResourceOperationId}Unable to resolve CRUD service for type {resourceType}!");
                }
            }

            _logger.LogInformation($"{CreateOperationLogMessagePrefix(currentResourceOperation)}Finished with provisioningState: {currentCrudResult.CurrentProvisioningState}");

            currentResourceOperation = await _sandboxResourceOperationService.UpdateStatusAsync(currentResourceOperation.Id.Value, CloudResourceOperationState.DONE_SUCCESSFUL, currentCrudResult.CurrentProvisioningState);

            //Go through operations that are depending on the one that just finished. If safe, these are re-queued immediately. 
            //This will only work if the messageId and pop receipt is stored on the operation
            if(currentResourceOperation.DependantOnThisOperation != null && currentResourceOperation.DependantOnThisOperation.Count > 0)
            {
                foreach(var curDependent in currentResourceOperation.DependantOnThisOperation)
                {
                    await CreateNewQueueItemForImmeadiateProcessing(curDependent);
                }
            }

            return currentCrudResult;
        }

        async Task<bool> CreateNewQueueItemForImmeadiateProcessing(SandboxResourceOperationDto operation)
        {
            try
            {
                if (operation.Resource.Deleted.HasValue)
                {
                    return false;
                }

                if (String.IsNullOrWhiteSpace(operation.QueueMessageId) == false && String.IsNullOrWhiteSpace(operation.QueueMessagePopReceipt) == false)
                {
                    if (operation.QueueMessageVisibleAgainAt.HasValue && operation.QueueMessageVisibleAgainAt.Value > DateTime.UtcNow.AddSeconds(15)) //Only if queue item is more than XX seconds away
                    {
                        _logger.LogInformation($"{CreateOperationLogMessagePrefix(operation)}Creating new queue item for operation");
                        var queueParentItem = new ProvisioningQueueParentDto();
                        queueParentItem.SandboxId = operation.Resource.SandboxId;
                        queueParentItem.Description = operation.Description;
                        queueParentItem.Children.Add(new ProvisioningQueueChildDto() { SandboxResourceOperationId = operation.Id.Value });
                        await _workQueue.SendMessageAsync(queueParentItem);

                        //Delete old queue item
                        _logger.LogInformation($"{CreateOperationLogMessagePrefix(operation)}Deleting original queue item");
                        await _workQueue.DeleteMessageAsync(operation.QueueMessageId, operation.QueueMessagePopReceipt);
                        await _sandboxResourceOperationService.ClearQueueMessageDetails(operation.Id.Value);

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Unable to create new Queue item for operation {operation.Id}", ex);  
            }

            return false;
        }


        string CreateOperationLogMessagePrefix(SandboxResourceOperationDto currentResourceOperation)
        {
            return $"{currentResourceOperation.Id} | {currentResourceOperation.Resource.ResourceType} | {currentResourceOperation.OperationType} | ";
        }

        bool MightBeInProgressByAnotherThread(SandboxResourceOperationDto currentResourceOperation)
        {
            if (currentResourceOperation.Status == CloudResourceOperationState.IN_PROGRESS)
            {
                if (currentResourceOperation.Updated.AddMinutes(2) < DateTime.UtcNow)
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
