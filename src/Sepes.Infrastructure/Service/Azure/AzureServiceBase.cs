
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Model.Config;
using System;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class AzureServiceBase
    {
        protected readonly ILogger _logger;
        protected readonly IAzure _azure;
        protected readonly AzureCredentials _credentials;


        public AzureServiceBase(IConfiguration config, ILogger logger)
          
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
            var tenantId = config[ConfigConstants.TENANT_ID];
            var clientId = config[ConfigConstants.AZ_CLIENT_ID];
            var clientSecret = config[ConfigConstants.AZ_CLIENT_SECRET];


            var subscriptionId = config[ConfigConstants.SUBSCRIPTION_ID];

            _credentials = new AzureCredentialsFactory().FromServicePrincipal(clientId, clientSecret, tenantId, AzureEnvironment.AzureGlobalCloud).WithDefaultSubscription(subscriptionId);


            _azure = Microsoft.Azure.Management.Fluent.Azure.Configure()
                .Authenticate(_credentials).WithDefaultSubscription();


           // _joinNetworkRoleName = config[ConfigConstants.JOIN_NETWORK_ROLE_NAME];

;
        }

    

        public string CreateVNetName(string studyName, string sandboxName)
        {
            return $"vnet-study-{studyName}-{sandboxName}";
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

        //    var bastion = await _azure.Networks.Bas

        //    return network;
        //}

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
            using (NetworkManagementClient client = new NetworkManagementClient(credentials))
            {

                VirtualNetworkInner vnet = new VirtualNetworkInner()
            {
                Location = "West US",
                AddressSpace = new AddressSpace()
                {
                    AddressPrefixes = new List<string>() { "0.0.0.0/16" }
                },

                DhcpOptions = new DhcpOptions()
                {
                    DnsServers = new List<string>() { "1.1.1.1", "1.1.2.4" }
                },

                Subnets = new List<Subnet>()
        {
            new Subnet()
            {
                Name = subnet1Name,
                AddressPrefix = "1.0.1.0/24",
            },
            new Subnet()
            {
                Name = subnet2Name,
               AddressPrefix = "1.0.2.0/24",
            }
        }
            };

            await client.VirtualNetworks.CreateOrUpdateAsync(resourceGroupName, vNetName, vnet);
            }
            return network;
        }

        public async Task Delete(string resourceGroup, string vNetName)
        {
            await _azure.Networks.DeleteByResourceGroupAsync(resourceGroup, vNetName);
            return;
        }

    }
}
