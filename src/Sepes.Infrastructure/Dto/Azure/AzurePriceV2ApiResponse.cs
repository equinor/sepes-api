using System;
using System.Collections.Generic;
using System.Text;

namespace Sepes.Infrastructure.Dto.Azure
{
    public class AzurePriceV2ApiResponse
    {
        public Dictionary<string, Offer> offers;
    }

    public class Offer
    {
        public Dictionary<string, PriceValue> prices;

        public string pricingTypes { get; set; }
    }

    public class PriceValue
    {
        public double value { get; set; }
    }

  

}
