using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Util;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class AzureVNetService : AzureServiceBase, IAzureVNetService
    { 
        public AzureVNetService(IConfiguration config, ILogger logger)
            :base (config, logger)
        {         
          
        }       

        public async Task<AzureVNetDto> Create(Region region, string resourceGroupName, string studyName, string sandboxName)
        {
            var networkDto = new AzureVNetDto();
            var networkName = AzureResourceNameUtil.VNet(studyName, sandboxName);

            var addressSpace = "10.100.0.0/23";  //Can have 512 adresses, but must reserve some; 10.100.0.0-10.100.1.255

            var bastionSubnetName = "AzureBastionSubnet";
            var bastionSubnetAddress = "10.100.0.0/24"; //Can only use 256 adress, so max is 10.100.0.255

            var sandboxSubnetName = AzureResourceNameUtil.SubNet(sandboxName);
            var sandboxSubnetAddress = "10.100.1.0/24";

            networkDto.Network = await _azure.Networks.Define(networkName)
                .WithRegion(region)
                .WithExistingResourceGroup(resourceGroupName)
                
                .WithAddressSpace(addressSpace)
                .WithSubnet(bastionSubnetName, bastionSubnetAddress)
                .WithSubnet(sandboxSubnetName, sandboxSubnetAddress)  
                
                .CreateAsync();

            return networkDto;
        }

        public async Task ApplySecurityGroup(string resourceGroupName, string securityGroupName, string subnetName, string networkName)
        {
            //Add the security group to a subnet.
            var nsg = _azure.NetworkSecurityGroups.GetByResourceGroup(resourceGroupName, securityGroupName);
            var network = _azure.Networks.GetByResourceGroup(resourceGroupName, networkName);
            await network.Update()
                .UpdateSubnet(subnetName)
                .WithExistingNetworkSecurityGroup(nsg)
                .Parent()
                .ApplyAsync();
        }

        public async Task<bool> Exists(string resourceGroupName, string networkName)
        {
           var network = await _azure.Networks.GetByResourceGroupAsync(resourceGroupName, networkName);

            if(network == null)
            {
                return false;
            }

            return !string.IsNullOrWhiteSpace(network.Id);
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
        }
    }
}
