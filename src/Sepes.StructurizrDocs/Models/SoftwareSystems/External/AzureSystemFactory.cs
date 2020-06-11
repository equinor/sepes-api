using Structurizr;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sepes.StructurizrDocs.Models.SoftwareSystems.External
{
    public static class AzureSystemFactory
    {
        public static SoftwareSystem AzureIaaS(Model model)
        {
            var azureIaaS = model.AddSoftwareSystem(Location.Internal, "Azure IaaS", "Hosts sandboxes.");
            azureIaaS.AddTags(Constants.ExistingSystemTag);

            return azureIaaS;
        }

        public static SoftwareSystem AzureAD(Model model)
        {
            var azureAd = model.AddSoftwareSystem(Location.Internal, "Azure AD", "Provides authentication and authorization");
            azureAd.AddTags(Constants.ExistingSystemTag);

            return azureAd;
        }
    }
}
