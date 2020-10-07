using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Service;
using System.Threading.Tasks;

namespace Sepes.Functions
{
    public class SandboxResourceMonitoringFunction
    {
        readonly IAzureResourceMonitoringService _resourceMonitoringService;

        public SandboxResourceMonitoringFunction(IAzureResourceMonitoringService resourceMonitoringService)
        {
            _resourceMonitoringService = resourceMonitoringService;
        }

        [FunctionName("SandboxResourceMonitoring")]
        public async Task Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, ILogger log)
        {
           await _resourceMonitoringService.StartMonitoringSession();
        }       
    }
}
