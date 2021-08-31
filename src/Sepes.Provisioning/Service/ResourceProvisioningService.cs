using Microsoft.Extensions.Configuration;
using Sepes.Azure.Service.Interface;
using Sepes.Azure.Util;
using Sepes.Common.Constants;
using Sepes.Common.Constants.CloudResource;
using Sepes.Common.Dto;
using Sepes.Common.Dto.Provisioning;
using Sepes.Common.Dto.Sandbox;
using Sepes.Common.Exceptions;
using Sepes.Common.Interface;
using Sepes.Common.Util;
using Sepes.Common.Util.Provisioning;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using Sepes.Provisioning.Service.Interface;
using System;
using System.Threading.Tasks;

namespace Sepes.Provisioning.Service
{
    public class ResourceProvisioningService : IResourceProvisioningService
    {
        readonly IConfiguration _configuration;
        readonly IServiceProvider _serviceProvider;
        readonly IUserService _userService;
        readonly IRequestIdService _requestIdService;
        readonly IProvisioningQueueService _provisioningQueueService;
        readonly ICloudResourceReadService _resourceReadService;
        readonly IResourceOperationModelService _resourceOperationModelService;
        readonly ICloudResourceOperationReadService _resourceOperationReadService;
        readonly ICloudResourceOperationUpdateService _resourceOperationUpdateService;

        readonly IProvisioningLogService _provisioningLogService;
        readonly IOperationCheckService _operationCheckService;
        readonly IOperationCompletedService _operationCompletedService;
        readonly ICreateAndUpdateService _createAndUpdateService;
        readonly IRoleProvisioningService _roleProvisioningService;
        readonly ICorsRuleProvisioningService _corsRuleProvisioningService;
        readonly ITagProvisioningService _tagProvisioningService;
        readonly IFirewallService _firewallService;

        readonly IDeleteOperationService _deleteOperationService;

        public ResourceProvisioningService(
            IConfiguration configuration,
            IServiceProvider serviceProvider,
            IUserService userService,
            IRequestIdService requestIdService,
            IProvisioningQueueService provisioningQueueService,
            ICloudResourceReadService cloudResourceReadService,
            IResourceOperationModelService resourceOperationModelService,
            ICloudResourceOperationReadService resourceOperationReadService,
            ICloudResourceOperationUpdateService resourceOperationUpdateService,
            IProvisioningLogService provisioningLogService,
            IOperationCheckService operationCheckService,
            IOperationCompletedService operationCompletedService,
            ICreateAndUpdateService createAndUpdateService,
            IDeleteOperationService deleteOperationService,
            IRoleProvisioningService roleProvisioningService,
            ITagProvisioningService tagProvisioningService,
            ICorsRuleProvisioningService corsRuleProvisioningService,
            IFirewallService firewallService)
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _requestIdService = requestIdService ?? throw new ArgumentNullException(nameof(requestIdService));
            _provisioningQueueService = provisioningQueueService ?? throw new ArgumentNullException(nameof(provisioningQueueService));

            //Resource services
            _resourceReadService = cloudResourceReadService ?? throw new ArgumentNullException(nameof(cloudResourceReadService));

            //Resource operation services
            _resourceOperationModelService = resourceOperationModelService ?? throw new ArgumentNullException(nameof(resourceOperationModelService)); ;
            _resourceOperationReadService = resourceOperationReadService ?? throw new ArgumentNullException(nameof(resourceOperationReadService));
            _resourceOperationUpdateService = resourceOperationUpdateService ?? throw new ArgumentNullException(nameof(resourceOperationUpdateService));

            //Provisioning services
            _provisioningLogService = provisioningLogService ?? throw new ArgumentNullException(nameof(provisioningLogService));
            _operationCheckService = operationCheckService ?? throw new ArgumentNullException(nameof(operationCheckService));
            _operationCompletedService = operationCompletedService ?? throw new ArgumentNullException(nameof(operationCompletedService));
            _createAndUpdateService = createAndUpdateService ?? throw new ArgumentNullException(nameof(createAndUpdateService));
            _deleteOperationService  = deleteOperationService ?? throw new ArgumentNullException(nameof(deleteOperationService));
            _roleProvisioningService = roleProvisioningService ?? throw new ArgumentNullException(nameof(roleProvisioningService));
            _corsRuleProvisioningService = corsRuleProvisioningService ?? throw new ArgumentNullException(nameof(corsRuleProvisioningService));
            _tagProvisioningService = tagProvisioningService ?? throw new ArgumentNullException(nameof(tagProvisioningService));
            _firewallService = firewallService ?? throw new ArgumentNullException(nameof(firewallService));
        }

