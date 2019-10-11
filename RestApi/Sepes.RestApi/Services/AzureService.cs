using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System;
using System.Threading.Tasks;


namespace Sepes.RestApi.Services
{
    // Wraps call to azure. This service will most likely need to be split up into smaller servies.
    // This is (and future children) is the only code that is alloed to create and destoy azure resources.
    public class AzureService : IAzureServices
    {
        // note do not want to include the AzureSdk just now so this will be more of a general list.

        // CreateResourceGroup(...);
        public IResourceGroup CreateResourceGroup(int podID, string podName, string podTag, IAzure azure)//Change to long form so function prompt is more descriptive
        {
            //throw new NotImplementedException();
            //return will likely be in form of Pod model
            //if(!hasresourcegroup()){
            //Create ResourceGroup
            Console.WriteLine("Creating a resource group with name: " + podName);

            var resourceGroup = azure.ResourceGroups
                    .Define(podName)
                    .WithRegion(Region.EuropeNorth)
                    .WithTag("Group", podTag) //Group is whatever we name the key as later.
                    .Create();

            Console.WriteLine("Created a resource group with name: " + podName);
            return resourceGroup;
            //}

        }
        // TerminateResourceGroup(...);
        public void TerminateResourceGroup(string resourceGroupName, IAzure azure)
        {
            //Wrap in try...catch? Or was that done in controller?
            azure.ResourceGroups.DeleteByName(resourceGroupName); //Delete might not be what we want.
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