using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Sepes.Infrastructure.Service.ServiceNow
{
    public class ServiceNowRequest
    {
        public ServiceNowRequest(string callerId, string category, string cmdbCi, string shortDescription)
        {
            CallerId = callerId;
            Category = category;
            CmdbCi = cmdbCi;
            ShortDescription = shortDescription;
        }

        [JsonPropertyName("caller_id")]
        public string CallerId { get; set; }  
        public string Category { get; set; }
        [JsonPropertyName("cmdb_ci")]
        public string CmdbCi { get; set; }
        [JsonPropertyName("short_description")]
        public string ShortDescription { get; set; }
    }
}
