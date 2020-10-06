using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
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
        readonly ISandboxResourceOperationService _sandboxResourceOperationService;
        readonly IResourceProvisioningQueueService _workQueue;
        readonly IAzureResourceMonitoringService _monitoringService;

        public static readonly string UnitTestPrefix = "unit-test";

        public SandboxResourceProvisioningService(ILogger<SandboxResourceProvisioningService> logger, IServiceProvider serviceProvider, IRequestIdService requestIdService, ISandboxResourceOperationService sandboxResourceOperationService, IResourceProvisioningQueueService workQueue, IAzureResourceMonitoringService monitoringService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _requestIdService = requestIdService;
            _sandboxResourceOperationService = sandboxResourceOperationService ?? throw new ArgumentNullException(nameof(sandboxResourceOperationService));
            _workQueue = workQueue ?? throw new ArgumentNullException(nameof(workQueue));
            _monitoringService = monitoringService;
        }

        public async Task LookForWork()
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
            _logger.LogInformation($"Handling queue message: {queueParentItem.MessageId}");

            //One per child item in queue item
            SandboxResourceOperationDto currentResourceOperation = null;

            //Get's re-used amonong child elements because the operations might share variables
            var currentCrudInput = new CloudResourceCRUDInput();

            CloudResourceCRUDResult currentCrudResult = null;


            foreach (var queueChildItem in queueParentItem.Children)
            {
                try
                {
                    currentResourceOperation = await _sandboxResourceOperationService.GetByIdAsync(queueChildItem.SandboxResourceOperationId);

                    _logger.LogInformation($"{CreateOperationLogMessagePrefix(currentResourceOperation)}: Starting operation");

                    if (currentResourceOperation.Status == CloudResourceOperationState.DONE_SUCCESSFUL )
                    {
                        _logger.LogInformation($"{CreateOperationLogMessagePrefix(currentResourceOperation)}: Allready completed!");
                        continue;
                    }
                    else if (currentResourceOperation.Status == CloudResourceOperationState.FAILED && currentResourceOperation.TryCount > 2)
                    {
                        _logger.LogInformation($"{CreateOperationLogMessagePrefix(currentResourceOperation)}: Retry count exceeded");
                        await HandleRetryCountExceeded(queueParentItem, queueChildItem, currentResourceOperation);
                        //cannot recover from this
                        return;
                    }
                    else if (MightBeInProgressByAnotherThread(currentResourceOperation))
                    {
                        //cannot recover from this
                        throw new Exception($"{CreateOperationLogMessagePrefix(currentResourceOperation)}: Aborting! In danger of picking up work in progress");
                    }

                    currentCrudResult = await HandleCRUD(queueParentItem, queueChildItem, currentResourceOperation, currentCrudInput, currentCrudResult);

                }
                catch (Exception ex)
                {

                    //TODO: HANDLE FAILED OPERATION            

                    //Queue item get's visible again after a while

                    if (currentResourceOperation != null)
                    {
                        await _sandboxResourceOperationService.UpdateStatusAndIncreaseTryCount(currentResourceOperation.Id.Value, CloudResourceOperationState.FAILED);
                    }                  

                    return;
                }

            }


            _logger.LogInformation($"Finished handling queue message: {queueParentItem.MessageId}. Deleting message");

            await _workQueue.DeleteMessageAsync(queueParentItem);
        }

        async Task<SandboxResourceOperationDto> HandleRetryCountExceeded(ProvisioningQueueParentDto queueParentItem, ProvisioningQueueChildDto queueChildItem, SandboxResourceOperationDto currentResourceOperation)
        {
            _logger.LogCritical($"ResourceOperation {queueChildItem.SandboxResourceOperationId}: Operation type:{currentResourceOperation.OperationType} exceeded max retry count: {currentResourceOperation.TryCount}!");
            currentResourceOperation = await _sandboxResourceOperationService.UpdateStatus(currentResourceOperation.Id.Value, CloudResourceOperationState.FAILED);
            await _workQueue.DeleteMessageAsync(queueParentItem);
            return currentResourceOperation;
        }

        async Task<CloudResourceCRUDResult> HandleCRUD(ProvisioningQueueParentDto queueParentItem, ProvisioningQueueChildDto queueChildItem, SandboxResourceOperationDto currentResourceOperation, CloudResourceCRUDInput currentCrudInput, CloudResourceCRUDResult currentCrudResult)
        {

            var resource = currentResourceOperation.Resource;
            var resourceType = currentResourceOperation.Resource.ResourceType;

            //Update operation with request id and "in progress" state

            _logger.LogInformation($"{CreateOperationLogMessagePrefix(currentResourceOperation)}: Setting operation to In Progress");
            currentResourceOperation = await _sandboxResourceOperationService.SetInProgress(currentResourceOperation.Id.Value, _requestIdService.GetRequestId(), CloudResourceOperationState.IN_PROGRESS);

            //TODO: Update queue with relevant timeout + 1 min 



            // TODO: Work out format on messages in Queue. How to decide what actions to take, and what service to use.
            // Possibly implement an ActionResolver...
            // Decide if it would be smart to have a reference to not only the resourceOperation but also the resource itself.

            //Possible actions steps:
            var service = AzureResourceServiceResolver.GetCRUDService(_serviceProvider, resourceType);

            if (service == null)
            {
                throw new NullReferenceException($"ResourceOperation {queueChildItem.SandboxResourceOperationId}: Unable to resolve CRUD service for type {resourceType}!");
            }

            currentCrudInput.ResetButKeepSharedVariables(currentCrudResult != null ? currentCrudResult.NewSharedVariables : null);
            currentCrudInput.Name = resource.ResourceName;
            currentCrudInput.StudyName = resource.StudyName;
            currentCrudInput.SandboxName = resource.SandboxName;
            currentCrudInput.ResourceGrupName = resource.ResourceGroupName;
            currentCrudInput.Region = RegionStringConverter.Convert(resource.Region);
            currentCrudInput.Tags = resource.Tags;

            currentCrudResult = null;

            var increaseQueueItemInvisibilityBy = AzureResourceProivisoningTimeoutResolver.GetTimeoutForOperationInSeconds(resourceType, currentResourceOperation.OperationType);
            await _workQueue.IncreaseInvisibilityAsync(queueParentItem, increaseQueueItemInvisibilityBy);

            if (currentResourceOperation.OperationType == CloudResourceOperationType.CREATE)
            {
                if (AllreadyCompleted(currentResourceOperation))
                {
                    _logger.LogInformation($"{CreateOperationLogMessagePrefix(currentResourceOperation)}: Operation allready completed. Resource should exist. Getting provsioning state");
                    await ThrowIfUnexpectedProvisioningStateAsync(currentResourceOperation);     //cannot recover from this                 
                }
                else
                {
                    _logger.LogInformation($"{CreateOperationLogMessagePrefix(currentResourceOperation)}: Initial checks succeeded. Proceeding with create");
                    currentCrudResult = await service.Create(currentCrudInput);
                }
            }
            else if (currentResourceOperation.OperationType == CloudResourceOperationType.UPDATE)
            {

            }
            else if (currentResourceOperation.OperationType == CloudResourceOperationType.DELETE)
            {

            }

            _logger.LogInformation($"{CreateOperationLogMessagePrefix(currentResourceOperation)}: finished with provisioningState: {currentCrudResult.CurrentProvisioningState}");

            await _sandboxResourceOperationService.UpdateStatus(currentResourceOperation.Id.Value, CloudResourceOperationState.DONE_SUCCESSFUL, currentCrudResult.CurrentProvisioningState);

            return currentCrudResult;
        }

        string CreateOperationLogMessagePrefix(SandboxResourceOperationDto currentResourceOperation)
        {
            return $"{currentResourceOperation.Id} | {currentResourceOperation.OperationType} | {currentResourceOperation.Resource.ResourceType}";
        }

        bool MightBeInProgressByAnotherThread(SandboxResourceOperationDto currentResourceOperation)
        {
            if (currentResourceOperation.Status == CloudResourceOperationState.IN_PROGRESS)
            {
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
                if (currentResourceOperation.OperationType == CloudResourceOperationType.CREATE)
                {

                    if (currentProvisioningState == "Succeeded")
                    {
                        return;
                    }
                }
            }

            throw new Exception($"{CreateOperationLogMessagePrefix(currentResourceOperation)}: Aborting! Comnponent should have been created, but provisioning state is not as expexted: {currentProvisioningState}");
        }

        void DecorateInput(CloudResourceCRUDInput currentCrudInput, SandboxResourceDto resource, CloudResourceCRUDResult currentCrudResult)
        {
            currentCrudInput.ResetButKeepSharedVariables(currentCrudResult != null ? currentCrudResult.NewSharedVariables : null);
            currentCrudInput.Name = resource.ResourceName;
            currentCrudInput.SandboxName = resource.SandboxName;
            currentCrudInput.ResourceGrupName = resource.ResourceGroupName;
            currentCrudInput.Region = RegionStringConverter.Convert(resource.Region);
            currentCrudInput.Tags = resource.Tags;
        }



        //public async Task<SandboxWithCloudResourcesDto> CreateVM(int sandboxId, SandboxWithCloudResourcesDto azureSandbox, Region region, Dictionary<string, string> tags)
        //{
        //    //TODO: How to make this ready for execution after picking up queue message?
        //    _logger.LogInformation($"Creating Virtual Machine for sandbox: {azureSandbox.SandboxName}");
        //    var virtualMachine = await _vmService.Create(region, azureSandbox.ResourceGroupName, azureSandbox.SandboxName, azureSandbox.VNet.Network, azureSandbox.VNet.SandboxSubnetName, "sepesTestAdmin", "sepesRules12345", "Cheap", "windows", "win2019datacenter", tags, azureSandbox.DiagnosticsStorage.ResourceName);
        //    await _sandboxResourceService.Add(sandboxId, azureSandbox.ResourceGroupId, azureSandbox.ResourceGroupName, virtualMachine.Type, virtualMachine.Key, virtualMachine.Name);
        //    return azureSandbox;
        //}



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
