using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Provisioning;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Interface;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Service.Azure;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using Sepes.Infrastructure.Util.Provisioning;
using System;
using System.Threading.Tasks;
using Sepes.Infrastructure.Service.Provisioning.Interface;

namespace Sepes.Infrastructure.Service
{
    public class ResourceProvisioningService : IResourceProvisioningService
    {
        readonly ILogger _logger;
        readonly IServiceProvider _serviceProvider;
        readonly IUserService _userService;
        readonly IRequestIdService _requestIdService;
        readonly IProvisioningQueueService _workQueue;
        readonly ICloudResourceReadService _resourceReadService;
        readonly IResourceOperationModelService _resourceOperationModelService;
        readonly ICloudResourceUpdateService _resourceUpdateService;
        readonly ICloudResourceOperationReadService _resourceOperationReadService;
        readonly ICloudResourceOperationUpdateService _resourceOperationUpdateService;

        readonly IRoleProvisioningService _roleProvisioningService;

        readonly ICloudResourceMonitoringService _monitoringService;

        public ResourceProvisioningService(
            ILogger<ResourceProvisioningService> logger,
            IServiceProvider serviceProvider,
            IUserService userService,
            IRequestIdService requestIdService,
            IProvisioningQueueService workQueue,
            ICloudResourceReadService cloudResourceReadService,
            ICloudResourceUpdateService resourceUpdateService,
            IResourceOperationModelService resourceOperationModelService,
            ICloudResourceOperationReadService resourceOperationReadService,
            ICloudResourceOperationUpdateService resourceOperationUpdateService,
            IRoleProvisioningService roleProvisioningService,
            ICloudResourceMonitoringService monitoringService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _requestIdService = requestIdService ?? throw new ArgumentNullException(nameof(requestIdService));
            _workQueue = workQueue ?? throw new ArgumentNullException(nameof(workQueue));

            //Resource services
            _resourceReadService = cloudResourceReadService ?? throw new ArgumentNullException(nameof(cloudResourceReadService));
            _resourceUpdateService = resourceUpdateService ?? throw new ArgumentNullException(nameof(resourceUpdateService));

            //Resource operation services
            _resourceOperationModelService = resourceOperationModelService ?? throw new ArgumentNullException(nameof(resourceOperationModelService)); ;
            _resourceOperationReadService = resourceOperationReadService ?? throw new ArgumentNullException(nameof(resourceOperationReadService));
            _resourceOperationUpdateService = resourceOperationUpdateService ?? throw new ArgumentNullException(nameof(resourceOperationUpdateService));

            _roleProvisioningService = roleProvisioningService ?? throw new ArgumentNullException(nameof(roleProvisioningService));
            _monitoringService = monitoringService ?? throw new ArgumentNullException(nameof(monitoringService));
        }

        public async Task DequeueAndHandleWork()
        {
            var currentUser = await _userService.GetCurrentUserAsync();

            if (!currentUser.Admin)
            {
                throw new ForbiddenException("This action requires Admin");
            }

            var work = await _workQueue.ReceiveMessageAsync();

            while (work != null)
            {
                await HandleWork(work);
                work = await _workQueue.ReceiveMessageAsync();
            }
        }

