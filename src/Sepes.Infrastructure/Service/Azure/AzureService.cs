using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class AzureService
    {
        readonly ILogger _logger;
        readonly ICloudResourceService _resourceService;
        readonly IAzureResourceGroupService _resourceGroupService;
        readonly IAzureVNetService _vNetService;

        public AzureService(ILogger logger, CloudResourceService resourceService, AzureResourceGroupService resourceGroupService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _resourceService = resourceService ?? throw new ArgumentNullException(nameof(resourceService));
            _resourceGroupService = resourceGroupService ?? throw new ArgumentNullException(nameof(resourceGroupService));
        }

        public async Task<string> CreateSandboxAsync(string studyName, Region region)
        {
            //STILL UNCERTAIN
            //WHAT TYPE SHOULD THIS METHOD RETURN?


            var sandboxName = AzureResourceNameUtil.Sandbox(studyName);         

            _logger.LogInformation($"Creating sandbox for study {studyName}. Sandbox name: {sandboxName}");

            _logger.LogInformation($"Creating resource group");

            //TODO: ADD RELEVANT TAGS, SEE AzureResourceGroupService FOR A PARTIAL LIST 
            var resourceGroupTags = new Dictionary<string, string>();

            var resourceGroup = await _resourceGroupService.CreateResourceGroupForStudy(studyName, sandboxName, region, resourceGroupTags);

            //TODO: DO WE NEED TO VERIFY A RESOURCE IS CREATED FINE?           
            _logger.LogInformation($"Resource group created! Id: {resourceGroup.Id}, name: {resourceGroup.Name}");

            //Add RG to resource table in SEPES DB
            await _resourceService.AddResourceGroup(resourceGroup.Id, resourceGroup.Name, resourceGroup.Type);

           // _vNetService.Create(region, resourceGroup)
           //TODO: Add VNET, Subnet and Bastion to resource table in SEPES DB

            //TODO: CREATE VNET, SUBNET AND BASTION (VNetService)
                //Nytt api: Alt i samme OP
            //TODO; CREATE SECURITYGROUP (VNetService or NsgService)
            //TODO: ASSIGN NSG TO SUBNET (VNetService or NsgService)



            //TODO: CREATE VMs (VmService)


            _logger.LogInformation($"Sandbox created: {sandboxName}");

            //TODO: FIX RETURN
            return "";
        }

        public async Task TerminateSandbox(string studyName, string sandboxName, string resourceGroupName)
        {
            //Delete ResourceGroup (and hence all its contents)
            _logger.LogInformation($"Terminating sandbox for study {studyName}. Sandbox name: {sandboxName}");

            _logger.LogInformation($"Deleting resource group {resourceGroupName}");
            await _resourceGroupService.DeleteResourceGroup(resourceGroupName);


            //Update sepes db
        }


    }
}
