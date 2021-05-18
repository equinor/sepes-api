using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Sepes.Azure.Dto;
using Sepes.Azure.Service.Interface;
using Sepes.Common.Service;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Azure.Service
{
    public class AzureDiskPriceService : AzureApiServiceBase, IAzureDiskPriceService
    {
        public AzureDiskPriceService(IConfiguration config, ILogger<AzureDiskPriceService> logger, ITokenAcquisition tokenAcquisition, HttpClient httpClient)
            : base(config, logger, tokenAcquisition, httpClient)
        {
        }

        public async Task<Dictionary<string, AzureDiskPriceForRegion>> GetDiskPrices(string region = null, CancellationToken cancellationToken = default)
        {
            var diskPriceUrl = $"https://azure.microsoft.com/api/v2/pricing/managed-disks/calculator";

            //Here is the prices in the structure they are received, but with a lot of unwanted info left out
            var diskPrices = await GetResponse<AzurePriceV2ApiResponse>(diskPriceUrl, false, cancellationToken);

            //Transponse it around to a Region -> Size -> Price hiearchy
            var diskPriceByRegion = new Dictionary<string, AzureDiskPriceForRegion>();

            foreach (var curOffer in diskPrices.offers)
            {
                if (curOffer.Key.StartsWith("standardssd-e") && !curOffer.Key.EndsWith("-disk-mount") && !curOffer.Key.EndsWith("-one-year") && !curOffer.Key.Contains("-zrs"))
                {
                    foreach (var curSize in curOffer.Value.prices)
                    {
                        var regionName = curSize.Key.Replace("-", "");

                        AzureDiskPriceForRegion relevantRegionItem = null;

                        if (!diskPriceByRegion.TryGetValue(regionName, out relevantRegionItem))
                        {
                            relevantRegionItem = diskPriceByRegion[regionName] = new AzureDiskPriceForRegion();
                        }

                        relevantRegionItem.Types.Add(curOffer.Key, new DiskType() { size = curOffer.Value.size, price = curSize.Value.value });
                    }

                }
            }

            return diskPriceByRegion;
        }
    }
}