        public async Task DequeueAndHandleWork()
        {
            var currentUser = await _userService.GetCurrentUserAsync();

            if (!currentUser.Admin)
            {
                var isIntegrationTest = ConfigUtil.GetBoolConfig(_configuration, ConfigConstants.IS_INTEGRATION_TEST);

                if (!isIntegrationTest)
                {
                    throw new ForbiddenException("This action requires Admin");
                }                
            }

            var work = await _provisioningQueueService.ReceiveMessageAsync();

            while (work != null)
            {
                await HandleWork(work);
                work = await _provisioningQueueService.ReceiveMessageAsync();
            }
        }

        public async Task HandleWork(ProvisioningQueueParentDto queueParentItem)
        {
            _provisioningLogService.HandlingQueueParent(queueParentItem);

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
                    _provisioningLogService.QueueParentProgressInformation(queueParentItem,
                        "Multiple child item, running pre checks");

                    foreach (var queueChildItem in queueParentItem.Children)
                    {
                        currentOperation = await _resourceOperationReadService.GetByIdAsync(queueChildItem.ResourceOperationId);
                        _operationCheckService.ThrowIfPossiblyInProgress(currentOperation);
                    }
                }               

                foreach (var queueChildItem in queueParentItem.Children)
                {
                    try
                    {
                        currentOperation = await _resourceOperationReadService.GetByIdAsync(queueChildItem.ResourceOperationId);

                        _provisioningLogService.OperationInformation(queueParentItem, currentOperation, "Starting operation");

                        _operationCheckService.ThrowIfTryCountExceededOrAborted(currentOperation);

                        _operationCheckService.ThrowIfResourceIsDeletedAndOperationIsNotADelete(currentOperation);

                        _operationCheckService.ThrowIfPossiblyInProgress(currentOperation);

                        await _operationCheckService.ThrowIfDependentOnUnfinishedOperationAsync(currentOperation, queueParentItem);

                        string networkSecurityGroupName = null;

                        //Only relevant for Sandbox Resource Creation
                        if (currentOperation.Resource.SandboxId.HasValue)
                        {
                            var nsg = CloudResourceUtil.GetSibilingResource(await _resourceReadService.GetByIdNoAccessCheckAsync(currentOperation.Resource.Id), AzureResourceType.NetworkSecurityGroup);
                            networkSecurityGroupName = nsg?.ResourceName;
                        }                     

                        ProvisioningParamaterUtil.PrepareForNewOperation(currentProvisioningParameters, currentOperation, currentProvisioningResult, networkSecurityGroupName);

                        await _provisioningQueueService.IncreaseInvisibleBasedOnResource(currentOperation, queueParentItem);

                        _provisioningLogService.OperationInformation(queueParentItem, currentOperation, "Initial checks passed");

                        if (_createAndUpdateService.CanHandle(currentOperation))
                        {
                            _provisioningLogService.OperationInformation(queueParentItem, currentOperation, "Operation is CREATE or UPDATE");

                            var provisioningService = AzureResourceServiceResolver.GetProvisioningServiceOrThrow(_serviceProvider, currentOperation.Resource.ResourceType);

                            if (await _operationCompletedService.HandledAsAllreadyCompletedAsync(currentOperation))
                            {
                                _provisioningLogService.OperationInformation(queueParentItem, currentOperation, "Operation is allready completed");
                                currentProvisioningResult = await provisioningService.GetSharedVariables(currentProvisioningParameters);
                                continue;
                            }
                            else
                            {
                                currentOperation = await _resourceOperationUpdateService.SetInProgressAsync(currentOperation.Id, _requestIdService.GetRequestId());
                                currentProvisioningResult = await _createAndUpdateService.Handle(queueParentItem, currentOperation, currentProvisioningParameters, provisioningService);
                            }

                            currentOperation = await _resourceOperationUpdateService.TouchAsync(currentOperation.Id);

                            await _resourceOperationUpdateService.UpdateStatusAsync(currentOperation.Id,
                                CloudResourceOperationState.DONE_SUCCESSFUL,
                                updatedProvisioningState: currentProvisioningResult.CurrentProvisioningState);
                        }
                        else if (_deleteOperationService.CanHandle(currentOperation))
                        {
                            _provisioningLogService.OperationInformation(queueParentItem, currentOperation, "Operation is DELETE");
                            var provisioningService = AzureResourceServiceResolver.GetProvisioningServiceOrThrow(_serviceProvider, currentOperation.Resource.ResourceType);
                            currentOperation = await _resourceOperationUpdateService.SetInProgressAsync(currentOperation.Id, _requestIdService.GetRequestId());
                            currentProvisioningResult = await _deleteOperationService.Handle(queueParentItem, currentOperation, currentProvisioningParameters, provisioningService);
                            await _resourceOperationUpdateService.UpdateStatusAsync(currentOperation.Id, CloudResourceOperationState.DONE_SUCCESSFUL, updatedProvisioningState: null);
                        }
                        else if (_roleProvisioningService.CanHandle(currentOperation))
                        {
                            _provisioningLogService.OperationInformation(queueParentItem, currentOperation, "Operation is ENSURE ROLES");
                            currentOperation = await _resourceOperationUpdateService.SetInProgressAsync(currentOperation.Id, _requestIdService.GetRequestId());

                            await _roleProvisioningService.Handle(queueParentItem, currentOperation);

                            await _resourceOperationUpdateService.UpdateStatusAsync(currentOperation.Id, CloudResourceOperationState.DONE_SUCCESSFUL);
                        }
                        else if (_firewallService.CanHandle(currentOperation))
                        {
                            _provisioningLogService.OperationInformation(queueParentItem, currentOperation, "Operation is ENSURE FIREWALL");
                            var firewallRuleService = AzureResourceServiceResolver.GetFirewallRuleService(_serviceProvider, currentOperation.Resource.ResourceType);
                            currentOperation = await _resourceOperationUpdateService.SetInProgressAsync(currentOperation.Id, _requestIdService.GetRequestId());

                            if (firewallRuleService is IHasFirewallRules)
                            {
                                await _firewallService.Handle(queueParentItem, currentOperation,
                                    firewallRuleService
                                );

                                await _resourceOperationUpdateService.UpdateStatusAsync(currentOperation.Id, CloudResourceOperationState.DONE_SUCCESSFUL);
                            }
                            else
                            {
                                throw new ProvisioningException($"Service {firewallRuleService.GetType().Name} does not support firewall operations", CloudResourceOperationState.ABORTED, deleteFromQueue: true);
                            }
                        }
                        else if (_corsRuleProvisioningService.CanHandle(currentOperation))
                        {
                            _provisioningLogService.OperationInformation(queueParentItem, currentOperation, "Operation is ENSURE CORS RULES");
                            var corsRuleService = AzureResourceServiceResolver.GetCorsRuleServiceOrThrow(_serviceProvider, currentOperation.Resource.ResourceType);
                            currentOperation = await _resourceOperationUpdateService.SetInProgressAsync(currentOperation.Id, _requestIdService.GetRequestId());

                            if (corsRuleService is IHasCorsRules)
                            {
                                await _corsRuleProvisioningService.Handle(queueParentItem, currentOperation, corsRuleService);

                                await _resourceOperationUpdateService.UpdateStatusAsync(currentOperation.Id, CloudResourceOperationState.DONE_SUCCESSFUL);
                            }
                            else
                            {
                                throw new ProvisioningException($"Service {corsRuleService.GetType().Name} does not support CORS operations", CloudResourceOperationState.ABORTED, deleteFromQueue: true);
                            }
                        }
                        else if (_tagProvisioningService.CanHandle(currentOperation))
                        {
                            _provisioningLogService.OperationInformation(queueParentItem, currentOperation, "Operation is ENSURE TAGS");
                            var tagServiceForResource = AzureResourceServiceResolver.GetServiceWithTags(_serviceProvider, currentOperation.Resource.ResourceType);
                            currentOperation = await _resourceOperationUpdateService.SetInProgressAsync(currentOperation.Id, _requestIdService.GetRequestId());

                            if (tagServiceForResource is IServiceForTaggedResource)
                            {
                                await _tagProvisioningService.Handle(queueParentItem, currentOperation, tagServiceForResource);
                                await _resourceOperationUpdateService.UpdateStatusAsync(currentOperation.Id, CloudResourceOperationState.DONE_SUCCESSFUL);
                            }
                            else
                            {
                                throw new ProvisioningException($"Service {tagServiceForResource.GetType().Name} does not support tag update operations", CloudResourceOperationState.ABORTED, deleteFromQueue: true);
                            }
                        }
                        else
                        {
                            throw new ProvisioningException("Unknown operation type", CloudResourceOperationState.ABORTED);
                        }

                        _provisioningLogService.OperationInformation(queueParentItem, currentOperation, "Successfully handeled operation");
                    }
                    catch (ProvisioningException ex) //Inner loop, ordinary exception is not catched
                    {
                        if (ex.LogAsWarning)
                        {
                            if (ex.IncludeExceptionInWarningLog)
                            {
                                _provisioningLogService.OperationWarning(queueParentItem, currentOperation, "Operation aborted", ex);
                            }
                            else
                            {
                                _provisioningLogService.OperationWarning(queueParentItem, currentOperation, $"Operation aborted: {ex.Message}");
                            }
                        }
                        else
                        {
                            _provisioningLogService.OperationError(queueParentItem, ex, currentOperation, "Operation failed");
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

                _provisioningLogService.QueueParentProgressInformation(queueParentItem, "Done");

                await _provisioningQueueService.DeleteMessageAsync(queueParentItem);
                await MoveUpAnyDependentOperations(queueParentItem);

            }
            catch (ProvisioningException ex) //Outer loop catch 1
            {
                if (ex.DeleteFromQueue)
                {
                    _provisioningLogService.QueueParentProgressWarning(queueParentItem, "Deleting message due to exception");
                    await _provisioningQueueService.DeleteMessageAsync(queueParentItem);
                }
                else if (ex.PostponeQueueItemFor.HasValue && ex.PostponeQueueItemFor.Value > 0)
                {
                    if (currentOperation.TryCount < currentOperation.MaxTryCount)
                    {
                        if (queueParentItem.DequeueCount == 5)
                        {
                            _provisioningLogService.QueueParentProgressWarning(queueParentItem, "Re-queuing message after exception");
                         
                            await _provisioningQueueService.ReQueueMessageAsync(queueParentItem, ex.PostponeQueueItemFor.Value);
                        }
                        else
                        {
                            _provisioningLogService.QueueParentProgressWarning(queueParentItem, "Increasing invisibility of message after exception");
                            await _provisioningQueueService.IncreaseInvisibilityAsync(queueParentItem, ex.PostponeQueueItemFor.Value);
                        }
                    }
                }

                if (ex.StoreQueueInfoOnOperation)
                {
                    if (!queueParentItem.NextVisibleOn.HasValue)
                    {
                        _provisioningLogService.QueueParentProgressError(queueParentItem, "Could not store queue info on operation, no next visible time exist");
                    }
                    else
                    {
                        currentOperation = await _resourceOperationUpdateService.SetQueueInformationAsync(currentOperation.Id, queueParentItem.MessageId, queueParentItem.PopReceipt, queueParentItem.NextVisibleOn.Value);
                    }
                }
            }
            catch (Exception ex) //Outer loop catch 2
            {
                _provisioningLogService.QueueParentProgressError(queueParentItem, "Unhandled exception occured", ex);
                await _provisioningQueueService.DeleteMessageAsync(queueParentItem);
            }
        }

        public async Task MoveUpAnyDependentOperations(ProvisioningQueueParentDto queueParentItem)
        {
            _provisioningLogService.QueueParentProgressInformation(queueParentItem, "Moving up relevant dependent operations");

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
                            if (!curDependantOnThisOp.Resource.Deleted &&
                                curDependantOnThisOp.Status == CloudResourceOperationState.NEW && String.IsNullOrWhiteSpace(curDependantOnThisOp.BatchId) &&
                                !String.IsNullOrWhiteSpace(curDependantOnThisOp.QueueMessageId) && String.IsNullOrWhiteSpace(curDependantOnThisOp.QueueMessagePopReceipt) &&
                                curDependantOnThisOp.QueueMessageVisibleAgainAt.HasValue && curDependantOnThisOp.QueueMessageVisibleAgainAt.Value > DateTime.UtcNow.AddSeconds(15)
                                )
                            {
                                //Create a new queue item for immediate pickup
                                await _provisioningQueueService.AddNewQueueMessageForOperation(curDependantOnThisOp);

                                //Delete existing message
                                await _provisioningQueueService.DeleteMessageAsync(curDependantOnThisOp.QueueMessageId, curDependantOnThisOp.QueueMessagePopReceipt);

                                //Clear stored message details on operation record
                                await _resourceOperationUpdateService.ClearQueueInformationAsync(curDependantOnThisOp.Id);

                                movedUpCount++;
                            }
                        }
                    }
                }

                _provisioningLogService.QueueParentProgressInformation(queueParentItem, $"Done moving up relevant dependent operations. Moved up {movedUpCount} operations");
                
            }
            catch (Exception ex)
            {
                _provisioningLogService.QueueParentProgressError(queueParentItem, $"Failed when moving up relevant dependent operations. ", ex);
            }
        }       
    }
}
