using System.Collections.Generic;

namespace Sepes.Azure.Dto
{
    public class AzurePriceV2ApiResponse
    {
        public Dictionary<string, Offer> offers { get; set; }
    }

    public class Offer
    {    
        public int size { get; set; }
        public Dictionary<string, PriceValue> prices { get; set; }

        public string pricingTypes { get; set; }
    }

    public class PriceValue
    {
        public double value { get; set; }
    }

  

}
