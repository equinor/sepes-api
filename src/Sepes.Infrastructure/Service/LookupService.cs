using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Service.Interface;
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
                new LookupDto{ Key= Constants.StudyRoles.SponsorRep, DisplayValue = Constants.StudyRoles.SponsorRep },
                new LookupDto{ Key= Constants.StudyRoles.VendorAdmin,  DisplayValue = Constants.StudyRoles.VendorAdmin },
                new LookupDto{ Key= Constants.StudyRoles.VendorContributor,  DisplayValue = Constants.StudyRoles.VendorContributor },
                new LookupDto{ Key= Constants.StudyRoles.StudyViewer,  DisplayValue =  Constants.StudyRoles.StudyViewer }
            };
        }
    }
}
