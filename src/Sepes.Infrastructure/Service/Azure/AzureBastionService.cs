using Microsoft.Azure.Management.Fluent;

using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.Azure.Management.Network.Fluent.Models;
using Azure.ResourceManager.Network.Models;
using System.Collections.Generic;
using static Microsoft.Azure.Management.ResourceManager.Fluent.Core.RestClient;
using Sepes.Infrastructure.Util;

namespace Sepes.Infrastructure.Service
{
    public class AzureBastionService : AzureServiceBase, IAzureBastionService
    {


        public AzureBastionService(IConfiguration config, ILogger logger) : base(config, logger)
        {

        }

        //TODO: Add Constructor

      

        public async Task<BastionHostInner> Create(Region region, string resourceGroupName, string studyName, string sandboxName, string nsgName)
        {

            var publicIpName = AzureResourceNameUtil.BastionPublicIp(studyName, sandboxName); // $"pip-{studyName}-{sandboxName}-bastion";

            var ipConfigs = new List<BastionHostIPConfigurationInner> { new BastionHostIPConfigurationInner()
                {
                Name = publicIpName,
                //Subnet = ,
                PrivateIPAllocationMethod = Microsoft.Azure.Management.Network.Fluent.Models.IPAllocationMethod.Static
                } };

            var restClientBuilder = RestClient.Configure().WithEnvironment(Microsoft.Azure.Management.ResourceManager.Fluent.AzureEnvironment.AzureGlobalCloud).WithCredentials(_credentials);

            using (var client = new NetworkManagementClient(restClientBuilder.Build()))
            {

                var bastion = new BastionHostInner()
                {
                    Location = region.Name,
                    IpConfigurations = ipConfigs,
                };

                var bastionName = AzureResourceNameUtil.Bastion(studyName, sandboxName);
                var createdBastion = await client.BastionHosts.CreateOrUpdateAsync(resourceGroupName, bastionName, bastion);

                return createdBastion;
            }
        }
    }



    //public async Task Delete(string resourceGroupName, string securityGroupName)
    //{
    //    await _azure.NetworkSecurityGroups.DeleteByResourceGroupAsync(resourceGroupName, securityGroupName);
    //}





}

