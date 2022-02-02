using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Sepes.Infrastructure.Service.ServiceNow
{
    public class ServiceNowResponse
    {
        public ServiceNowResult Result { get; set; }
    }

    public class ServiceNowResult {
        public string Status { get; set; }
        public ServiceNowResultDetails Details { get; set; }
    }

    public class ServiceNowResultDetails
    {
        public string Number { get; set; }
        [JsonPropertyName("assignment_group")]
        public string AssignmentGroup { get; set; }
        [JsonPropertyName("caller_id")]
        public string CallerId { get; set; }
        public string Category { get; set; }
        [JsonPropertyName("cmdb_ci")]
        public string CmdbCi { get; set; }
        public string Description { get; set; }
        [JsonPropertyName("short_description")]
        public string ShortDescription { get; set; }
        public string  State { get; set; }
        [JsonPropertyName("sys_id")]
        public string SysId { get; set; }
        public string Urgency { get; set; }
    }
}
