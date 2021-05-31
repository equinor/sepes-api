using Microsoft.Azure.WebJobs;
using Sepes.Provisioning.Service.Interface;
using System.Threading.Tasks;

namespace Sepes.Functions
{    public class SandboxResourceMonitoringFunction
    {      
        readonly ICloudResourceMonitoringService _resourceMonitoringService;

        public SandboxResourceMonitoringFunction(ICloudResourceMonitoringService resourceMonitoringService)
        {           
            _resourceMonitoringService = resourceMonitoringService;
        }

        //To run every minute (in debug only): "0 */30 * * * *"
        //Run every hour: "0 * * * *"        
        [FunctionName("SandboxResourceMonitoring")]
        public async Task Run([TimerTrigger("0 * * * * *", RunOnStartup = true)] TimerInfo myTimer)
        {
            await _resourceMonitoringService.StartMonitoringSession();
        }
    }
}
