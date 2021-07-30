using Microsoft.Azure.Management.Compute.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Azure.Dto;
using Sepes.Azure.Service.Interface;
using Sepes.Common.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Azure.Service
{
    public class AzureResourceSkuService : AzureSdkServiceBaseV1, IAzureResourceSkuService
    {
        public AzureResourceSkuService(IConfiguration config, ILogger<AzureResourceSkuService> logger, IAzureCredentialService azureCredentialService)
            : base(config, logger, azureCredentialService)
        {

        }

        public async Task<List<AzureResourceSku>> GetSKUsForRegion(string region, string resourceType = null, bool filterBasedOnResponseRestrictions = true, CancellationToken cancellationToken = default)
        {
            using (var client = new Microsoft.Azure.Management.Compute.ComputeManagementClient(_credentials))
            {
                client.SubscriptionId = _subscriptionId;

                var skus = await client.ResourceSkus.ListWithHttpMessagesAsync($"location eq '{region}'", cancellationToken: cancellationToken);
                var responseText = await skus.Response.Content.ReadAsStringAsync();
                var responseDeserialized = JsonSerializerUtil.Deserialize<AzureSkuResponse>(responseText);

                return ApplyRelevantFilters(region, responseDeserialized.value, resourceType, filterBasedOnResponseRestrictions);
            }
        }

        List<AzureResourceSku> ApplyRelevantFilters(string region, IEnumerable<AzureResourceSku> source, string resourceType = null, bool filterBasedOnResponseRestrictions = true)
        {
            IEnumerable<AzureResourceSku> result = source;

            if (!String.IsNullOrWhiteSpace(resourceType))
            {
                result = FilterOnResourceType(region, result, resourceType);
            }

            if (filterBasedOnResponseRestrictions)
            {
                result = FilterOnResponseRestrictions(region, result);
            }

            return result.ToList();
        }

        IEnumerable<AzureResourceSku> FilterOnResourceType(string region, IEnumerable<AzureResourceSku> source, string resourceType)
        {
            return source.Where(r => r.ResourceType == resourceType);
        }

        IEnumerable<AzureResourceSku> FilterOnResponseRestrictions(string region, IEnumerable<AzureResourceSku> source)
        {
            return source
                .Where(sku => sku.Restrictions.Count == 0
                ||
                (
                !sku.Restrictions.Where(r => r.Type == ResourceSkuRestrictionsType.Location && r.Values.Contains(region)).Any()
                && !sku.Restrictions.Where(r => r.Type == ResourceSkuRestrictionsType.Zone && r.Values.Contains(region)).Any()
                )
                );
        }
    }
}
