using Sepes.Infrastructure.Dto.Azure;
using System.Collections.Generic;

namespace Sepes.Infrastructure.Constants
{
    public static class AzureRegionConstants
    {
        public static List<AzureRegionDto> Regions { get; set; } = new List<AzureRegionDto>()
        {
            new AzureRegionDto{ Key= "norwayeast", DisplayValue = "Norway East" },
            new AzureRegionDto{ Key= "europenorth",  DisplayValue = "North Europe" },
            new AzureRegionDto{ Key= "europewest",  DisplayValue = "West Europe" }
        };
    }
}
