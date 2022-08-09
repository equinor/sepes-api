using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Azure.Dto;
using Sepes.Azure.Service.Interface;
using Sepes.Common.Constants;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Azure.Service
{
    public class AzureResourceProviderApiService : AzureApiServiceBase, IAzureResourceProviderApiService
    {
        const string API_VERSION_ARGUMENT = "api-version=2021-04-01";
        readonly string _subscriptionId;
        public AzureResourceProviderApiService(IConfiguration config, ILogger<AzureResourceProviderApiService> logger, IAzureApiRequestAuthenticatorService azureApiRequestAuthenticatorService, HttpClient httpClient) : base(config, logger, azureApiRequestAuthenticatorService, httpClient)
        {
            _subscriptionId = config[ConfigConstants.SUBSCRIPTION_ID];
        }

        public async Task<IList<string>> ListSupportedLocations(string @namespace, string resourceTypeName, CancellationToken cancellationToken = default)
        {
            var providerResponse = await GetAzureResourceProvider(@namespace, cancellationToken);
            if (providerResponse == null)
                return null;

            var resourceType = providerResponse.ResourceTypes.SingleOrDefault(p => p.ResourceType == resourceTypeName);
            var locations = resourceType.Locations.Where(l => l.Contains("Europe") || l.Contains("Norway")).ToList();
            return locations;
        }

        private async Task<AzureProviderResponse> GetAzureResourceProvider(string @namespace, CancellationToken cancellation)
        {
            var apiUrl = $"https://management.azure.com/subscriptions/{_subscriptionId}/providers/{@namespace}?{API_VERSION_ARGUMENT}";
            var response = await PerformRequest<AzureProviderResponse>(apiUrl, HttpMethod.Get, needsAuth: true, cancellationToken: cancellation);
            return response;
        }
    }
}
