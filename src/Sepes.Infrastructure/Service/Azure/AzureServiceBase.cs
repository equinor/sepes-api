
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Model.Config;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class AzureServiceBase
    {
        protected readonly IConfiguration _config;
        protected readonly ILogger _logger;
        protected readonly IAzure _azure;
        protected readonly AzureCredentials _credentials;

        protected string _subscriptionId { get; set; }


        public AzureServiceBase(IConfiguration config, ILogger logger)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var tenantId = config[ConfigConstants.AZ_TENANT_ID];
            var clientId = config[ConfigConstants.AZ_CLIENT_ID];
            var clientSecret = config[ConfigConstants.AZ_CLIENT_SECRET];

            _subscriptionId = config[ConfigConstants.SUBSCRIPTION_ID];

           // _logger.LogInformation($"Using Subscription Id {_subscriptionId}");

            _credentials = new AzureCredentialsFactory().FromServicePrincipal(clientId, clientSecret, tenantId, AzureEnvironment.AzureGlobalCloud).WithDefaultSubscription(_subscriptionId);

            _azure = Microsoft.Azure.Management.Fluent.Azure.Configure()
                .WithLogLevel(Microsoft.Azure.Management.ResourceManager.Fluent.Core.HttpLoggingDelegatingHandler.Level.Basic)
                .Authenticate(_credentials).WithSubscription(_subscriptionId);


            // _joinNetworkRoleName = config[ConfigConstants.JOIN_NETWORK_ROLE_NAME];



        }

        protected void CheckIfResourceHasCorrectManagedByTagThrowIfNot(string resourceName, IReadOnlyDictionary<string, string> resourceTags)
        {
            var convertedTags = AzureResourceTagsFactory.TagReadOnlyDictionaryToDictionary(resourceTags);
            CheckIfResourceHasCorrectManagedByTagThrowIfNot(resourceName, convertedTags);
        }     

        protected void CheckIfResourceHasCorrectManagedByTagThrowIfNot(string resourceName, IDictionary<string, string> resourceTags)
        {
            try
            {
                AzureResourceTagsFactory.CheckIfResourceIsManagedByThisInstanceThrowIfNot(_config, resourceTags);
            }
            catch (Exception ex)
            {
                throw new Exception($"Attempting to modify Azure resource not managed by this instance: {resourceName} ", ex);
            }
          
        }

        protected string GetSharedVariableThrowIfNotFoundOrEmpty(CloudResourceCRUDInput parameters, string variableName, string descriptionForErrorMessage)
        {
            string sharedVariableValue = null;

            if (parameters.TryGetSharedVariable(variableName, out sharedVariableValue) == false)
            {                
                throw new ArgumentException($"{this.GetType().Name}: Missing {descriptionForErrorMessage} from input");
            }
            else if(String.IsNullOrWhiteSpace(sharedVariableValue)){
                throw new ArgumentException($"{this.GetType().Name}: Empty {descriptionForErrorMessage} from input");
            }

            return sharedVariableValue;
        }



        //public string CreateVNetName(string studyName, string sandboxName)
        //{
        //    return $"vnet-study-{studyName}-{sandboxName}";
        //}

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

        //public async Task Delete(string resourceGroup, string vNetName)
        //{
        //    await _azure.Networks.DeleteByResourceGroupAsync(resourceGroup, vNetName);
        //    return;
        //}

    }
}
