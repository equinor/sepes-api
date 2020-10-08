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
        readonly ISandboxResourceService _sandboxResourceService;
        readonly ISandboxResourceOperationService _sandboxResourceOperationService;
        readonly IResourceProvisioningQueueService _workQueue;
        readonly IAzureResourceMonitoringService _monitoringService;

        public static readonly string UnitTestPrefix = "unit-test";

        public SandboxResourceProvisioningService(ILogger<SandboxResourceProvisioningService> logger, IServiceProvider serviceProvider, IRequestIdService requestIdService, ISandboxResourceService sandboxResourceService, ISandboxResourceOperationService sandboxResourceOperationService, IResourceProvisioningQueueService workQueue, IAzureResourceMonitoringService monitoringService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            _requestIdService = requestIdService;
            _sandboxResourceService = sandboxResourceService ?? throw new ArgumentNullException(nameof(sandboxResourceService));
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

        async Task<bool> HasUnfinishedPreceedingOperations(SandboxResourceOperationDto queueParentItem)
        {
            //Check for created, not finished/inprogress and batchid!=
            return await _sandboxResourceOperationService.ExistsPreceedingUnfinishedOperations(queueParentItem);
        }

        public async Task HandleQueueItem(ProvisioningQueueParentDto queueParentItem)
        {
            _logger.LogInformation($"Handling queue message: {queueParentItem.MessageId}. Descr: {queueParentItem.Description}");

            //One per child item in queue item
            SandboxResourceOperationDto currentResourceOperation = null;

            //Get's re-used amonong child elements because the operations might share variables
            var currentCrudInput = new CloudResourceCRUDInput();

            CloudResourceCRUDResult currentCrudResult = null;
            try
            {
                foreach (var queueChildItem in queueParentItem.Children)
                {
                    try
                    {
                        currentResourceOperation = await _sandboxResourceOperationService.GetByIdAsync(queueChildItem.SandboxResourceOperationId);

                        _logger.LogInformation($"{CreateOperationLogMessagePrefix(currentResourceOperation)} - Starting operation");

                        if (currentResourceOperation.OperationType != CloudResourceOperationType.DELETE && currentResourceOperation.Resource.Deleted.HasValue)
                        {
                            //cannot recover from this
                            await _workQueue.DeleteMessageAsync(queueParentItem);
                            throw new Exception($"{CreateOperationLogMessagePrefix(currentResourceOperation)} - Resource is marked for deletion in database, Aborting!"); 
                        }
                        else if (currentResourceOperation.Status == CloudResourceOperationState.FAILED && currentResourceOperation.TryCount > 2)
                        {
                            await HandleRetryCountExceeded(queueParentItem, queueChildItem, currentResourceOperation);

                            //cannot recover from this
                            throw new Exception($"{CreateOperationLogMessagePrefix(currentResourceOperation)} - Max retry count exceeded: {currentResourceOperation.TryCount}, Aborting!");
                        }
                        else if (MightBeInProgressByAnotherThread(currentResourceOperation))
                        {
                            //cannot recover from this
                            throw new Exception($"{CreateOperationLogMessagePrefix(currentResourceOperation)} - In danger of picking up work in progress, Aborting!");
                        }
                        else if (currentResourceOperation.Status == CloudResourceOperationState.DONE_SUCCESSFUL)
                        {
                            _logger.LogInformation($"{CreateOperationLogMessagePrefix(currentResourceOperation)} - Allready completed, Aborting!");
                            continue;
                        }

                        currentCrudResult = await HandleCRUD(queueParentItem, queueChildItem, currentResourceOperation, currentCrudInput, currentCrudResult);
                    }
                    catch (Exception)
                    {
                        if (currentResourceOperation != null)
                        {
                            await _sandboxResourceOperationService.UpdateStatusAndIncreaseTryCount(currentResourceOperation.Id.Value, CloudResourceOperationState.FAILED);
                        }

                        throw;
                    }
                }

                _logger.LogInformation($"Finished handling queue message: {queueParentItem.MessageId}. Deleting message");

                await _workQueue.DeleteMessageAsync(queueParentItem);

            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"Error occured while processing message {queueParentItem.MessageId}");
            }
        }

        async Task<SandboxResourceOperationDto> HandleRetryCountExceeded(ProvisioningQueueParentDto queueParentItem, ProvisioningQueueChildDto queueChildItem, SandboxResourceOperationDto currentResourceOperation)
        {
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
                    _logger.LogInformation($"{CreateOperationLogMessagePrefix(currentResourceOperation)} - Operation allready completed. Resource should exist. Getting provsioning state");
                    await ThrowIfUnexpectedProvisioningStateAsync(currentResourceOperation);     //cannot recover from this                 
                }
                else
                {
                    _logger.LogInformation($"{CreateOperationLogMessagePrefix(currentResourceOperation)} - Initial checks succeeded. Proceeding with create");
                    currentCrudResult = await service.Create(currentCrudInput);
                    await _sandboxResourceService.UpdateMissingDetailsAfterCreation(currentResourceOperation.Resource.Id.Value, currentCrudResult.IdInTargetSystem, currentCrudResult.NameInTargetSystem);
                }
            }
            else if (currentResourceOperation.OperationType == CloudResourceOperationType.UPDATE)
            {

            }
            else if (currentResourceOperation.OperationType == CloudResourceOperationType.DELETE)
            {
                if (currentResourceOperation.Resource.ResourceType == AzureResourceType.ResourceGroup)
                {
                    currentCrudResult = await service.Delete(currentCrudInput);
                }
                else
                {
                    _logger.LogCritical($"{CreateOperationLogMessagePrefix(currentResourceOperation)} - Attempted to delete resource with type {currentCrudResult.Resource.Type}. Only deleting resource groups are supprorted.");
                    throw new ArgumentException($"ResourceOperation {queueChildItem.SandboxResourceOperationId}: Unable to resolve CRUD service for type {resourceType}!");
                }
            }

            _logger.LogInformation($"{CreateOperationLogMessagePrefix(currentResourceOperation)} - finished with provisioningState: {currentCrudResult.CurrentProvisioningState}");

            await _sandboxResourceOperationService.UpdateStatus(currentResourceOperation.Id.Value, CloudResourceOperationState.DONE_SUCCESSFUL, currentCrudResult.CurrentProvisioningState);

            return currentCrudResult;
        }

        string CreateOperationLogMessagePrefix(SandboxResourceOperationDto currentResourceOperation)
        {
            return $"{currentResourceOperation.Id} | {currentResourceOperation.Resource.ResourceType} | {currentResourceOperation.OperationType}";
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

            throw new Exception($"{CreateOperationLogMessagePrefix(currentResourceOperation)} - Aborting! Comnponent should have been created, but provisioning state is not as expexted: {currentProvisioningState}");
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
