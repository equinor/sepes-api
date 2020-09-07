using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class SandboxWorkerService : ISandboxWorkerService
    {
        readonly ILogger _logger;
        readonly ISandboxResourceService _sandboxResourceService;
        readonly IAzureResourceGroupService _resourceGroupService;
        readonly IAzureVNetService _vNetService;
        readonly IAzureNwSecurityGroupService _nsgService;
        readonly IAzureStorageAccountService _storageService;
        readonly IAzureBastionService _bastionService;
        readonly IAzureVMService _vmService;
        readonly IAzureQueueService _azureQueueService;
        readonly SandboxResourceOperationService _sandboxResourceOperationService;

        public static readonly string UnitTestPrefix = "unit-test";

        public SandboxWorkerService(ILogger<SandboxWorkerService> logger, ISandboxResourceService sandboxResourceService, IAzureResourceGroupService resourceGroupService
            , IAzureVNetService vNetService, IAzureBastionService bastionService, IAzureNwSecurityGroupService nsgService
            , IAzureVMService vmService, IAzureStorageAccountService storageService, IAzureQueueService azureQueueService, SandboxResourceOperationService sandboxResourceOperationService
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _sandboxResourceService = sandboxResourceService ?? throw new ArgumentNullException(nameof(sandboxResourceService));
            _resourceGroupService = resourceGroupService ?? throw new ArgumentNullException(nameof(resourceGroupService));
            _vNetService = vNetService ?? throw new ArgumentNullException(nameof(vNetService));
            _bastionService = bastionService ?? throw new ArgumentNullException(nameof(bastionService));
            _nsgService = nsgService ?? throw new ArgumentNullException(nameof(nsgService));
            _vmService = vmService ?? throw new ArgumentNullException(nameof(vmService));
            _storageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
            _azureQueueService = azureQueueService ?? throw new ArgumentNullException(nameof(azureQueueService));
            _sandboxResourceOperationService = sandboxResourceOperationService ?? throw new ArgumentNullException(nameof(sandboxResourceOperationService));
        }

        private SandboxResourceOperationDto CreateInitialResourceOperation(string description, int dependsOn = 0)
        {
            return new SandboxResourceOperationDto
            {
                DependsOn = dependsOn,
                Status = "Initial",
                TryCount = 0,
                SessionId = "",
                Description = description
            };
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

        public async Task<AzureSandboxDto> CreateBasicSandboxResourcesAsync(int sandboxId, Region region, string studyName, Dictionary<string, string> tags)
        {
            var azureSandbox = new AzureSandboxDto() { StudyName = studyName, SandboxName = AzureResourceNameUtil.Sandbox(studyName) };          

            _logger.LogInformation($"Creating basic sandbox resources for sandbox: {azureSandbox.SandboxName}");

            azureSandbox = await CreateResourceGroup(sandboxId, azureSandbox, region, tags);
            azureSandbox = await CreateDiagStorageAccount(sandboxId, azureSandbox, region, tags);
            azureSandbox = await CreateNetworkSecurityGroup(sandboxId, azureSandbox, region, tags);
            azureSandbox = await CreateVirtualNetwork(sandboxId, azureSandbox, region, tags);        

            _logger.LogInformation($"Done creating basic resources for sandbox: {azureSandbox.SandboxName}");

            return azureSandbox;
        }

        public async Task<AzureSandboxDto> CreateResourceGroup(int sandboxId, AzureSandboxDto azureSandbox, Region region, Dictionary<string, string> tags)
        {
            // Create entry in SandboxResource-table,
            var resourceGroupName = AzureResourceNameUtil.ResourceGroup(azureSandbox.SandboxName);
            var sandboxResourceEntry = await _sandboxResourceService.AddResourceGroup(sandboxId, "NotYetAvailable", resourceGroupName, AzureResourceType.ResourceGroup);
            _logger.LogInformation($"Creating resource group for sandbox: {azureSandbox.SandboxName}");

            // Create entry in SandboxResourceOperations-table
            var sandboxOperation = CreateInitialResourceOperation("Create Resource Group.");
            var operation = await _sandboxResourceOperationService.Add((int)sandboxResourceEntry.Id, sandboxOperation);

            // Create actual resource group in Azure.
            azureSandbox.ResourceGroup = await _resourceGroupService.Create(resourceGroupName, region, tags);

            // After Resource is created, mark entry in SandboxResourceOperations-table as "created/successful" and update Id in resource-table.
            _ = await _sandboxResourceService.Update((int)sandboxResourceEntry.Id, azureSandbox.ResourceGroup);
            _ = await _sandboxResourceOperationService.UpdateStatus((int)operation.Id, azureSandbox.ResourceGroup.ProvisioningState);
            _logger.LogInformation($"Resource group created for sandbox with Id: {sandboxId}! Id: {azureSandbox.ResourceGroupId}, name: {azureSandbox.ResourceGroupName}");

            return azureSandbox;
        }

        public async Task<AzureSandboxDto> CreateDiagStorageAccount(int sandboxId, AzureSandboxDto azureSandbox, Region region, Dictionary<string, string> tags)
        {
            _logger.LogInformation($"Creating diagnostics storage account for sandbox: {azureSandbox.SandboxName}");
            // Create resource-entry
            var sandboxResourceEntry = await _sandboxResourceService.Add(sandboxId, azureSandbox.ResourceGroupId, azureSandbox.ResourceGroupName, "StorageAccount", "Not Yet Available", "Not Yet Available");

            // Create resource-operation-entry
            var sandboxOperation = CreateInitialResourceOperation("Create Diagnostic Storage Account.");
            var operationEntry = await _sandboxResourceOperationService.Add((int)sandboxResourceEntry.Id, sandboxOperation);

            // Create storage account for diagnostics logging of vms.
            azureSandbox.DiagnosticsStorage = await _storageService.CreateDiagnosticsStorageAccount(region, azureSandbox.SandboxName, azureSandbox.ResourceGroupName, tags);

            // Update entries
            _ = await _sandboxResourceService.Update((int)sandboxResourceEntry.Id, azureSandbox.DiagnosticsStorage);
            _ = await _sandboxResourceOperationService.UpdateStatus((int)operationEntry.Id, azureSandbox.DiagnosticsStorage.ProvisioningState.ToString());
            return azureSandbox;
        }

        public async Task<AzureSandboxDto> CreateNetworkSecurityGroup(int sandboxId, AzureSandboxDto azureSandbox, Region region, Dictionary<string, string> tags)
        {
            _logger.LogInformation($"Creating network security group for sandbox: {azureSandbox.SandboxName}");
            // Create resource-entry
            var sandboxResourceEntry = await _sandboxResourceService.Add(sandboxId, azureSandbox.ResourceGroupId, azureSandbox.ResourceGroupName, "NetworkSecurityGroup", "Not Yet Available", AzureResourceNameUtil.NetworkSecGroup(azureSandbox.SandboxName));

            // Create resource-operation-entry
            var sandboxOperation = CreateInitialResourceOperation("Create Network Security Group.");
            var operationEntry = await _sandboxResourceOperationService.Add((int)sandboxResourceEntry.Id, sandboxOperation);

            //NSG creation
            azureSandbox.NetworkSecurityGroup = await _nsgService.CreateSecurityGroupForSubnet(region, azureSandbox.ResourceGroupName, azureSandbox.SandboxName, tags);

            // Update entries
            _ = await _sandboxResourceService.Update((int)sandboxResourceEntry.Id, azureSandbox.NetworkSecurityGroup);
            _ = await _sandboxResourceOperationService.UpdateStatus((int)operationEntry.Id, azureSandbox.NetworkSecurityGroup.Inner.ProvisioningState.ToString());
            return azureSandbox;
        }

        public async Task<AzureSandboxDto> CreateVirtualNetwork(int sandboxId, AzureSandboxDto azureSandbox, Region region, Dictionary<string, string> tags)
        {
            _logger.LogInformation($"Creating VNet for sandbox: {azureSandbox.SandboxName}");
            // Create resource-entry
            var sandboxResourceEntry = await _sandboxResourceService.Add(sandboxId, azureSandbox.ResourceGroupId, azureSandbox.ResourceGroupName, "VirtualNetwork", "Not Yet Available", AzureResourceNameUtil.VNet(azureSandbox.StudyName, azureSandbox.SandboxName));

            // Create resource-operation-entry
            var sandboxOperation = CreateInitialResourceOperation("Create Virtual Network.");
            var operationEntry = await _sandboxResourceOperationService.Add((int)sandboxResourceEntry.Id, sandboxOperation);

            // Create actual VNET in Azure
            azureSandbox.VNet = await _vNetService.CreateAsync(region, azureSandbox.ResourceGroupName, azureSandbox.StudyName, azureSandbox.SandboxName, tags);

            // Update Entries
            _ = await _sandboxResourceService.Update((int)sandboxResourceEntry.Id, azureSandbox.VNet.Network);
            _ = await _sandboxResourceOperationService.UpdateStatus((int)operationEntry.Id, azureSandbox.VNet.Network.Inner.ProvisioningState.ToString());

            _logger.LogInformation($"Applying NSG to subnet for sandbox: {azureSandbox.SandboxName}");

            //Applying nsg to subnet
            sandboxOperation = CreateInitialResourceOperation($"Apply Network Security Group: {azureSandbox.NetworkSecurityGroup.Name} to Subnet.");
            operationEntry = await _sandboxResourceOperationService.Add((int)sandboxResourceEntry.Id, sandboxOperation);
            await _vNetService.ApplySecurityGroup(azureSandbox.ResourceGroupName, azureSandbox.NetworkSecurityGroup.Name, azureSandbox.VNet.SandboxSubnetName, azureSandbox.VNet.Network.Name);
            _ = await _sandboxResourceOperationService.UpdateStatus((int)operationEntry.Id, "Succeeded");
            return azureSandbox;
        }

        public async Task<AzureSandboxDto> CreateBastion(int sandboxId, AzureSandboxDto azureSandbox, Region region, Dictionary<string, string> tags)
        {
            //TODO: How to make this ready for execution after picking up queue message?
            _logger.LogInformation($"Creating Bastion for sandbox: {azureSandbox.SandboxName}");
            var bastion = await _bastionService.Create(region, azureSandbox.ResourceGroupName, azureSandbox.StudyName, azureSandbox.SandboxName, azureSandbox.VNet.BastionSubnetId, tags);
            await _sandboxResourceService.Add(sandboxId, azureSandbox.ResourceGroupId, azureSandbox.ResourceGroupName, bastion);
            return azureSandbox;
        }

        public async Task<AzureSandboxDto> CreateVM(int sandboxId, AzureSandboxDto azureSandbox, Region region, Dictionary<string, string> tags)
        {
            //TODO: How to make this ready for execution after picking up queue message?
            _logger.LogInformation($"Creating Virtual Machine for sandbox: {azureSandbox.SandboxName}");
            var virtualMachine = await _vmService.Create(region, azureSandbox.ResourceGroupName, azureSandbox.SandboxName, azureSandbox.VNet.Network, azureSandbox.VNet.SandboxSubnetName, "sepesTestAdmin", "sepesRules12345", "Cheap", "windows", "win2019datacenter", tags, azureSandbox.DiagnosticsStorage.Name);
            await _sandboxResourceService.Add(sandboxId, azureSandbox.ResourceGroupId, azureSandbox.ResourceGroupName, virtualMachine.Type, virtualMachine.Key, virtualMachine.Name);
            return azureSandbox;
        }

        public async Task NukeSandbox(string studyName, string sandboxName, string resourceGroupName)
        {
            _logger.LogInformation($"Terminating sandbox for study {studyName}. Sandbox name: {sandboxName}. Deleting Resource Group {resourceGroupName} and all it's contents");
           
            await _resourceGroupService.Delete(resourceGroupName);         
        }

        public async Task NukeUnitTestSandboxes()
        {
            var deleteTasks = new List<Task>();

            //Get list of resource groups
            var resourceGroups = await _resourceGroupService.GetResourceGroupsAsList();
            foreach (var resourceGroup in resourceGroups)
            {
                //If resource group has unit-test prefix, nuke it
                if (resourceGroup.Name.Contains(SandboxWorkerService.UnitTestPrefix))
                {
                    // TODO: Mark as deleted in SEPES DB
                    deleteTasks.Add(_resourceGroupService.Delete(resourceGroup.Name));
                }
            }

            await Task.WhenAll(deleteTasks);
            return;
        }


    }
}
