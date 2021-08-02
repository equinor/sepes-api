using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Azure.Dto;
using Sepes.Azure.Service.Interface;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Azure.Service
{
    public class AzureCostManagementService : AzureApiServiceBase, IAzureCostManagementService
    {
        public AzureCostManagementService(IConfiguration config, ILogger<AzureCostManagementService> logger, IAzureApiRequestAuthenticatorService azureApiRequestAuthenticatorService, HttpClient httpClient)
            : base(config, logger, azureApiRequestAuthenticatorService, httpClient)
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
