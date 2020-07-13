﻿using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Dto;
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
        readonly IAzureBastionService _bastionService;
        readonly IAzureNwSecurityGroupService _nsgService;

        public static readonly string UnitTestPrefix = "unit-test";

        public AzureService(ILogger logger, CloudResourceService resourceService, IAzureResourceGroupService resourceGroupService
            , IAzureVNetService vNetService, IAzureBastionService bastionService, IAzureNwSecurityGroupService nsgService
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _resourceService = resourceService ?? throw new ArgumentNullException(nameof(resourceService));
            _resourceGroupService = resourceGroupService ?? throw new ArgumentNullException(nameof(resourceGroupService));
            _vNetService = vNetService ?? throw new ArgumentNullException(nameof(vNetService));
            _bastionService = bastionService ?? throw new ArgumentNullException(nameof(bastionService));
            _nsgService = nsgService ?? throw new ArgumentNullException(nameof(nsgService));
        }

        public async Task<AzureSandboxDto> CreateSandboxAsync(string studyName, Region region)
        {
            var azureSandbox = new AzureSandboxDto() { StudyName = studyName };

            azureSandbox.SandboxName = AzureResourceNameUtil.Sandbox(studyName);         

            _logger.LogInformation($"Creating sandbox for study {studyName}. Sandbox name: {azureSandbox.SandboxName}");

            _logger.LogInformation($"Creating resource group");

            //TODO: ADD RELEVANT TAGS, SEE AzureResourceGroupService FOR A PARTIAL LIST 
            var resourceGroupTags = new Dictionary<string, string>();
                    
            azureSandbox.ResourceGroup = await _resourceGroupService.CreateForStudy(studyName, azureSandbox.SandboxName, region, resourceGroupTags);
                                  
            _logger.LogInformation($"Resource group created! Id: {azureSandbox.ResourceGroupId}, name: {azureSandbox.ResourceGroupName}");

            //Add RG to resource table in SEPES DB
            await _resourceService.AddResourceGroup(azureSandbox.ResourceGroupId, azureSandbox.ResourceGroupName, azureSandbox.ResourceGroup.Type);

            var nsgForSandboxSubnet = await _nsgService.CreateSecurityGroupForSubnet(region, azureSandbox.ResourceGroupName, azureSandbox.SandboxName);

            azureSandbox.VNet = await _vNetService.Create(region, azureSandbox.ResourceGroupName, azureSandbox.StudyName, azureSandbox.SandboxName);
            var subnetName = $"snet-{azureSandbox.SandboxName}"; //TODO: RETURN FROM METHOD ABOVE IN DTO
            await _vNetService.ApplySecurityGroup(azureSandbox.ResourceGroupName, nsgForSandboxSubnet.Name, subnetName, azureSandbox.VNet.Name);


            var bastion = await _bastionService.Create(region, azureSandbox.ResourceGroupName, studyName, azureSandbox.SandboxName, azureSandbox.VNet.BastionSubnetId);


           //TODO: Add VNET, Subnet and Bastion to resource table in SEPES DB

            //TODO: CREATE VNET, SUBNET AND BASTION (VNetService)
            //Nytt api: Alt i samme OP
            //TODO; CREATE SECURITYGROUP (VNetService or NsgService)
            //TODO: ASSIGN NSG TO SUBNET (VNetService or NsgService)



            //TODO: CREATE VMs (VmService)


            _logger.LogInformation($"Sandbox created: {azureSandbox.SandboxName}");

            //TODO: FIX RETURN
            return azureSandbox;
        }

        public async Task NukeSandbox(string studyName, string sandboxName, string resourceGroupName)
        {
            //Delete ResourceGroup (and hence all its contents)
            _logger.LogInformation($"Terminating sandbox for study {studyName}. Sandbox name: {sandboxName}");

            _logger.LogInformation($"Deleting resource group {resourceGroupName}");
            await _resourceGroupService.Delete(resourceGroupName);


            //Update sepes db? NO, ONE LEVEL ABOVE SHOULD DO THAT
        }

        public async Task NukeUnitTestSandboxes()
        {
            //TODO: Get list of resource groups
            //If resource group has prefix, nuke it

        }


    }
}
