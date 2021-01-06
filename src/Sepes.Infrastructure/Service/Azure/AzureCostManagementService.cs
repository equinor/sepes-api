using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Sepes.Infrastructure.Dto.Azure;
using Sepes.Infrastructure.Service.Azure.Interface;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure
{
    public class AzureCostManagementService : AzureApiServiceBase, IAzureCostManagementService
    {
        public AzureCostManagementService(IConfiguration config, ILogger<AzureCostManagementService> logger, ITokenAcquisition tokenAcquisition) : base(config, logger, tokenAcquisition)
        {
        }

        public async Task<double> GetVmPrice(string region, string size, CancellationToken cancellationToken = default)
        {           
            var priceUrl = $"https://prices.azure.com/api/retail/prices?$filter=serviceName eq 'Virtual Machines' and armRegionName eq '{region}' and armSkuName eq '{size}' and priceType eq 'Consumption'";    
            
            var prices = await GetResponse<AzurePriceResponseDto>(priceUrl, false, cancellationToken);                 

            var relevantPricesInOrder = prices.Items.Where(p => p.effectiveStartDate <= DateTime.UtcNow).OrderBy(p => p.meterName.ToLower().Contains("spot") || p.meterName.ToLower().Contains("low")).ThenByDescending(p => p.retailPrice).ToList();
            var relevantPriceItem = relevantPricesInOrder.FirstOrDefault();

            if(relevantPriceItem == null)
            {
                return 0.0;
            }

            return relevantPriceItem.retailPrice * 730; //Prices are per hour, azure defaults to 730 hours/month in their web interface
        }      
    }
}
