using Sepes.Infrastructure.Dto;
using System.Collections.Generic;

namespace Sepes.Infrastructure.Service
{
    public class LookupService : ILookupService
    {
        public IEnumerable<LookupDto> GetAzureRegions()
        {
            return new List<LookupDto>()
            {
                new LookupDto{ Key= "NorwayEast", DisplayValue = "Norway East" },
                new LookupDto{  Key= "NorthEurope",  DisplayValue = "North Europe" },
                new LookupDto{Key= "WestEurope",  DisplayValue = "West Europe" },
                new LookupDto{ Key= "NorwayWest",  DisplayValue =  "Norway West" }
            };
        }
    }
}
