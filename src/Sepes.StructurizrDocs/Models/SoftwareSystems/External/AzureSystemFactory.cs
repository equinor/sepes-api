using Structurizr;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sepes.StructurizrDocs.Models.SoftwareSystems.External
{
    public static class AzureSystemFactory
    {
        public static SoftwareSystem AzureVMs(Model model)
        {
            var azureIaaS = model.AddSoftwareSystem(Location.External, "Azure VMs", "Hosts Sandboxes (VMs)");
            azureIaaS.AddTags(Constants.ExistingSystemTag);

            return azureIaaS;
        }

        public static SoftwareSystem AzureStorage(Model model)
        {
            var azureIaaS = model.AddSoftwareSystem(Location.External, "Azure IaaS", "Contains data for Sandboxes");
            azureIaaS.AddTags(Constants.ExistingSystemTag);

            return azureIaaS;
        }

        public static SoftwareSystem AzureAd(Model model)
        {
            var azureAd = model.AddSoftwareSystem(Location.External, "Azure AD", "Provides authentication and authorization");
            azureAd.AddTags(Constants.ExistingSystemTag);

            return azureAd;
        }

        public static SoftwareSystem AzureAppInsight(Model model)
        {
            var appi = model.AddSoftwareSystem(Location.External, "Azure Application Insights", "Logging and usage statistics");
            appi.AddTags(Constants.ExistingSystemTag);

            return appi;
        }
    }
}
