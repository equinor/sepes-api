using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Interface;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class SandboxResourceProvisioningService : ISandboxResourceProvisioningService
    {
        readonly ILogger _logger;
        readonly IHasRequestId _requestIdService;
        readonly ISandboxResourceService _sandboxResourceService;
        readonly IAzureResourceGroupService _resourceGroupService;
        readonly IAzureVNetService _vNetService;
        readonly IAzureNwSecurityGroupService _nsgService;
        readonly IAzureStorageAccountService _storageService;
        readonly IAzureBastionService _bastionService;
        readonly IAzureVMService _vmService;
        readonly IAzureQueueService _azureQueueService;
       

        public static readonly string UnitTestPrefix = "unit-test";

        public SandboxResourceProvisioningService(ILogger<SandboxResourceProvisioningService> logger, IHasRequestId requestIdService, ISandboxResourceService sandboxResourceService, IAzureResourceGroupService resourceGroupService
            , IAzureVNetService vNetService, IAzureBastionService bastionService, IAzureNwSecurityGroupService nsgService
            , IAzureVMService vmService, IAzureStorageAccountService storageService, IAzureQueueService azureQueueService
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _requestIdService = requestIdService;
            _sandboxResourceService = sandboxResourceService ?? throw new ArgumentNullException(nameof(sandboxResourceService));
            _resourceGroupService = resourceGroupService ?? throw new ArgumentNullException(nameof(resourceGroupService));
            _vNetService = vNetService ?? throw new ArgumentNullException(nameof(vNetService));
            _bastionService = bastionService ?? throw new ArgumentNullException(nameof(bastionService));
            _nsgService = nsgService ?? throw new ArgumentNullException(nameof(nsgService));
            _vmService = vmService ?? throw new ArgumentNullException(nameof(vmService));
            _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
            _azureQueueService = azureQueueService ?? throw new ArgumentNullException(nameof(azureQueueService));
            
        }     

        public async Task DoWork()
        {
            // This method should take orders from queue, check for dependencies and execute.
            var queueMessage = await _azureQueueService.RecieveMessage();
            var workOrder = _azureQueueService.MessageToSandboxResourceOperation(queueMessage);
            if(workOrder.DependsOn > 0)
            {
                // Skip queue item
                // Possibly after checking if dependent resources already exists.
                return;
            }
            else // No dependencies => Execute workOrder
            {
                if (workOrder.TryCount > 2)
                {
                    // Report that order has failed too many times.
                    _logger.LogCritical($"Workorder {workOrder.Id} : {workOrder.Description} exceeded max retry count!");
                    return;
                }
                else
                {
                    // TODO: Work out format on messages in Queue. How to decide what actions to take, and what service to use.
                    // Possibly implement an ActionResolver...
                    // Decide if it would be smart to have a reference to not only the resourceOperation but also the resource itself.

                    //Possible actions steps:
                    // var service = resolveService(workOrder)
                    // var action = resolveAction(workOrder)
                    // var result = await service.doAction(action)
                    // await sandboxResourceOperationService.RemoveDependencies(workOrder);
                    // await sandboxResourceOperationService.UpdateProvisioningState(workOrder);
                    // _logger.LogInformation($"WorkOrder {workOrder.Id} : {workOrder.Description} finished with provisioningState: {workOrder.ProvisioningState}");
                }
            }
            // When completed should report with provisioning state and mark this in SandboxResourceOperation-table.
            throw new NotImplementedException();
        }           


        public async Task<SandboxWithCloudResourcesDto> CreateDiagStorageAccount(int sandboxId, SandboxWithCloudResourcesDto azureSandbox, Region region, Dictionary<string, string> tags)
        {
            throw new NotImplementedException();
            //_logger.LogInformation($"Creating diagnostics storage account for sandbox: {azureSandbox.SandboxName}");
            //// Create resource-entry
            //var sandboxResourceEntry = await _sandboxResourceService.Add(sandboxId, azureSandbox.ResourceGroupId, azureSandbox.ResourceGroupName, "StorageAccount", "Not Yet Available", "Not Yet Available");

            //// Create resource-operation-entry
            //var sandboxOperation = CreateInitialResourceOperation("Create Diagnostic Storage Account.");
            //var operationEntry = await _sandboxResourceOperationService.Add((int)sandboxResourceEntry.Id, sandboxOperation);

            //// Create storage account for diagnostics logging of vms.
            ////TODO: Add to queue instead
            //azureSandbox.DiagnosticsStorage = await _storageService.CreateDiagnosticsStorageAccount(region, azureSandbox.SandboxName, azureSandbox.ResourceGroupName, tags);

            //// Update entries
            //_ = await _sandboxResourceService.Update((int)sandboxResourceEntry.Id, azureSandbox.DiagnosticsStorage);
            //_ = await _sandboxResourceOperationService.UpdateStatus((int)operationEntry.Id, azureSandbox.DiagnosticsStorage.ProvisioningState.ToString());
            //return azureSandbox;
        }

        public async Task<SandboxWithCloudResourcesDto> CreateNetworkSecurityGroup(int sandboxId, SandboxWithCloudResourcesDto azureSandbox, Region region, Dictionary<string, string> tags)
        {
            throw new NotImplementedException();
            //_logger.LogInformation($"Creating network security group for sandbox: {azureSandbox.SandboxName}");
            //// Create resource-entry
            //var sandboxResourceEntry = await _sandboxResourceService.Add(sandboxId, azureSandbox.ResourceGroupId, azureSandbox.ResourceGroupName, "NetworkSecurityGroup", "Not Yet Available", AzureResourceNameUtil.NetworkSecGroup(azureSandbox.SandboxName));

            //// Create resource-operation-entry
            //var sandboxOperation = CreateInitialResourceOperation("Create Network Security Group.");
            //var operationEntry = await _sandboxResourceOperationService.Add((int)sandboxResourceEntry.Id, sandboxOperation);

            ////NSG creation
            ////TODO: Add to queue instead
            //azureSandbox.NetworkSecurityGroup = await _nsgService.CreateSecurityGroupForSubnet(region, azureSandbox.ResourceGroupName, azureSandbox.SandboxName, tags);

            //// Update entries
            //_ = await _sandboxResourceService.Update((int)sandboxResourceEntry.Id, azureSandbox.NetworkSecurityGroup);
            //_ = await _sandboxResourceOperationService.UpdateStatus((int)operationEntry.Id, azureSandbox.NetworkSecurityGroup.Inner.ProvisioningState.ToString());
            //return azureSandbox;
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

        public async Task<SandboxWithCloudResourcesDto> CreateBastion(int sandboxId, SandboxWithCloudResourcesDto azureSandbox, Region region, Dictionary<string, string> tags)
        {
            //TODO: How to make this ready for execution after picking up queue message?
            _logger.LogInformation($"Creating Bastion for sandbox: {azureSandbox.SandboxName}");
            var bastion = await _bastionService.Create(region, azureSandbox.ResourceGroupName, azureSandbox.StudyName, azureSandbox.SandboxName, azureSandbox.VNet.BastionSubnetId, tags);
            await _sandboxResourceService.Add(sandboxId, azureSandbox.ResourceGroupId, azureSandbox.ResourceGroupName, bastion);
            return azureSandbox;
        }

        public async Task<SandboxWithCloudResourcesDto> CreateVM(int sandboxId, SandboxWithCloudResourcesDto azureSandbox, Region region, Dictionary<string, string> tags)
        {
            //TODO: How to make this ready for execution after picking up queue message?
            _logger.LogInformation($"Creating Virtual Machine for sandbox: {azureSandbox.SandboxName}");
            var virtualMachine = await _vmService.Create(region, azureSandbox.ResourceGroupName, azureSandbox.SandboxName, azureSandbox.VNet.Network, azureSandbox.VNet.SandboxSubnetName, "sepesTestAdmin", "sepesRules12345", "Cheap", "windows", "win2019datacenter", tags, azureSandbox.DiagnosticsStorage.Name);
            await _sandboxResourceService.Add(sandboxId, azureSandbox.ResourceGroupId, azureSandbox.ResourceGroupName, virtualMachine.Type, virtualMachine.Key, virtualMachine.Name);
            return azureSandbox;
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
