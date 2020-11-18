using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Service.Interface;
using System.Threading.Tasks;

namespace Sepes.Functions
{
    public class UpdateCacheFunction
    {
        readonly IVirtualMachineSizeService _vmSizeService;

        public UpdateCacheFunction(IVirtualMachineSizeService vmSizeService)
        {
            _vmSizeService = vmSizeService;
        }

        //To run every minute (in debug only): "0 */30 * * * *"
        //Run every hour: "0 * * * *"    
        //Run ever 6 hour "0 */6 * * *"
        [FunctionName("UpdateAllCaches")]
        public async Task Run([TimerTrigger("0 */6 * * *", RunOnStartup =true)]TimerInfo myTimer, ILogger log)
        {
           await _vmSizeService.UpdateVmSizeCache();
        }       
    }
}
