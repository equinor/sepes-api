﻿using Microsoft.Azure.Management.Compute.Models;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sepes.Azure.Dto
{
    public class AzureSkuResponse
    {
        public IEnumerable<AzureResourceSku> value { get; set; }     
    }

    public class AzureResourcesSkuCapabilities
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("value")]
        public string Value { get; set; }
    }

    public class AzureResourceSku
    {       
        [JsonPropertyName("resourceType")]
        public string ResourceType { get; set; }  
      
        [JsonPropertyName("name")]
        public string Name { get; set; }              

        [JsonPropertyName("capabilities")]
        public IList<AzureResourcesSkuCapabilities> Capabilities { get; set; }  
       
        [JsonPropertyName("restrictions")]
        public IList<ResourceSkuRestrictions> Restrictions { get; set; }  
    }
}
