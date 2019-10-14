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
        public IResourceGroup CreateResourceGroup(int podID, string podName, string podTag)//Change to long form so function prompt is more descriptive
        {
            //throw new NotImplementedException();
            //return will likely be in form of Pod model
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
        }

        // Add gives a user contributor to a resource group and network join on a network
        // AddUser(...)
        // RemoveUser(...)

        // CreateNetwork(...)
        // RemoveNetwork(...)

        // CreateNsg(...)
        // ApplyNsg(...)
        // RemoveNsg(...)

        // ApplyDataset(...)
        // Don't need a remove dataset as that happes when resource group gets terminated.
    }
}