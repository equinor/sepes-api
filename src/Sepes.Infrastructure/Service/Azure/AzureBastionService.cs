
using Microsoft.Azure.Management.Network;
using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Collections.Generic;
using Sepes.Infrastructure.Util;
using Microsoft.Azure.Management.Network.Models;
using Microsoft.Azure.Management.Network.Fluent.Models;

namespace Sepes.Infrastructure.Service
{
    public class AzureBastionService : AzureServiceBase, IAzureBastionService
    {
        public AzureBastionService(IConfiguration config, ILogger logger) : base(config, logger)
        {

        }

        public async Task<BastionHost> Create(Region region, string resourceGroupName, string studyName, string sandboxName, string subnetId)
        {
            try
            {
                var publicIpName = AzureResourceNameUtil.BastionPublicIp(sandboxName); // $"pip-{studyName}-{sandboxName}-bastion";

                var pip = await _azure.PublicIPAddresses.Define(publicIpName)
                 .WithRegion(region)                 
                 .WithExistingResourceGroup(resourceGroupName)
                 .WithStaticIP()      
                 .WithSku(PublicIPSkuType.Standard)
                 .CreateAsync();

                using (var client = new Microsoft.Azure.Management.Network.NetworkManagementClient(_credentials))
                {
                    client.SubscriptionId = _subscriptionId;

                    var bastionName = AzureResourceNameUtil.Bastion(sandboxName);

                    // Kan hende PrivateIPAllocationMethod bør settes til "Static"? Vet ikke hva som er ønsket her...
                    var ipConfigs = new List<BastionHostIPConfiguration> { new BastionHostIPConfiguration()
                        {
                            Name = $"{bastionName}-ip-config",
                            Subnet =  new SubResource(subnetId),
                            PrivateIPAllocationMethod = "Dynamic",
                            PublicIPAddress = new SubResource(pip.Inner.Id),
                        }
                    };

                    var bastion = new BastionHost()
                    {
                        Location = region.Name,                        
                        IpConfigurations = ipConfigs,
                        
                    };
             
                    var createdBastion = await client.BastionHosts.CreateOrUpdateAsync(resourceGroupName, bastionName, bastion);
                    
                    var provState = createdBastion.ProvisioningState;

                    var test = await client.BastionHosts.GetAsync(resourceGroupName, bastionName);

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

