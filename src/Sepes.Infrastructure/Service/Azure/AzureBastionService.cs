
using Microsoft.Azure.Management.Network;
using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;
using Sepes.Infrastructure.Util;
using Microsoft.Azure.Management.Network.Models;

namespace Sepes.Infrastructure.Service
{
    public class AzureBastionService : AzureServiceBase, IAzureBastionService
    {


        public AzureBastionService(IConfiguration config, ILogger logger) : base(config, logger)
        {

        }

        //TODO: Add Constructor

      

        public async Task<BastionHost> Create(Region region, string resourceGroupName, string studyName, string sandboxName, string subnetId)
        {
            

            try
            {

         
            var publicIpName = AzureResourceNameUtil.BastionPublicIp(sandboxName); // $"pip-{studyName}-{sandboxName}-bastion";

                var pip  = await _azure.PublicIPAddresses.Define(publicIpName)
                 .WithRegion(region)                 
                 .WithExistingResourceGroup(resourceGroupName)   
                 .WithStaticIP()
                 .CreateAsync();

                var ipConfigs = new List<BastionHostIPConfiguration> { new BastionHostIPConfiguration()
                {
                Name = publicIpName,
                Subnet =  new SubResource(subnetId),
                PrivateIPAllocationMethod = "Static",
                PublicIPAddress = new SubResource(pip.Id)
                } };

                

                //var restClientBuilder = RestClient.Configure().WithEnvironment(Microsoft.Azure.Management.ResourceManager.Fluent.AzureEnvironment.AzureGlobalCloud).WithCredentials(_credentials);

                using (var client = new Microsoft.Azure.Management.Network.NetworkManagementClient(_credentials))
                {
                    client.SubscriptionId = _subscriptionId;

                    var bastion = new BastionHost()
                    {
                        Location = region.Name,
                        IpConfigurations = ipConfigs                        

                    };

                    var bastionName = AzureResourceNameUtil.Bastion(sandboxName);
                    var createdBastion = await client.BastionHosts.CreateOrUpdateAsync(resourceGroupName, bastionName, bastion);

                    var provState = createdBastion.ProvisioningState;

                    var test = await client.BastionHosts.GetAsync(resourceGroupName, bastionName);


                    //using (var client = new Microsoft.Azure.Management.Network.NetworkManagementClient(restClientBuilder.Build()))
                    //{

                    //    var bastion = new BastionHostInner()
                    //    {
                    //        Location = region.Name,
                    //        IpConfigurations = ipConfigs

                    //        //IpConfigurations = ipConfigs

                    //    };

                    //    var bastionName = AzureResourceNameUtil.Bastion(studyName, sandboxName);
                    //    var createdBastion = await client.BastionHosts.CreateOrUpdateAsync(resourceGroupName, bastionName, bastion);

                    return createdBastion;
                }

            }
            catch (System.Exception ex)
            {

                var polse = 1;
                throw;
            }
        }
    }



    //public async Task Delete(string resourceGroupName, string securityGroupName)
    //{
    //    await _azure.NetworkSecurityGroups.DeleteByResourceGroupAsync(resourceGroupName, securityGroupName);
    //}





}

