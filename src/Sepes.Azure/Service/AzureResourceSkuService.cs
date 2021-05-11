using Microsoft.Azure.Management.Compute.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sepes.Common.Dto.Azure;
using Sepes.Azure.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Azure.Service
{
    public class AzureResourceSkuService : AzureServiceBase, IAzureResourceSkuService
    {   
        public AzureResourceSkuService(IConfiguration config, ILogger<AzureResourceSkuService> logger)
            : base(config, logger)
        {     
        
        }

        public async Task<List<ResourceSku>> GetSKUsForRegion(string region, string resourceType = null, bool filterBasedOnResponseRestrictions = true, CancellationToken cancellationToken = default)
        {
            using (var client = new Microsoft.Azure.Management.Compute.ComputeManagementClient(_credentials))
            {
                client.SubscriptionId = _subscriptionId;

                var skus = await client.ResourceSkus.ListWithHttpMessagesAsync($"location eq '{region}'", cancellationToken: cancellationToken);
                var responseText = await skus.Response.Content.ReadAsStringAsync();
                var responseDeserialized = JsonConvert.DeserializeObject<AzureSkuResponse>(responseText);

                return ApplyRelevantFilters(region, responseDeserialized.Value, resourceType, filterBasedOnResponseRestrictions);               
            }
        }

        List<ResourceSku> ApplyRelevantFilters(string region, IEnumerable<ResourceSku> source, string resourceType = null, bool filterBasedOnResponseRestrictions = true)
        {
            IEnumerable<ResourceSku> result = source;

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

        IEnumerable<ResourceSku> FilterOnResourceType(string region, IEnumerable<ResourceSku> source, string resourceType)
        {
            return source.Where(r => r.ResourceType == resourceType);
        }

        IEnumerable<ResourceSku> FilterOnResponseRestrictions(string region, IEnumerable<ResourceSku> source)
        {
            return source
                .Where(sku => sku.Restrictions.Count == 0 
                ||
                (
                !sku.Restrictions.Where(r=> r.Type == ResourceSkuRestrictionsType.Location && r.Values.Contains(region)).Any()
                && !sku.Restrictions.Where(r => r.Type == ResourceSkuRestrictionsType.Zone && r.Values.Contains(region)).Any()
                ) 
                );
        }
    }
}
