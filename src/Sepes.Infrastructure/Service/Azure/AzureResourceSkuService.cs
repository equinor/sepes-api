using Microsoft.Azure.Management.Compute.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sepes.Infrastructure.Dto.Azure;
using Sepes.Infrastructure.Service.Azure.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class AzureResourceSkuService : AzureServiceBase, IAzureResourceSkuService
    {   
        public AzureResourceSkuService(IConfiguration config, ILogger<AzureResourceSkuService> logger)
            : base(config, logger)
        {     
        
        }

        public async Task<List<ResourceSku>> GetSKUsForRegion(string region, string resourceType = null, CancellationToken cancellationToken = default)
        {
            using (var client = new Microsoft.Azure.Management.Compute.ComputeManagementClient(_credentials))
            {
                client.SubscriptionId = _subscriptionId;

                var skus = await client.ResourceSkus.ListWithHttpMessagesAsync($"location eq '{region}'", cancellationToken: cancellationToken);
                var responseText = await skus.Response.Content.ReadAsStringAsync();
                var responseDeserialized = JsonConvert.DeserializeObject<AzureSkuResponse>(responseText);

                if (String.IsNullOrWhiteSpace(resourceType))
                {
                    return responseDeserialized.Value.ToList();
                }
                else
                {
                    return responseDeserialized.Value.Where(r => r.ResourceType == resourceType).ToList();
                }
               
            }
        }
    }
}
