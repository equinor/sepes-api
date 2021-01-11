using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Sepes.Infrastructure.Dto.Azure;
using Sepes.Infrastructure.Service.Azure.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure
{
    public class AzureDiskPriceService : AzureApiServiceBase, IAzureDiskPriceService
    {
        public AzureDiskPriceService(IConfiguration config, ILogger<AzureCostManagementService> logger, ITokenAcquisition tokenAcquisition) : base(config, logger, tokenAcquisition)
        {
        }

        public async Task<double> GetDiskPrice(string region, string size, CancellationToken cancellationToken = default)
        {
            var diskPriceUrl = $"https://azure.microsoft.com/api/v2/pricing/managed-disks/calculator";

            //Here is the prices in the structure they are received, but with a lot of unwanted info left out
            var diskPrices = await GetResponse<AzurePriceV2ApiResponse>(diskPriceUrl, false, cancellationToken);

            //Transponse it around to a Region -> Size -> Price hiearchy
            var diskPriceByRegion = new Dictionary<string, AzureDiskPriceForRegion>();

            foreach (var curOffer in diskPrices.offers)
            {
                if (curOffer.Key.StartsWith("premiumssd-p") && !curOffer.Key.EndsWith("-disk-mount") && !curOffer.Key.EndsWith("-one-year"))
                {
                    foreach (var curSize in curOffer.Value.prices)
                    {
                        var regionName = curSize.Key;

                        AzureDiskPriceForRegion relevantRegionItem = null;

                        if (diskPriceByRegion.TryGetValue(regionName, out relevantRegionItem) == false)
                        {
                            relevantRegionItem = diskPriceByRegion[regionName] = new AzureDiskPriceForRegion();
                        }

                        relevantRegionItem.Types.Add(curOffer.Key, new DiskType() { price = curSize.Value.value });
                    }

                }
            }

            //Example usage: "Get price for premiumssd-p2 (the 8gb one) in norwayeast"
            double priceWeAreLookingFor = default;

            AzureDiskPriceForRegion pricesForRelevantRegion = null;

            //Why are there suddenly a dash in regions here
            if (diskPriceByRegion.TryGetValue("norway-east", out pricesForRelevantRegion))
            {
                DiskType diskTypeWithPrice = null;

                if (pricesForRelevantRegion.Types.TryGetValue("premiumssd-p2", out diskTypeWithPrice))
                {
                    priceWeAreLookingFor = diskTypeWithPrice.price;
                }
            }
            return priceWeAreLookingFor;
                
        }
        }
}
