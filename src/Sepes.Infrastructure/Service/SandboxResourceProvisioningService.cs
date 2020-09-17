using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Interface;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class SandboxResourceProvisioningService : ISandboxResourceProvisioningService
    {
        readonly ILogger _logger;
        readonly IServiceProvider _serviceProvider;
        readonly IHasRequestId _requestIdService;
        readonly ISandboxResourceOperationService _sandboxResourceOperationService;
        readonly IResourceProvisioningQueueService _workQueue;

        public static readonly string UnitTestPrefix = "unit-test";

        public SandboxResourceProvisioningService(ILogger<SandboxResourceProvisioningService> logger, IServiceProvider serviceProvider, IHasRequestId requestIdService, ISandboxResourceOperationService sandboxResourceOperationService, IResourceProvisioningQueueService workQueue
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _requestIdService = requestIdService;
            _sandboxResourceOperationService = sandboxResourceOperationService ?? throw new ArgumentNullException(nameof(sandboxResourceOperationService));
            _workQueue = workQueue ?? throw new ArgumentNullException(nameof(workQueue));
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
            //One per child item in queue item
            SandboxResourceOperationDto currentResourceOperation = null;

            //Get's re-used amonong child elements because the operations might share variables
            CloudResourceCRUDInput currentCrudInput = new CloudResourceCRUDInput();
            CloudResourceCRUDResult currentCrudResult = null;
            try
            {
                foreach (var queueChildItem in queueParentItem.Children)
                {

                    currentResourceOperation = await _sandboxResourceOperationService.GetByIdAsync(queueChildItem.SandboxResourceOperationId);

                    if (currentResourceOperation.TryCount > 2)
                    {
                        await HandleRetryCountExceeded(queueParentItem, queueChildItem, currentResourceOperation);
                        break;
                    }
                    else if (currentResourceOperation.Status == CloudResourceOperationState.DONE_SUCCESSFUL)
                    {
                        //This particular resource has been setup
                        //TODO: Do som validation here?
                    }
                    else
                    {
                        //If item allready in progress
                        if (currentResourceOperation.Status == CloudResourceOperationState.IN_PROGRESS)
                        {
                            //TODO: What to do here?
                        }

                        currentCrudResult = await HandleCRUD(queueParentItem, queueChildItem, currentResourceOperation, currentCrudInput, currentCrudResult);
                    }

                }

            }
            catch (Exception ex)
            {
                //TODO: HANDLE FAILED OPERATION            
               
                //Queue item get's visible again after a while
                await _sandboxResourceOperationService.UpdateStatusAndIncreaseTryCount(currentResourceOperation.Id.Value, CloudResourceOperationState.FAILED);
                return;
            }


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
            currentResourceOperation = await _sandboxResourceOperationService.SetInProgress(currentResourceOperation.Id.Value, _requestIdService.RequestId(), CloudResourceOperationState.IN_PROGRESS);

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
            currentCrudInput.SandboxName = resource.SandboxName;
            currentCrudInput.ResourceGrupName = resource.ResourceGroupName;
            currentCrudInput.Region = RegionStringConverter.Convert(resource.Region);
            currentCrudInput.Tags = resource.Tags;

            currentCrudResult = null;

            var increaseQueueItemInvisibilityBy = AzureResourceProivisoningTimeoutResolver.GetTimeoutForOperationInSeconds(resourceType, currentResourceOperation.OperationType);
            await _workQueue.IncreaseInvisibilityAsync(queueParentItem, increaseQueueItemInvisibilityBy);

            if (currentResourceOperation.OperationType == CloudResourceOperationType.CREATE)
            {
                currentCrudResult = await service.Create(currentCrudInput);
            }
            else if (currentResourceOperation.OperationType == CloudResourceOperationType.UPDATE)
            {

            }
            else if (currentResourceOperation.OperationType == CloudResourceOperationType.DELETE)
            {

            }

            _logger.LogInformation($"ResourceOperation {currentResourceOperation.Id}: Operation type:{currentResourceOperation.OperationType} finished with provisioningState: {currentCrudResult.CurrentProvisioningState}");

            await _sandboxResourceOperationService.UpdateStatus(currentResourceOperation.Id.Value, CloudResourceOperationState.DONE_SUCCESSFUL, currentResourceOperation.Resource.ProvisioningState); 
            
            return currentCrudResult;
        }

        public async Task<SandboxWithCloudResourcesDto> CreateVirtualNetwork(int sandboxId, SandboxWithCloudResourcesDto azureSandbox, Region region, Dictionary<string, string> tags)
        {
            throw new NotImplementedException();
            //_logger.LogInformation($"Creating VNet for sandbox: {azureSandbox.SandboxName}");
            //// Create resource-entry
            //var sandboxResourceEntry = await _sandboxResourceService.Add(sandboxId, azureSandbox.ResourceGroupId, azureSandbox.ResourceGroupName, "VirtualNetwork", "Not Yet Available", AzureResourceNameUtil.VNet(azureSandbox.StudyName, azureSandbox.SandboxName));

            //// Create resource-operation-entry
            //var sandboxOperation = CreateInitialResourceOperation("Create Virtual Network.");
            //var operationEntry = await _sandboxResourceOperationService.Add((int)sandboxResourceEntry.Id, sandboxOperation);

            //// Create actual VNET in Azure
            ////TODO: Add to queue instead
            //azureSandbox.VNet = await _vNetService.CreateAsync(region, azureSandbox.ResourceGroupName, azureSandbox.StudyName, azureSandbox.SandboxName, tags);

            //// Update Entries
            //_ = await _sandboxResourceService.Update((int)sandboxResourceEntry.Id, azureSandbox.VNet.Network);
            //_ = await _sandboxResourceOperationService.UpdateStatus((int)operationEntry.Id, azureSandbox.VNet.Network.Inner.ProvisioningState.ToString());

            //_logger.LogInformation($"Applying NSG to subnet for sandbox: {azureSandbox.SandboxName}");

            ////Applying nsg to subnet
            //sandboxOperation = CreateInitialResourceOperation($"Apply Network Security Group: {azureSandbox.NetworkSecurityGroup.Name} to Subnet.");
            //operationEntry = await _sandboxResourceOperationService.Add((int)sandboxResourceEntry.Id, sandboxOperation);
            //await _vNetService.ApplySecurityGroup(azureSandbox.ResourceGroupName, azureSandbox.NetworkSecurityGroup.Name, azureSandbox.VNet.SandboxSubnetName, azureSandbox.VNet.Network.Name);
            //_ = await _sandboxResourceOperationService.UpdateStatus((int)operationEntry.Id, "Succeeded");
            //return azureSandbox;
        }

        //public async Task<SandboxWithCloudResourcesDto> CreateBastion(int sandboxId, SandboxWithCloudResourcesDto azureSandbox, Region region, Dictionary<string, string> tags)
        //{
        //    //TODO: How to make this ready for execution after picking up queue message?
        //    _logger.LogInformation($"Creating Bastion for sandbox: {azureSandbox.SandboxName}");
        //    var bastion = await _bastionService.Create(region, azureSandbox.ResourceGroupName, azureSandbox.StudyName, azureSandbox.SandboxName, azureSandbox.VNet.BastionSubnetId, tags);
        //    await _sandboxResourceService.Add(sandboxId, azureSandbox.ResourceGroupId, azureSandbox.ResourceGroupName, bastion);
        //    return azureSandbox;
        //}

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
