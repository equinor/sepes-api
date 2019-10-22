using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System;
using System.Threading.Tasks;
using Sepes.RestApi.Model;
using System.Diagnostics.CodeAnalysis;

namespace Sepes.RestApi.Services
{
    // Wraps call to azure. This service will most likely need to be split up into smaller servies.
    // This is (and future children) is the only code that is alloed to create and destoy azure resources.
    [ExcludeFromCodeCoverage]
    public class AzureService : IAzureService
    {
        private readonly IAzure _azure;
        private readonly string _commonResourceGroup;
        public AzureService(AzureConfig config)
        {
            _commonResourceGroup = config.commonGroup;
            _azure = Azure.Authenticate(config.credentials).WithDefaultSubscription();

            if (!_azure.ResourceGroups.Contain(_commonResourceGroup))
            {
                _azure.ResourceGroups
                    .Define(_commonResourceGroup)
                    .WithRegion(Region.EuropeNorth)
                    .Create();
            }
        }
        public string getSubscription()
        {
            return _azure.GetCurrentSubscription().DisplayName;
        }

        // CreateResourceGroup(...);
        public async Task<string> CreateResourceGroup(string networkName)
        {
            //Create ResourceGroup
            var resourceGroup = await _azure.ResourceGroups
                    .Define(networkName)
                    .WithRegion(Region.EuropeNorth)
                    .CreateAsync();
            return resourceGroup.Id;//return resource id from iresource objects

        }
        // TerminateResourceGroup(...);
        public Task TerminateResourceGroup(string commonResourceGroup)
        {
            //Wrap in try...catch? Or was that done in controller?
            return _azure.ResourceGroups.DeleteByNameAsync(commonResourceGroup); //Delete might not be what we want.
            //Might instead want to get list of all users then remove them?
        }

        // Add gives a user contributor to a resource group and network join on a network
        // AddUser(...)
        // RemoveUser(...)

        // CreateNetwork(...)

        public async Task<string> CreateNetwork(string networkName, string addressSpace)
        {
            var network = await _azure.Networks.Define(networkName)
                .WithRegion(Region.EuropeNorth)
                .WithExistingResourceGroup(_commonResourceGroup)
                .WithAddressSpace(addressSpace)
                .CreateAsync();

            return network.Id;
        }

        // RemoveNetwork(...)
        public async Task RemoveNetwork(string vNetName)
        {
            await _azure.Networks.DeleteByResourceGroupAsync(_commonResourceGroup, vNetName);
            return;
        }

        // CreateNsg(...)
        // ApplyNsg(...)
        // RemoveNsg(...)

        // ApplyDataset(...)
        // Don't need a remove dataset as that happes when resource group gets terminated.
    }
}