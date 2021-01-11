using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Service.Interface;
using System.Threading.Tasks;

namespace Sepes.Functions
{
    public class UpdateCacheFunction
    {
        readonly IVirtualMachineSizeService _vmSizeService;
        readonly IVirtualMachineDiskService _virtualMachineDiskService;

        public UpdateCacheFunction(IVirtualMachineSizeService vmSizeService, IVirtualMachineDiskService virtualMachineDiskService)
        {
            _vmSizeService = vmSizeService;
            _virtualMachineDiskService = virtualMachineDiskService;
        }

        //To run every minute (in debug only): "0 */30 * * * *"
        //Run every hour: "0 * * * *"    
        //Run ever 6 hour "0 */6 * * *"
        [FunctionName("UpdateAllCaches")]
        public async Task Run([TimerTrigger("0 */6 * * *", RunOnStartup = true)] TimerInfo myTimer, ILogger log)
        {
            await _vmSizeService.UpdateVmSizeCache();
            await _virtualMachineDiskService.UpdateVmDiskSizeCache();
        }
    }
}
