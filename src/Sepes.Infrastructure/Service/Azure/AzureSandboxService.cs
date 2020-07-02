using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sepes.Infrastructure.Service
{
    public class AzureSandboxService
    {
        readonly ILogger _logger;
        readonly AzureResourceGroupService _resourceGroupService;

        public void CreateSandbox(string studyName)
        {
            string resourceGroupName = "";

            //Remember to do logging!!!!!


            //TODO: MAYBE CREATE A RESOURCE TABLE?
            //StudyId, ResourceType, ResourceId, ResourceName, Created, CreatedBy, 

            //TODO: CREATE SANDBOX IN SEPES DB


            //TODO: CREATE RESOURCEGROUP (ResourceGroupSevice)
            //ResourceGroupName
            //_resourceGroupService.CreateResourceGroup();



            //TODO: CREATE VNET, SUBNET AND BASTION (VNetService)
            //TODO; CREATE SECURITYGROUP (VNetService or NsgService)
            //TODO: ASSIGN NSG TO SUBNET (VNetService or NsgService)



            //TODO: CREATE VMs (VmService)



        }
    }
}
