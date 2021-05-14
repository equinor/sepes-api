using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Sepes.Azure.Service.Interface;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sepes.Azure.Dto;
using Sepes.Common.Service;
using System.Net.Http;

namespace Sepes.Azure.Service
{
    public class AzureCostManagementService : RestApiServiceBase, IAzureCostManagementService
    {
        public AzureCostManagementService(IConfiguration config, ILogger<AzureCostManagementService> logger, ITokenAcquisition tokenAcquisition, HttpClient httpClient)
            : base(config, logger, tokenAcquisition, httpClient)
        {

        }

        public async Task<double> GetVmPrice(string region, string size, CancellationToken cancellationToken = default)
        {
            //Size
            var sizePriceUrl = $"https://prices.azure.com/api/retail/prices?$filter=serviceName eq 'Virtual Machines' and armRegionName eq '{region}' and armSkuName eq '{size}' and priceType eq 'Consumption'";

            var sizePrices = await GetResponse<AzureRetailPriceApiResponseDto>(sizePriceUrl, false, cancellationToken);

            var relevantPricesInOrder = sizePrices.Items.Where(p => p.effectiveStartDate <= DateTime.UtcNow).OrderBy(p => p.meterName.ToLower().Contains("spot") || p.meterName.ToLower().Contains("low")).ThenByDescending(p => p.retailPrice).ToList();

            var relevantPriceItem = relevantPricesInOrder.FirstOrDefault();

            if (relevantPriceItem == null)
            {
                return 0.0;
            }

            return (relevantPriceItem.retailPrice * 730); //Prices are per hour, azure defaults to 730 hours/month in their web interface
        }
    }
}
