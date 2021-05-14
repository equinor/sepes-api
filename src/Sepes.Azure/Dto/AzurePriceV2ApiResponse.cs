using System.Collections.Generic;

namespace Sepes.Azure.Dto
{
    public class AzurePriceV2ApiResponse
    {
        public Dictionary<string, Offer> offers;
    }

    public class Offer
    {    
        public int size { get; set; }
        public Dictionary<string, PriceValue> prices;

        public string pricingTypes { get; set; }
    }

    public class PriceValue
    {
        public double value { get; set; }
    }

  

}
