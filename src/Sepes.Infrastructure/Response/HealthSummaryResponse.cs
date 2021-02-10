using System.Collections.Generic;

namespace Sepes.Infrastructure.Response
{
    public class HealthSummaryResponse
    {
        public bool DatabaseConnectionOk { get; set; }
        public Dictionary<string, string> IpAddresses { get; set; }
    }
}
