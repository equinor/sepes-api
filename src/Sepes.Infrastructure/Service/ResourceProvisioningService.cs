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
    public class ResourceProvisioningService : IResourceProvisioningService
    {
        readonly ILogger _logger;
        readonly IServiceProvider _serviceProvider;
        readonly IRequestIdService _requestIdService;
        readonly ISandboxResourceService _sandboxResourceService;
        readonly ISandboxResourceOperationService _sandboxResourceOperationService;
        readonly IProvisioningQueueService _workQueue;
        readonly ISandboxResourceMonitoringService _monitoringService;

        public static readonly string UnitTestPrefix = "unit-test";

        public ResourceProvisioningService(ILogger<ResourceProvisioningService> logger, IServiceProvider serviceProvider, IRequestIdService requestIdService, ISandboxResourceService sandboxResourceService, ISandboxResourceOperationService sandboxResourceOperationService, IProvisioningQueueService workQueue, ISandboxResourceMonitoringService monitoringService)
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

        async Task<bool> CheckAndHandleTryCount(ProvisioningQueueParentDto queueParentItem, SandboxResourceOperationDto currentResourceOperation)
        {
            if (currentResourceOperation.TryCount >= currentResourceOperation.MaxTryCount)
            {
                _logger.LogWarning($"{CreateOperationLogMessagePrefix(currentResourceOperation)}Max retry count exceeded: {currentResourceOperation.TryCount}, Aborting!");

                currentResourceOperation = await _sandboxResourceOperationService.UpdateStatusAsync(currentResourceOperation.Id, CloudResourceOperationState.FAILED);
                await _workQueue.DeleteMessageAsync(queueParentItem);

                return true;
            }

            return false;

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
                            throw new TaskCanceledException($"{CreateOperationLogMessagePrefix(currentResourceOperation)}Resource is marked for deletion in database, Aborting!");
                        }
                        else if (currentResourceOperation.TryCount >= currentResourceOperation.MaxTryCount)
                        {
                            _logger.LogWarning($"{CreateOperationLogMessagePrefix(currentResourceOperation)}Max retry count exceeded: {currentResourceOperation.TryCount}, Aborting!");

                            currentResourceOperation = await _sandboxResourceOperationService.UpdateStatusAsync(currentResourceOperation.Id, CloudResourceOperationState.FAILED);
                            await _workQueue.DeleteMessageAsync(queueParentItem);
                            deleteFromQueueAfterCompletion = false; //Has allready been done
                            break;
                        }
                        else if (String.IsNullOrWhiteSpace(currentResourceOperation.Status) == false && currentResourceOperation.Status == CloudResourceOperationState.ABORTED)
                        {
                            throw new TaskCanceledException($"{CreateOperationLogMessagePrefix(currentResourceOperation)}Has aborted state!");
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
                                if (invisibilityIncrease > 180) invisibilityIncrease = 180;

                                await _workQueue.IncreaseInvisibilityAsync(queueParentItem, invisibilityIncrease);
                                deleteFromQueueAfterCompletion = false;
                                break;
                            }
                        }

                        currentCrudResult = await HandleCRUD(queueParentItem, queueChildItem, currentResourceOperation, currentCrudInput, currentCrudResult);
                    }                  
                   
                    catch (Exception ex)
                    {
                        if(ex is TaskCanceledException || ( ex.InnerException != null && ex.InnerException is TaskCanceledException))
                        {
                            if (currentResourceOperation != null)
                            {
                                currentResourceOperation = await _sandboxResourceOperationService.UpdateStatusAsync(currentResourceOperation.Id, CloudResourceOperationState.ABORTED, errorMessage: AzureResourceUtil.CreateResourceOperationErrorMessage(ex));
                            }
                        }
                        else
                        {
                            if (currentResourceOperation != null)
                            {
                                currentResourceOperation = await _sandboxResourceOperationService.UpdateStatusAsync(currentResourceOperation.Id, CloudResourceOperationState.FAILED, errorMessage: AzureResourceUtil.CreateResourceOperationErrorMessage(ex));

                                if (currentResourceOperation.TryCount < currentResourceOperation.MaxTryCount && queueParentItem != null && queueParentItem.DequeueCount == 5)
                                {
                                    await _workQueue.ReQueueMessageAsync(queueParentItem);
                                }
                                else
                                {
                                    await _workQueue.IncreaseInvisibilityAsync(queueParentItem, 10);
                                }

                            }

                          
                        }                     

                        throw;
                    }//catch
                } //foreach

                if (deleteFromQueueAfterCompletion)
                {
                    _logger.LogInformation($"Deleting handling queue message: {queueParentItem.MessageId}");
                    await _workQueue.DeleteMessageAsync(queueParentItem);
                }            

                _logger.LogInformation($"Finished handling queue message: {queueParentItem.MessageId}");

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

            var nsg = SandboxResourceUtil.GetSibilingResource(await _sandboxResourceService.GetByIdAsync(resource.Id), AzureResourceType.NetworkSecurityGroup);

            currentCrudInput.ResetButKeepSharedVariables(currentCrudResult != null ? currentCrudResult.NewSharedVariables : null);
            currentCrudInput.Name = resource.ResourceName;
            currentCrudInput.StudyName = resource.StudyName;
            currentCrudInput.DatabaseId = resource.Id;
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
                    return currentCrudResult;
                }
                else
                {
                    currentResourceOperation = await _sandboxResourceOperationService.SetInProgressAsync(currentResourceOperation.Id, _requestIdService.GetRequestId(), CloudResourceOperationState.IN_PROGRESS);


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

                    while (!currentCrudResultTask.IsCompleted)
                    {
                        currentResourceOperation = await _sandboxResourceOperationService.GetByIdAsync(currentResourceOperation.Id);

                        if (await _sandboxResourceService.ResourceIsDeleted(resource.Id) || currentResourceOperation.Status == CloudResourceOperationState.ABORTED)
                        {
                            _logger.LogInformation($"{CreateOperationLogMessagePrefix(currentResourceOperation)}Operation canceled!");
                            cancellationTokenSource.Cancel();
                            break;
                        }

                        Thread.Sleep((int)TimeSpan.FromSeconds(3).TotalMilliseconds);
                    }

                    currentCrudResult = currentCrudResultTask.Result;

                    if (currentResourceOperation.OperationType == CloudResourceOperationType.CREATE)
                    {
                        _logger.LogInformation($"{CreateOperationLogMessagePrefix(currentResourceOperation)}Setting resource id and name!");
                        await _sandboxResourceService.UpdateResourceIdAndName(currentResourceOperation.Resource.Id, currentCrudResult.IdInTargetSystem, currentCrudResult.NameInTargetSystem);
                    }
                }
            }
            else if (currentResourceOperation.OperationType == CloudResourceOperationType.DELETE)
            {
                currentResourceOperation = await _sandboxResourceOperationService.SetInProgressAsync(currentResourceOperation.Id, _requestIdService.GetRequestId(), CloudResourceOperationState.IN_PROGRESS);

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

            await _sandboxResourceOperationService.UpdateStatusAsync(currentResourceOperation.Id, CloudResourceOperationState.DONE_SUCCESSFUL, updatedProvisioningState: currentCrudResult.CurrentProvisioningState);

            return currentCrudResult;
        }

        string CreateOperationLogMessagePrefix(SandboxResourceOperationDto currentResourceOperation)
        {
            return $"{currentResourceOperation.Id} | {currentResourceOperation.Resource.ResourceType} | {currentResourceOperation.OperationType.ToUpper()} | attempt ({currentResourceOperation.TryCount}/{currentResourceOperation.MaxTryCount}) | ";
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
