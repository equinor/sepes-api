﻿using System.Collections.Generic;

namespace Sepes.Common.Response
{
    public class HealthSummaryResponse
    {
        public bool DatabaseConnectionOk { get; set; }
        public Dictionary<string, string> IpAddresses { get; set; }

        public Dictionary<string, string> Headers { get; set; }

    }
}
