
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.Network.Fluent.Models;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Azure.ResourceManager.Network;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Azure.ResourceManager.Network.Models;

namespace Sepes.Infrastructure.Service
{
    public class AzureVNetService : AzureServiceBase, IAzureVNetService
    {       
        readonly IAzureNwSecurityGroupService _nsgService;

        public AzureVNetService(IConfiguration config, ILogger<AzureVNetService> logger, IAzureNwSecurityGroupService nsgService)
            :base (config, logger)
        {
          
            _nsgService = nsgService ?? throw new ArgumentNullException(nameof(nsgService));

        }

        //TODO: Add Constructor

        public string CreateVNetName(string studyName, string sandboxName)
        {
            return $"vnet-study-{studyName}-{sandboxName}";
        }

        public async Task<INetwork> Create(Region region, string resourceGroupName, string studyName, string sandboxName)
        {
            var networkName = CreateVNetName(studyName, sandboxName);

            var addressSpace = "10.100.10.0/23"; // Until 10.100.11.255 Can have 512 adresses, but must reserve some;

            var bastionSubnetName = "AzureBastionSubnet";
            var bastionSubnetAddress = "10.100.0.0/24"; //Can only use 256 adress, so max is 10.100.0.255

            var sandboxSubnetName = $"snet-{sandboxName}";
            var sandboxSubnetAddress = "10.100.1.0/24";

            var network = await _azure.Networks.Define(networkName)
                .WithRegion(region)
                .WithExistingResourceGroup(resourceGroupName)
                .WithAddressSpace(addressSpace)
                .WithSubnet(bastionSubnetName, bastionSubnetAddress)
                .WithSubnet(sandboxSubnetName, sandboxSubnetAddress)  
                
                .CreateAsync();

     

            

                

               

        

            return network;
        }

        //public async Task<INetwork> Create(Region region, string resourceGroupName, string studyName, string sandboxName)
        //{
        //    var networkName = CreateVNetName(studyName, sandboxName);

        //    var addressSpace = "10.100.10.0/23"; // Until 10.100.11.255 Can have 512 adresses, but must reserve some;

        //    var bastionSubnetName = "AzureBastionSubnet";
        //    var bastionSubnetAddress = "10.100.0.0/24"; //Can only use 256 adress, so max is 10.100.0.255

        //    var sandboxSubnetName = $"snet-{sandboxName}";
        //    var sandboxSubnetAddress = "10.100.1.0/24";

        //    var network = await _azure.Networks.Define(networkName)
        //        .WithRegion(region)
        //        .WithExistingResourceGroup(resourceGroupName)
        //        .WithAddressSpace(addressSpace)
        //        .WithSubnet(bastionSubnetName, bastionSubnetAddress)
        //        .WithSubnet(sandboxSubnetName, sandboxSubnetAddress)
        //        .CreateAsync();
        //    using (NetworkManagementClient client = new NetworkManagementClient(credentials))
        //    {

        //        VirtualNetworkInner vnet = new VirtualNetworkInner()
        //    {
        //        Location = "West US",
        //        AddressSpace = new AddressSpace()
        //        {
        //            AddressPrefixes = new List<string>() { "0.0.0.0/16" }
        //        },

        //        DhcpOptions = new DhcpOptions()
        //        {
        //            DnsServers = new List<string>() { "1.1.1.1", "1.1.2.4" }
        //        },

        //        Subnets = new List<Subnet>()
        //{
        //    new Subnet()
        //    {
        //        Name = subnet1Name,
        //        AddressPrefix = "1.0.1.0/24",
        //    },
        //    new Subnet()
        //    {
        //        Name = subnet2Name,
        //       AddressPrefix = "1.0.2.0/24",
        //    }
        //}
        //    };

        //    await client.VirtualNetworks.CreateOrUpdateAsync(resourceGroupName, vNetName, vnet);
        //    }
        //    return network;
        //}

        public async Task Delete(string resourceGroup, string vNetName)
        {
            await _azure.Networks.DeleteByResourceGroupAsync(resourceGroup, vNetName);
            return;
        }

    }
}
