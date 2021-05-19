using Microsoft.Azure.Management.Compute.Models;
using System.Collections.Generic;

namespace Sepes.Azure.Dto
{
    public class AzureSkuResponse
    {
        public IEnumerable<ResourceSku> Value { get; set; }
    }
}
