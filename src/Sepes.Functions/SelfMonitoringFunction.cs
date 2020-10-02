using System;
using System.Text;
using Microsoft.ApplicationInsights;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Sepes.Functions
{
    public static class SelfMonitoringFunction
    {
        [FunctionName("SelfMonitoring")]
        public static void Run([TimerTrigger("0 */15 * * * *")]TimerInfo myTimer, ILogger log)
        {
            var databaseOkay = true;
            var provisioningQueueOkay = true;

            var selfMonitoringStringBuilder = new StringBuilder($"Self monitoring started at {DateTime.UtcNow}: ");
            selfMonitoringStringBuilder.Append(GenerateKeyValue("db", databaseOkay));
            selfMonitoringStringBuilder.Append(GenerateKeyValue("resource queue", provisioningQueueOkay));
            log.LogWarning(selfMonitoringStringBuilder.ToString());
       
       
        }

        static string GenerateKeyValue(string key, bool okay)
        {
            return $" | {key}:{(okay ? "ok" : "error")}";
        }
    }
}
