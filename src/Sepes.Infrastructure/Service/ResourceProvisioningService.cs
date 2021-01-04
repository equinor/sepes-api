using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Provisioning;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using Sepes.Infrastructure.Util.Provisioning;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class ResourceProvisioningService : IResourceProvisioningService
    {
        readonly ILogger _logger;
        readonly IServiceProvider _serviceProvider;
        readonly IRequestIdService _requestIdService;
        readonly IProvisioningQueueService _workQueue;
        readonly ICloudResourceReadService _resourceReadService;
        readonly ICloudResourceUpdateService _resourceUpdateService;
        readonly ICloudResourceOperationReadService _resourceOperationReadService;
        readonly ICloudResourceOperationUpdateService _resourceOperationUpdateService;
        readonly ICloudResourceMonitoringService _monitoringService;

        public ResourceProvisioningService(
            ILogger<ResourceProvisioningService> logger,
            IServiceProvider serviceProvider,
            IRequestIdService requestIdService,
            IProvisioningQueueService workQueue,
            ICloudResourceReadService resourceService,
            ICloudResourceUpdateService resourceUpdateService,
            ICloudResourceOperationReadService resourceOperationReadService,
            ICloudResourceOperationUpdateService resourceOperationUpdateService,
            ICloudResourceMonitoringService monitoringService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            _requestIdService = requestIdService;
            _resourceReadService = resourceService ?? throw new ArgumentNullException(nameof(resourceService));
            _resourceUpdateService = resourceUpdateService ?? throw new ArgumentNullException(nameof(resourceUpdateService));
            _resourceOperationReadService = resourceOperationReadService ?? throw new ArgumentNullException(nameof(resourceOperationReadService));
            _resourceOperationUpdateService = resourceOperationUpdateService;
            _workQueue = workQueue ?? throw new ArgumentNullException(nameof(workQueue));
            _monitoringService = monitoringService;
        }

        public async Task DequeueAndHandleWork()
        {
            var work = await _workQueue.RecieveMessageAsync();

            while (work != null)
            {
                await HandleWork(work);
                work = await _workQueue.RecieveMessageAsync();
            }
        }


        public async Task HandleWork(ProvisioningQueueParentDto queueParentItem)
        {
            _logger.LogInformation(ProvisioningLogUtil.QueueParent(queueParentItem));

            //One per child item in queue item
            CloudResourceOperationDto currentOperation = null;

            //Get's re-used amonong child elements because the operations might share variables
            var currentProvisioningParameters = new ResourceProvisioningParameters();

            ResourceProvisioningResult currentProvisioningResult = null;

            try
            {
                foreach (var queueChildItem in queueParentItem.Children)
                {
                    try
                    {
                        currentOperation = await _resourceOperationReadService.GetByIdAsync(queueChildItem.ResourceOperationId);

                        _logger.LogInformation(ProvisioningLogUtil.Operation(currentOperation, "Starting operation"));

                        OperationCheckUtils.ThrowIfTryCountExceededOrAborted(currentOperation);

                        OperationCheckUtils.ThrowIfResourceIsDeletedAndOperationIsNotADelete(currentOperation);

                        OperationCheckUtils.ThrowIfPossiblyInProgress(currentOperation);

                        await OperationCheckUtils.ThrowIfDependentOnUnfinishedOperationAsync(currentOperation, queueParentItem, _resourceOperationReadService);

                        var provisioningService = AzureResourceServiceResolver.GetProvisioningServiceOrThrow(_serviceProvider, currentOperation.Resource.ResourceType);

                        await ProvisioningParamaterUtil.PrepareForNewOperation(currentProvisioningParameters, currentOperation, currentProvisioningResult, _resourceReadService);

                        await ProvisioningQueueUtil.IncreaseInvisibleBasedOnResource(currentOperation, queueParentItem, _workQueue);

                        if (CreateAndUpdateUtil.WillBeHandledAsCreateOrUpdate(currentOperation))
                        {
                            if (await OperationCompletedUtil.HandledAsAllreadyCompletedAsync(currentOperation, _monitoringService))
                            {
                                currentProvisioningResult = await provisioningService.GetSharedVariables(currentProvisioningParameters);
                                continue;
                            }
                            else
                            {
                                currentOperation = await _resourceOperationUpdateService.SetInProgressAsync(currentOperation.Id, _requestIdService.GetRequestId());
                                currentProvisioningResult = await CreateAndUpdateUtil.HandleCreateOrUpdate(currentOperation, currentProvisioningParameters, provisioningService, _resourceReadService, _resourceUpdateService, _resourceOperationUpdateService, _logger);
                            }

                            await _resourceOperationUpdateService.UpdateStatusAsync(currentOperation.Id, CloudResourceOperationState.DONE_SUCCESSFUL, updatedProvisioningState: currentProvisioningResult.CurrentProvisioningState);
                        }
                        else if (DeleteOperationUtil.WillBeHandledAsDelete(currentOperation))
                        {
                            currentOperation = await _resourceOperationUpdateService.SetInProgressAsync(currentOperation.Id, _requestIdService.GetRequestId());
                            currentProvisioningResult = await DeleteOperationUtil.HandleDelete(currentOperation, currentProvisioningParameters, provisioningService, _resourceOperationUpdateService, _logger);
                            await _resourceOperationUpdateService.UpdateStatusAsync(currentOperation.Id, CloudResourceOperationState.DONE_SUCCESSFUL, updatedProvisioningState: currentProvisioningResult.CurrentProvisioningState);
                        }
                        else
                        {
                            throw new ProvisioningException("Unknown operation type", CloudResourceOperationState.ABORTED);
                        }

                        _logger.LogInformation(ProvisioningLogUtil.Operation(currentOperation, "Successfully handeled operation"));
                    }
                    catch (ProvisioningException ex) //Inner loop, ordinary exception is not catched
                    {
                        if (ex.LogAsWarning)
                        {
                            _logger.LogWarning(ex, ProvisioningLogUtil.Operation(currentOperation, "Operation aborted"));
                        }
                        else
                        {
                            _logger.LogError(ex, ProvisioningLogUtil.Operation(currentOperation, "Operation failed"));
                        }                      

                        if (String.IsNullOrWhiteSpace(ex.NewOperationStatus) == false)
                        {
                            currentOperation = await _resourceOperationUpdateService.UpdateStatusAsync(currentOperation.Id, ex.NewOperationStatus);
                        }

                        if (ex.ProceedWithOtherOperations == false)
                        {
                            throw;
                        }
                    }


                } //foreach               

                _logger.LogInformation($"Done handling message: {queueParentItem.MessageId}");

                await _workQueue.DeleteMessageAsync(queueParentItem);
                await MoveUpAnyDependentOperations(queueParentItem);

            }
            catch (ProvisioningException ex) //Outer loop catch 1
            {
                if (ex.DeleteFromQueue)
                {
                    _logger.LogWarning($"Deleting message {queueParentItem.MessageId} from queue after exception");
                    await _workQueue.DeleteMessageAsync(queueParentItem);
                }
                else if (ex.PostponeQueueItemFor.HasValue && ex.PostponeQueueItemFor.Value > 0)
                {
                    if (currentOperation.TryCount < currentOperation.MaxTryCount)
                    {
                        if (queueParentItem.DequeueCount == 5)
                        {
                            _logger.LogWarning($"Re-queing message {queueParentItem.MessageId} after exception");
                            await _workQueue.ReQueueMessageAsync(queueParentItem, ex.PostponeQueueItemFor.Value);
                        }
                        else
                        {
                            _logger.LogWarning($"Increasing invisibility for message {queueParentItem.MessageId} after exception");
                            await _workQueue.IncreaseInvisibilityAsync(queueParentItem, ex.PostponeQueueItemFor.Value);
                        }
                    }
                }

                if (ex.StoreQueueInfoOnOperation)
                {
                    if(queueParentItem.NextVisibleOn.HasValue == false)
                    {
                        _logger.LogError($"Could not store queue info on operation from message {queueParentItem.MessageId}, no next visible time exist");
                    }
                    else
                    {
                        currentOperation = await _resourceOperationUpdateService.SetQueueInformationAsync(currentOperation.Id, queueParentItem.MessageId, queueParentItem.PopReceipt, queueParentItem.NextVisibleOn.Value);
                    }
                    
                }
            }
            catch (Exception ex) //Outer loop catch 2
            {
                _logger.LogCritical(ex, $"Unhandeled exception occured while processing message {queueParentItem.MessageId}, message description: {queueParentItem.Description}. See exception info for details ");
                await _workQueue.DeleteMessageAsync(queueParentItem);
            }
        }

        public async Task MoveUpAnyDependentOperations(ProvisioningQueueParentDto queueParentItem)
        {
            _logger.LogInformation($"Moving up relevant dependent operations on message: {queueParentItem.MessageId}");

            try
            {
                int movedUpCount = 0;

                var operationsToMoveUp = new List<CloudResourceOperationDto>();
                CloudResourceOperationDto currentOperation = null;

                foreach (var queueChildItem in queueParentItem.Children)
                {
                    currentOperation = await _resourceOperationReadService.GetByIdAsync(queueChildItem.ResourceOperationId);

                    if (currentOperation.DependantOnThisOperation != null && currentOperation.DependantOnThisOperation.Count > 0)
                    {
                        foreach (var curDependantOnThisOp in currentOperation.DependantOnThisOperation)
                        {
                            if (CloudResourceUtil.IsDeleted(curDependantOnThisOp.Resource) == false)
                            {
                                if (curDependantOnThisOp.Status == CloudResourceOperationState.NEW && String.IsNullOrWhiteSpace(curDependantOnThisOp.BatchId))
                                {
                                    if (String.IsNullOrWhiteSpace(curDependantOnThisOp.QueueMessageId) == false && String.IsNullOrWhiteSpace(curDependantOnThisOp.QueueMessagePopReceipt))
                                    {
                                        if (curDependantOnThisOp.QueueMessageVisibleAgainAt.HasValue && curDependantOnThisOp.QueueMessageVisibleAgainAt.Value > DateTime.UtcNow.AddSeconds(15))
                                        {
                                            //Create a new queue item for immediate pickup
                                            await AddNewQueueMessageForOperation(curDependantOnThisOp);

                                            //Delete existing message
                                            await _workQueue.DeleteMessageAsync(curDependantOnThisOp.QueueMessageId, curDependantOnThisOp.QueueMessagePopReceipt);

                                            //Clear stored message details on operation record
                                            await _resourceOperationUpdateService.ClearQueueInformationAsync(curDependantOnThisOp.Id);                                            
                                        }

                                    }
                                }
                            }
                        }
                    }
                }

                _logger.LogInformation($"Done moving up relevant dependent operations on message: {queueParentItem.MessageId}. Moved up {movedUpCount} operations");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed when moving up dependent operations for message: {queueParentItem.MessageId}");               
            }
        }

        async Task AddNewQueueMessageForOperation(CloudResourceOperationDto operation)
        {
            var queueParentItem = new ProvisioningQueueParentDto
            {
                SandboxId = operation.Resource.SandboxId,
                Description = operation.Description
            };

            queueParentItem.Children.Add(new ProvisioningQueueChildDto() { ResourceOperationId = operation.Id });

            await _workQueue.SendMessageAsync(queueParentItem, visibilityTimeout: TimeSpan.FromSeconds(5));
        }
    }
}
