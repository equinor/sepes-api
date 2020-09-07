using Structurizr;

namespace Sepes.StructurizrDocs.Models.SoftwareSystems.External
{
    public static class AzureSystemFactory
    {

        public static SoftwareSystem AddAzureAd(Model model, SoftwareSystem system)
        {
            var azureAd = model.AddSoftwareSystem(Location.External, Constants.AzureAd, "Provides authentication and authorization");
            azureAd.AddTags(Constants.ExistingSystemTag);
            system.Uses(azureAd, "Authenticates user with");
            return azureAd;
        }

        public static SoftwareSystem AddAzureAppInsight(Model model, SoftwareSystem system)
        {
            var appi = model.AddSoftwareSystem(Location.External, Constants.AzureAppInsights, "Logging and usage statistics");
            appi.AddTags(Constants.ExistingSystemTag);       
            system.Uses(appi, "Sends logs/usage to");
            return appi;
        }


        public static SoftwareSystem AddAzureIaaS(Model model, SoftwareSystem system)
        {
            var azureIaaS = model.AddSoftwareSystem(Location.External, Constants.AzureIaaS, "Hosts Sandbox resources (Network, VMs, Storage and more). Also used for queue, diagnostics storage and more");
            azureIaaS.AddTags(Constants.ExistingSystemTag);
            system.Uses(azureIaaS, "Creates and Maintains");

            //20200805 KRST: Wanted to show Azure as a big system, with each service (Vm, Network, Storage etc) as components inside. But they ended up inside Sepes system instead. I'm probably missing something. Keeping it around for later
            //var networking = azureIaaS.AddContainer("Network", "Provides network", "Azure Network Service");
            //var storage = azureIaaS.AddContainer("Storage", "Stores data for Sandboxes. Also for diagnostics data", "Azure Storage Accounts");
            //var virtualMachines = azureIaaS.AddContainer("VMs", "Where supplier installs their software", "Azure Virtual Machines");                   
            //var bastion = azureIaaS.AddContainer("Bastion", "Provides remote access to VM's", "Azure Bastion Service");

            //containerView.Add(networking);
            //containerView.Add(storage);
            //containerView.Add(virtualMachines);
            //containerView.Add(bastion);

            return azureIaaS;
        }

        public static SoftwareSystem AddAzureAsABlackBox(Model model, SoftwareSystem system)
        {
            var msAzure = model.AddSoftwareSystem(Location.External, "Microsoft Azure", "Auth, Monitoring, Network, VMs, Storage and more");
            msAzure.AddTags(Constants.ExistingSystemTag);
            system.Uses(msAzure, "Uses");       
            return msAzure;
        }         
    }
}
