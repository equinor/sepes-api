using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Service.Azure;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class SandboxWorkerService
    {
        readonly ILogger _logger;
        readonly ISandboxResourceService _sandboxResourceService;
        readonly IAzureResourceGroupService _resourceGroupService;
        readonly IAzureVNetService _vNetService;
        readonly IAzureNwSecurityGroupService _nsgService;
        readonly IAzureStorageAccountService _storageService;
        readonly IAzureBastionService _bastionService;
        readonly IAzureVMService _vmService;
        readonly AzureQueueService _azureQueueService;

        public static readonly string UnitTestPrefix = "unit-test";

        public SandboxWorkerService(ILogger logger, ISandboxResourceService sandboxResourceService, IAzureResourceGroupService resourceGroupService
            , IAzureVNetService vNetService, IAzureBastionService bastionService, IAzureNwSecurityGroupService nsgService
            , IAzureVMService vmService, IAzureStorageAccountService storageService, AzureQueueService azureQueueService
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
        }

        public async Task DoWork()
        {

        }

        public async Task<AzureSandboxDto> CreateBasicSandboxResourcesAsync(string studyName, Region region, Dictionary<string, string> tags)
        {
            //At what point do SEPES know enough to start creating a sandbox?
                //Sandbox exists -> ResourceGroup, Diag Storage Account

                //Network config -> Network, NSG, Bastion


            var azureSandbox = new AzureSandboxDto() { StudyName = studyName };

            azureSandbox.SandboxName = AzureResourceNameUtil.Sandbox(studyName);         

            _logger.LogInformation($"Creating basic sandbox resources for sandbox: {azureSandbox.SandboxName}");

            _logger.LogInformation($"Creating resource group");

            //TODO: ADD RELEVANT TAGS, SEE AzureResourceGroupService FOR A PARTIAL LIST            
                    
           await CreateResourceGroup(studyName)
          

            // Create storage account for diagnostics logging of vms.
            var diagStorage = await _storageService.CreateDiagnosticsStorageAccount(region, azureSandbox.SandboxName, azureSandbox.ResourceGroupName);
            await _sandboxResourceService.Add(azureSandbox.ResourceGroupId, azureSandbox.ResourceGroupName, diagStorage.Type, diagStorage.Key, diagStorage.Name);

            //NSG creation
            var nsgForSandboxSubnet = await _nsgService.CreateSecurityGroupForSubnet(region, azureSandbox.ResourceGroupName, azureSandbox.SandboxName);
            await _sandboxResourceService.Add(azureSandbox.ResourceGroupId, azureSandbox.ResourceGroupName, nsgForSandboxSubnet.Type, nsgForSandboxSubnet.Key, nsgForSandboxSubnet.Name);

            //VNet creation
            azureSandbox.VNet = await _vNetService.Create(region, azureSandbox.ResourceGroupName, azureSandbox.StudyName, azureSandbox.SandboxName);
            await _sandboxResourceService.Add(azureSandbox.ResourceGroupId, azureSandbox.ResourceGroupName, azureSandbox.VNet.Network.Type, azureSandbox.VNet.Key, azureSandbox.VNet.Name);

            //Applying nsg to subnet
            var subnetName = AzureResourceNameUtil.SubNet(azureSandbox.SandboxName); //TODO: RETURN FROM METHOD ABOVE IN DTO
            await _vNetService.ApplySecurityGroup(azureSandbox.ResourceGroupName, nsgForSandboxSubnet.Name, subnetName, azureSandbox.VNet.Network.Name);

            //Bastion creation
            //var bastion = await _bastionService.Create(region, azureSandbox.ResourceGroupName, studyName, azureSandbox.SandboxName, azureSandbox.VNet.BastionSubnetId);
            //await _sandboxResourceService.Add(azureSandbox.ResourceGroupId, azureSandbox.ResourceGroupName, bastion);

            //// CREATE VMs (VmService) 
            //var virtualMachine = await _vmService.Create(region, azureSandbox.ResourceGroupName, azureSandbox.SandboxName, azureSandbox.VNet.Network, subnetName, "sepesTestAdmin", "sepesRules12345", "Cheap", "windows", "win2019datacenter", tags);
            //await _sandboxResourceService.Add(azureSandbox.ResourceGroupId, azureSandbox.ResourceGroupName, virtualMachine.Type, virtualMachine.Key, virtualMachine.Name);

            _logger.LogInformation($"Done creating basic resources for Sandbox: {azureSandbox.SandboxName}");

            return azureSandbox;
        }

        public async Task CreateResourceGroup(Region region, AzureSandboxDto azureSandbox, string studyName)
        {
            azureSandbox.ResourceGroup = await _resourceGroupService.CreateForStudy(studyName, azureSandbox.SandboxName, region, tags);
            await _sandboxResourceService.AddResourceGroup(azureSandbox.ResourceGroupId, azureSandbox.ResourceGroupName, azureSandbox.ResourceGroup.Type);
        _logger.LogInformation($"Resource group created! Id: {azureSandbox.ResourceGroupId}, name: {azureSandbox.ResourceGroupName}");
        }

        public async Task NukeSandbox(string studyName, string sandboxName, string resourceGroupName)
        {
            
            _logger.LogInformation($"Terminating sandbox for study {studyName}. Sandbox name: {sandboxName}");

            //TODO: Get list of relevant resources and mark as deleted in our db

            _logger.LogInformation($"Deleting resource group {resourceGroupName}");
            await _resourceGroupService.Delete(resourceGroupName);


            //Update sepes db? NO, ONE LEVEL ABOVE SHOULD DO THAT
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
