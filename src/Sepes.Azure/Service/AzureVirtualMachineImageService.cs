using Microsoft.Azure.Management.Compute.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Azure.Service.Interface;
using Sepes.Common.Constants;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Azure.Service
{
    public class AzureVirtualMachineImageService : AzureApiServiceBase, IAzureVirtualMachineImageService
    {
        readonly string _subscriptionId;

        public AzureVirtualMachineImageService(IConfiguration config, ILogger<AzureVirtualMachineImageService> logger, IAzureApiRequestAuthenticatorService azureApiRequestAuthenticatorService, HttpClient httpClient)
            : base(config, logger, azureApiRequestAuthenticatorService, httpClient)
        {       
            _subscriptionId = config[ConfigConstants.SUBSCRIPTION_ID];
        }

        public async Task<List<VirtualMachineImageResource>> GetImagesAsync(string region, string publisher, string offer, string sku, CancellationToken cancellationToken = default)
        {
            var imagesUrl = $"https://management.azure.com/subscriptions/{_subscriptionId}/providers/Microsoft.Compute/locations/{region}/publishers/{publisher}/artifacttypes/vmimage/offers/{offer}/skus/{sku}/versions?api-version=2020-06-01";
            var images = await GetResponse<List<VirtualMachineImageResource>>(imagesUrl, cancellationToken: cancellationToken);
            return images;
        } 
    } 
}
