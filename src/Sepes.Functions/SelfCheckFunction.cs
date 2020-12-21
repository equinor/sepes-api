using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Sepes.Functions
{
    public class SelfCheckFunction
    {
        public SelfCheckFunction()
        {
          
        }

        [FunctionName("SelfCheck")]
        public Task Run([TimerTrigger("0 */30 * * * *", RunOnStartup = true)]TimerInfo myTimer, ILogger log)
        {
            var databaseOkay = true;
            var provisioningQueueOkay = true;

            var selfMonitoringStringBuilder = new StringBuilder($"Self check started at {DateTime.UtcNow}: ");
            selfMonitoringStringBuilder.Append(GenerateKeyValue("db", databaseOkay));
            selfMonitoringStringBuilder.Append(GenerateKeyValue("resource queue", provisioningQueueOkay));
            log.LogWarning(selfMonitoringStringBuilder.ToString());
            return Task.CompletedTask;
        }

        static string GenerateKeyValue(string key, bool okay)
        {
            return $" | {key}:{(okay ? "ok" : "error")}";
        }
    }
}
