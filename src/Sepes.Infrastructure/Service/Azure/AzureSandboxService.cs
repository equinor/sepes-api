﻿using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Migrations;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class AzureSandboxService
    {
        readonly ILogger _logger;
        readonly IAzureResourceService _resourceService;
        readonly IAzureResourceGroupService _resourceGroupService;
        readonly IAzureVNetService _vNetService;

        public AzureSandboxService(ILogger logger, AzureResourceService resourceService, AzureResourceGroupService resourceGroupService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _resourceService = resourceService ?? throw new ArgumentNullException(nameof(resourceService));
            _resourceGroupService = resourceGroupService ?? throw new ArgumentNullException(nameof(resourceGroupService));

        }

        public async Task<string> CreateSandboxAsync(string studyName, Region region)
        {
            var sandboxName = AzureResourceNameUtil.Sandbox(studyName);

            //TODO: CREATE SANDBOX IN SEPES DB
            //var sandbox = new Sandbox() {;

            //var region = Region.EuropeWest;

            _logger.LogInformation($"Creating sandbox for study {studyName}. Sandbox name: {sandboxName}");

         

            _logger.LogInformation($"Creating resource group");

            var tags = new Dictionary<string, string>();
            var resourceGroup = await _resourceGroupService.CreateResourceGroupForStudy(studyName, sandboxName, region, tags);

            //HOW TO REALLY KNOW IT'S CREATED FINE?           
            _logger.LogInformation($"Resource group created! Id: {resourceGroup.Id}, name: {resourceGroup.Name}");

            //Add RG to resource table
            await _resourceService.AddResourceGroup(resourceGroup.Id, resourceGroup.Name, resourceGroup.Type);

            _vNetService.Create(region, resourceGroup)

            //TODO: CREATE VNET, SUBNET AND BASTION (VNetService)
                //Nytt api: Alt i samme OP
            //TODO; CREATE SECURITYGROUP (VNetService or NsgService)
            //TODO: ASSIGN NSG TO SUBNET (VNetService or NsgService)



            //TODO: CREATE VMs (VmService)


            _logger.LogInformation($"Sandbox created: {sandboxName}");

            //TODO: FIX RETURN
            return "";
        }
    }
}
