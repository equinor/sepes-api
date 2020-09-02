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
                new LookupDto{ Key= "NorwayEast", DisplayValue = "Norway East" },
                new LookupDto{ Key= "NorthEurope",  DisplayValue = "North Europe" },
                new LookupDto{ Key= "WestEurope",  DisplayValue = "West Europe" },
                new LookupDto{ Key= "NorwayWest",  DisplayValue =  "Norway West" }
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