        public async Task HandleWork(ProvisioningQueueParentDto queueParentItem)
        {
            _logger.LogInformation(ProvisioningLogUtil.HandlingQueueParent(queueParentItem));

            //One per child item in queue item
            CloudResourceOperationDto currentOperation = null;

            //Get's re-used amonong child elements because the operations might share variables
            var currentProvisioningParameters = new ResourceProvisioningParameters();

            ResourceProvisioningResult currentProvisioningResult = null;

            try
            {
                //If more than one child/operation, run basic checks on all operations before starting
                if(queueParentItem.Children.Count > 1)
                {
                    _logger.LogInformation(ProvisioningLogUtil.QueueParentProgress(queueParentItem, "Multiple child item, running pre checks"));

                    foreach (var queueChildItem in queueParentItem.Children)
                    {
                        currentOperation = await _resourceOperationReadService.GetByIdAsync(queueChildItem.ResourceOperationId);
                        OperationCheckUtils.ThrowIfPossiblyInProgress(currentOperation);
                    }
                }               

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

                        await ProvisioningParamaterUtil.PrepareForNewOperation(currentProvisioningParameters, currentOperation, currentProvisioningResult, _resourceReadService);

                        await ProvisioningQueueUtil.IncreaseInvisibleBasedOnResource(currentOperation, queueParentItem, _workQueue);

                        _logger.LogInformation(ProvisioningLogUtil.Operation(currentOperation, "Initial checks passed"));

                        if (CreateAndUpdateUtil.WillBeHandledAsCreateOrUpdate(currentOperation))
                        {
                            _logger.LogInformation(ProvisioningLogUtil.Operation(currentOperation, "Operation is CREATE or UPDATE"));

                            var provisioningService = AzureResourceServiceResolver.GetProvisioningServiceOrThrow(_serviceProvider, currentOperation.Resource.ResourceType);

                            if (await OperationCompletedUtil.HandledAsAllreadyCompletedAsync(currentOperation, _monitoringService))
                            {
                                _logger.LogInformation(ProvisioningLogUtil.Operation(currentOperation, "Operation is allready completed"));
                                currentProvisioningResult = await provisioningService.GetSharedVariables(currentProvisioningParameters);
                                continue;
                            }
                            else
                            {
                                currentOperation = await _resourceOperationUpdateService.SetInProgressAsync(currentOperation.Id, _requestIdService.GetRequestId());
                                currentProvisioningResult = await CreateAndUpdateUtil.HandleCreateOrUpdate(currentOperation, currentProvisioningParameters, provisioningService, _resourceReadService, _resourceUpdateService, _resourceOperationUpdateService, _logger);
                            }

                            currentOperation = await _resourceOperationUpdateService.TouchAsync(currentOperation.Id);

                            await _resourceOperationUpdateService.UpdateStatusAsync(currentOperation.Id,
                                CloudResourceOperationState.DONE_SUCCESSFUL,
                                updatedProvisioningState: currentProvisioningResult.CurrentProvisioningState);
                        }
                        else if (DeleteOperationUtil.WillBeHandledAsDelete(currentOperation))
                        {
                            _logger.LogInformation(ProvisioningLogUtil.Operation(currentOperation, "Operation is DELETE"));
                            var provisioningService = AzureResourceServiceResolver.GetProvisioningServiceOrThrow(_serviceProvider, currentOperation.Resource.ResourceType);
                            currentOperation = await _resourceOperationUpdateService.SetInProgressAsync(currentOperation.Id, _requestIdService.GetRequestId());
                            currentProvisioningResult = await DeleteOperationUtil.HandleDelete(currentOperation, currentProvisioningParameters, provisioningService, _resourceOperationUpdateService, _logger);
                            await _resourceOperationUpdateService.UpdateStatusAsync(currentOperation.Id, CloudResourceOperationState.DONE_SUCCESSFUL, updatedProvisioningState: null);
                        }
                        else if (_roleProvisioningService.CanHandle(currentOperation))
                        {
                            _logger.LogInformation(ProvisioningLogUtil.Operation(currentOperation, "Operation is ENSURE ROLES"));
                            currentOperation = await _resourceOperationUpdateService.SetInProgressAsync(currentOperation.Id, _requestIdService.GetRequestId());

                            await _roleProvisioningService.Handle(currentOperation);

                            await _resourceOperationUpdateService.UpdateStatusAsync(currentOperation.Id, CloudResourceOperationState.DONE_SUCCESSFUL);
                        }
                        else if (EnsureFirewallRulesUtil.CanHandle(currentOperation))
                        {
                            _logger.LogInformation(ProvisioningLogUtil.Operation(currentOperation, "Operation is ENSURE FIREWALL"));
                            var firewallRuleService = AzureResourceServiceResolver.GetFirewallRuleService(_serviceProvider, currentOperation.Resource.ResourceType);
                            currentOperation = await _resourceOperationUpdateService.SetInProgressAsync(currentOperation.Id, _requestIdService.GetRequestId());

                            if (firewallRuleService is IHasFirewallRules)
                            {
                                await EnsureFirewallRulesUtil.Handle(currentOperation,
                                    firewallRuleService,
                                    _resourceReadService,
                                    _resourceOperationUpdateService,
                                    _logger);

                                await _resourceOperationUpdateService.UpdateStatusAsync(currentOperation.Id, CloudResourceOperationState.DONE_SUCCESSFUL);
                            }
                            else
                            {
                                throw new ProvisioningException($"Service {firewallRuleService.GetType().Name} does not support firewall operations", CloudResourceOperationState.ABORTED, deleteFromQueue: true);
                            }
                        }
                        else if (EnsureCorsRulesUtil.CanHandle(currentOperation))
                        {
                            _logger.LogInformation(ProvisioningLogUtil.Operation(currentOperation, "Operation is ENSURE CORS RULES"));
                            var corsRuleService = AzureResourceServiceResolver.GetCorsRuleServiceOrThrow(_serviceProvider, currentOperation.Resource.ResourceType);
                            currentOperation = await _resourceOperationUpdateService.SetInProgressAsync(currentOperation.Id, _requestIdService.GetRequestId());

                            if (corsRuleService is IHasCorsRules)
                            {
                                await EnsureCorsRulesUtil.Handle(currentOperation,
                                    corsRuleService,
                                    _resourceReadService,
                                    _resourceOperationUpdateService,
                                    _logger);

                                await _resourceOperationUpdateService.UpdateStatusAsync(currentOperation.Id, CloudResourceOperationState.DONE_SUCCESSFUL);
                            }
                            else
                            {
                                throw new ProvisioningException($"Service {corsRuleService.GetType().Name} does not support CORS operations", CloudResourceOperationState.ABORTED, deleteFromQueue: true);
                            }
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
                            if (ex.IncludeExceptionInWarningLog)
                            {
                                _logger.LogWarning(ex, ProvisioningLogUtil.Operation(currentOperation, "Operation aborted"));
                            }
                            else
                            {
                                _logger.LogWarning(ProvisioningLogUtil.Operation(currentOperation, $"Operation aborted: {ex.Message}"));
                            }
                        }
                        else
                        {
                            _logger.LogError(ex, ProvisioningLogUtil.Operation(currentOperation, "Operation failed"));
                        }

                        currentOperation = await _resourceOperationUpdateService.SetErrorMessageAsync(currentOperation.Id, ex);

                        if (!String.IsNullOrWhiteSpace(ex.NewOperationStatus))
                        {
                            currentOperation = await _resourceOperationUpdateService.UpdateStatusAsync(currentOperation.Id, ex.NewOperationStatus);
                        }

                        if (!ex.ProceedWithOtherOperations)
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
                    if (!queueParentItem.NextVisibleOn.HasValue)
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

                CloudResourceOperation currentOperation = null;

                foreach (var queueChildItem in queueParentItem.Children)
                {
                    currentOperation = await _resourceOperationModelService.GetForOperationPromotion(queueChildItem.ResourceOperationId);

                    if (currentOperation.DependantOnThisOperation != null && currentOperation.DependantOnThisOperation.Count > 0)
                    {
                        foreach (var curDependantOnThisOp in currentOperation.DependantOnThisOperation)
                        {
                            if (!curDependantOnThisOp.Resource.Deleted)
                            {
                                if (curDependantOnThisOp.Status == CloudResourceOperationState.NEW && String.IsNullOrWhiteSpace(curDependantOnThisOp.BatchId))
                                {
                                    if (!String.IsNullOrWhiteSpace(curDependantOnThisOp.QueueMessageId) && String.IsNullOrWhiteSpace(curDependantOnThisOp.QueueMessagePopReceipt))
                                    {
                                        if (curDependantOnThisOp.QueueMessageVisibleAgainAt.HasValue && curDependantOnThisOp.QueueMessageVisibleAgainAt.Value > DateTime.UtcNow.AddSeconds(15))
                                        {
                                            //Create a new queue item for immediate pickup
                                            await AddNewQueueMessageForOperation(curDependantOnThisOp);

                                            //Delete existing message
                                            await _workQueue.DeleteMessageAsync(curDependantOnThisOp.QueueMessageId, curDependantOnThisOp.QueueMessagePopReceipt);

                                            //Clear stored message details on operation record
                                            await _resourceOperationUpdateService.ClearQueueInformationAsync(curDependantOnThisOp.Id);

                                            movedUpCount++;
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

        async Task AddNewQueueMessageForOperation(CloudResourceOperation operation)
        {
            var queueParentItem = new ProvisioningQueueParentDto
            {
                Description = operation.Description
            };

            queueParentItem.Children.Add(new ProvisioningQueueChildDto() { ResourceOperationId = operation.Id });

            await _workQueue.SendMessageAsync(queueParentItem, visibilityTimeout: TimeSpan.FromSeconds(5));
        }
    }
}
