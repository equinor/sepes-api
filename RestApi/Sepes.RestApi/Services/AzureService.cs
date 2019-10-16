using Microsoft.Azure.Management.Network;
using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.Network.Fluent.Models;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Sepes.RestApi.Model;

namespace Sepes.RestApi.Services
{
    // Wraps call to azure. This service will most likely need to be split up into smaller servies.
    // This is (and future children) is the only code that is alloed to create and destoy azure resources.
    public class AzureService : IAzureService
    {
        IAzure _azure;
        private string _commonResourceGroup;
        public AzureService(IConfiguration configuration) {
            /////////////////////
            //// Azure setup
            string tenant = configuration["Azure:TenantId"];
            string client = configuration["Azure:ClientId"];
            string secret = configuration["Azure:ClientSecret"];
            string subscription = configuration["Azure:SubscriptionId"];
            _commonResourceGroup = configuration["Azure:CommonResourceGroupNamePrefix"]+configuration["Azure:CommonResourceGroupName"];

            var creds = new AzureCredentialsFactory().FromServicePrincipal(client, secret, tenant, AzureEnvironment.AzureGlobalCloud);
            var authenticated = Azure.Authenticate(creds);
            _azure = authenticated.WithSubscription(subscription);
            
            if (!_azure.ResourceGroups.Contain(_commonResourceGroup)) {
                _azure.ResourceGroups
                    .Define(_commonResourceGroup)
                    .WithRegion(Region.EuropeNorth)
                    .Create();
            }

            //CreateNetwork("TomTestNetwork").Wait();
        }
        public string getSubscription() {
            return _azure.GetCurrentSubscription().DisplayName;
        }

        // CreateResourceGroup(...);
        public async Task<string> CreateResourceGroup(Pod pod)//Change to long form so function prompt is more descriptive
        {
            //if(!hasresourcegroup()){
            //Create ResourceGroup
            Console.WriteLine("Creating a resource group with name: " + pod.podName);

            var resourceGroup = await _azure.ResourceGroups
                    .Define(pod.studyID + '-' + pod.podName)
                    .WithRegion(Region.EuropeNorth)
                    .WithTag("Group", pod.podTag) //Group is whatever we name the key as later.
                    .CreateAsync();

            Console.WriteLine("Created a resource group with name: " + pod.podName);
            return resourceGroup.Id;//return resource id from iresource object
            //}

        }
        // TerminateResourceGroup(...);
        public Task TerminateResourceGroup(string _commonResourceGroup)
        {
            //Wrap in try...catch? Or was that done in controller?
            return _azure.ResourceGroups.DeleteByNameAsync(_commonResourceGroup); //Delete might not be what we want.
            //Might instead want to get list of all users then remove them?
        }

        // Add gives a user contributor to a resource group and network join on a network
        // AddUser(...)
        // RemoveUser(...)

        // CreateNetwork(...)
        public async Task<string> CreateNetwork(Pod pod)
        {
            var network = await _azure.Networks.Define($"{pod.studyID}-{pod.podName.Replace(" ", "-")}Network")
                .WithRegion(Region.EuropeNorth)
                .WithExistingResourceGroup(_commonResourceGroup)
                .WithAddressSpace($"10.{1 + pod.podID / 256}.{pod.podID % 256}.0/24")
                .CreateAsync();

            return network.Id;
        }

        // RemoveNetwork(...)
        public Task RemoveNetwork(string vNetName, string _commonResourceGroup)
        {
            return _azure.Networks.DeleteByResourceGroupAsync(_commonResourceGroup, vNetName);
        }

        // CreateNsg(...)
        // ApplyNsg(...)
        // RemoveNsg(...)

        // ApplyDataset(...)
        // Don't need a remove dataset as that happes when resource group gets terminated.
    }
}