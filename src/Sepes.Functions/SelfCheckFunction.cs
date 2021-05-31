using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Sepes.Functions
{
    public class SelfCheckFunction
    {
        readonly ILogger _logger;
        readonly IHealthService _healthService;

        public SelfCheckFunction(ILogger<SelfCheckFunction> logger, IHealthService healthService)
        {
            _logger = logger;
            _healthService = healthService;
        }

        [FunctionName("SelfCheck")]
        public async Task Run([TimerTrigger("0 */30 * * * *", RunOnStartup = true)]TimerInfo myTimer)
        {
            var databaseOkay = await _healthService.DatabaseOkayAsync();          

            var selfMonitoringStringBuilder = new StringBuilder($"Self check started at {DateTime.UtcNow}: ");
            selfMonitoringStringBuilder.Append(GenerateKeyValue("Database", databaseOkay));

            _logger.LogWarning(selfMonitoringStringBuilder.ToString());         
        }

        static string GenerateKeyValue(string key, bool okay)
        {
            return $" | {key}:{(okay ? "ok" : "error")}";
        }
    }
}
