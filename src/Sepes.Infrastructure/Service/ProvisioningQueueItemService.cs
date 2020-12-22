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
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class ProvisioningQueueItemService : IResourceProvisioningService
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

        public ProvisioningQueueItemService(ILogger<ResourceProvisioningService> logger,
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
            _logger.LogInformation(ProvisioningLogUtil.QueueParent(queueParentItem));

            //One per child item in queue item
            CloudResourceOperationDto currentOperation = null;

            //Get's re-used amonong child elements because the operations might share variables
            var currentProvisioningParameters = new ResourceProvisioningParameters();

            ResourceProvisioningResult currentProvisioningResult = null;

            var deleteFromQueueAfterCompletion = true;

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

                        await OperationCheckUtils.ThrowIfDependentOnUnfinishedOperationAsync(currentOperation, _resourceOperationReadService, _resourceOperationUpdateService);

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
                                currentProvisioningResult = await CreateAndUpdateUtil.HandleCreateOrUpdate(currentOperation, currentProvisioningParameters, provisioningService, _resourceReadService, _resourceUpdateService, _resourceOperationReadService, _logger);
                            }

                            await _resourceOperationUpdateService.UpdateStatusAsync(currentOperation.Id, CloudResourceOperationState.DONE_SUCCESSFUL, updatedProvisioningState: currentProvisioningResult.CurrentProvisioningState);
                        }
                        else if (DeleteOperationUtil.WillBeHandledAsDelete(currentOperation))
                        {
                            currentOperation = await _resourceOperationUpdateService.SetInProgressAsync(currentOperation.Id, _requestIdService.GetRequestId());
                            currentProvisioningResult = await DeleteOperationUtil.HandleDelete(currentOperation, currentProvisioningParameters, provisioningService, _logger);
                            await _resourceOperationUpdateService.UpdateStatusAsync(currentOperation.Id, CloudResourceOperationState.DONE_SUCCESSFUL, updatedProvisioningState: currentProvisioningResult.CurrentProvisioningState);
                        }
                        else
                        {
                            throw new ProvisioningException("Unknown operation type", CloudResourceOperationState.ABORTED);
                        }

                       
                    }
                    catch (ProvisioningException ex) //Inner loop, ordinary exception is not catched
                    {
                        //Update operation status  

                        _logger.LogError(ex, ProvisioningLogUtil.Operation(currentOperation, "Operation failed"));

                        if (String.IsNullOrWhiteSpace(ex.NewOperationStatus) == false)
                        {
                            await _resourceOperationUpdateService.UpdateStatusAsync(currentOperation.Id, ex.NewOperationStatus);
                        }

                        if (ex.ProceedWithOtherOperations == false)
                        {
                            throw;
                        }  
                    }
                } //foreach

                if (deleteFromQueueAfterCompletion)
                {
                    _logger.LogInformation($"Deleting handling queue message: {queueParentItem.MessageId}");
                    await _workQueue.DeleteMessageAsync(queueParentItem);
                }

                _logger.LogInformation($"Finished handling queue message: {queueParentItem.MessageId}");              

            }
            catch (ProvisioningException ex) //Outer loop catch 1
            {
                if (ex.DeleteFromQueue)
                {
                    await _workQueue.DeleteMessageAsync(queueParentItem);
                }
                else if(ex.PostponeQueueItemFor.HasValue && ex.PostponeQueueItemFor.Value > 0)
                {
                    await _workQueue.IncreaseInvisibilityAsync(queueParentItem, ex.PostponeQueueItemFor.Value);
                } 
            }
            catch (Exception ex) //Outer loop catch
            {
                _logger.LogCritical(ex, $"Error occured while processing message {queueParentItem.MessageId}, message description: {queueParentItem.Description}. See exception info for details ");
            }
        }
    }
}
