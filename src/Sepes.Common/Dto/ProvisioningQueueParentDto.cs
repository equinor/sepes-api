﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;


namespace Sepes.Common.Dto.Sandbox
{
    public class ProvisioningQueueParentDto
    {     

        public string Description { get; set; }

        public List<ProvisioningQueueChildDto> Children { get; set; } = new List<ProvisioningQueueChildDto>();

        [JsonIgnore]
        public string MessageId { get; set; }
        [JsonIgnore]
        public string PopReceipt { get; set; }
        [JsonIgnore]
        public int DequeueCount { get; set; }

        [JsonIgnore]
        public DateTimeOffset? NextVisibleOn { get; set; }
    }
}
