using Sepes.Infrastructure.Service.Interface;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class WaitForOperationService
    {       
        readonly ICloudResourceOperationReadService _cloudResourceOperationReadService;

        public WaitForOperationService(ICloudResourceOperationReadService cloudResourceOperationReadService)
        {      
            _cloudResourceOperationReadService = cloudResourceOperationReadService ?? throw new ArgumentNullException(nameof(cloudResourceOperationReadService));
        }

        protected async Task WaitForOperationToCompleteAsync(int operationId, int timeoutInSeconds = 60)
        {
            var timeout = TimeSpan.FromSeconds(timeoutInSeconds);
            var startTime = DateTime.UtcNow;

            while ((DateTime.UtcNow - startTime) < timeout)
            {
                Thread.Sleep(TimeSpan.FromSeconds(3));

                if (await _cloudResourceOperationReadService.OperationIsFinishedAndSucceededAsync(operationId))
                {
                    return;
                }
                else if (await _cloudResourceOperationReadService.OperationFailedOrAbortedAsync(operationId))
                {
                    throw new Exception("Awaited operation failed");
                }
            }

            throw new Exception("Awaited operation timed out");
        }
    }
}
