using Sepes.Infrastructure.Dto;
using System.Collections.Generic;

namespace Sepes.Infrastructure.Service
{
    public class LookupService : ILookupService
    {
        public IEnumerable<LookupDto> AzureRegions()
        {
            return new List<LookupDto>()
            {
                new LookupDto{ Key= "norwayeast", DisplayValue = "Norway East" },
                new LookupDto{ Key= "europenorth",  DisplayValue = "North Europe" },
                new LookupDto{ Key= "europewest",  DisplayValue = "West Europe" }
            };
        }

        public IEnumerable<LookupDto> StudyRoles()
        {
            return new List<LookupDto>()
            {
                new LookupDto{ Key= "SponsorRep", DisplayValue = "Sponsor Rep" },
                new LookupDto{ Key= "VendorAdmin",  DisplayValue = "Vendor Admin" },
                new LookupDto{ Key= "VendorContributor",  DisplayValue = "Vendor Contributor" },
                new LookupDto{ Key= "StudyViewer",  DisplayValue =  "Study Viewer" }
            };
        }
    }
}
