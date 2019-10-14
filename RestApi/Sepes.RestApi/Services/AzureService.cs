using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System;
using System.Threading.Tasks;


namespace Sepes.RestApi.Services
{
    // Wraps call to azure. This service will most likely need to be split up into smaller servies.
    // This is (and future children) is the only code that is alloed to create and destoy azure resources.
    public class AzureService : IAzureService
    {
        IAzure _azure;
        public AzureService(IAzure azure) {
            _azure = azure;
        }

        public string getSubscription() {
            return _azure.GetCurrentSubscription().DisplayName;
        }

        // CreateResourceGroup(...);
        public IResourceGroup CreateResourceGroup(int studyID, string podName, string podTag)//Change to long form so function prompt is more descriptive
        {
            //if(!hasresourcegroup()){
            //Create ResourceGroup
            Console.WriteLine("Creating a resource group with name: " + podName);

            var resourceGroup = _azure.ResourceGroups
                    .Define(podName)
                    .WithRegion(Region.EuropeNorth)
                    .WithTag("Group", podTag) //Group is whatever we name the key as later.
                    .Create();

            Console.WriteLine("Created a resource group with name: " + podName);
            return resourceGroup;
            //}

        }
        // TerminateResourceGroup(...);
        public void TerminateResourceGroup(string resourceGroupName)
        {
            //Wrap in try...catch? Or was that done in controller?
            _azure.ResourceGroups.DeleteByName(resourceGroupName); //Delete might not be what we want.
            //Might instead want to get list of all users then remove them?
        }

        // Add gives a user contributor to a resource group and network join on a network
        // AddUser(...)
        // RemoveUser(...)

        // CreateNetwork(...)
        /*public async void CreateNetwork(string networkName, string sepesCommonNetwork, IAzure azure)
        {
            using (NetworkManagementClient client = new NetworkManagementClient(credentials))
            {
                // Define VNet
                VirtualNetworkInner vnet = new VirtualNetworkInner()
                {
                    Location = "North EU",
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

        }*/
        // RemoveNetwork(...)

        // CreateNsg(...)
        // ApplyNsg(...)
        // RemoveNsg(...)

        // ApplyDataset(...)
        // Don't need a remove dataset as that happes when resource group gets terminated.
    }
}