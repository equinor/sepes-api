using Microsoft.Azure.Management.Compute.Models;
using System.Collections.Generic;

namespace Sepes.Infrastructure.Dto.Azure
{
    public class AzureSkuResponse
    {
        public IEnumerable<ResourceSku> Value { get; set; }
    }
}
