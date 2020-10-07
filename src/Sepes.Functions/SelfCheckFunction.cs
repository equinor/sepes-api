using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Text;

namespace Sepes.Functions
{
    public static class SelfCheckFunction
    {
        [FunctionName("SelfCheck")]
        public static void Run([TimerTrigger("0 */15 * * * *")]TimerInfo myTimer, ILogger log)
        {
            var databaseOkay = true;
            var provisioningQueueOkay = true;

            var selfMonitoringStringBuilder = new StringBuilder($"Self check started at {DateTime.UtcNow}: ");
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
