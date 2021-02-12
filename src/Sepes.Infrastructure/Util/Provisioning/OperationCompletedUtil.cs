using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Util.Provisioning
{
    public static class OperationCompletedUtil
    {
       public static async Task<bool> HandledAsAllreadyCompletedAsync(CloudResourceOperationDto operation, ICloudResourceMonitoringService monitoringService)
        {
            if (operation.Status == CloudResourceOperationState.DONE_SUCCESSFUL)
            {
                await ThrowIfUnexpectedProvisioningState(operation, monitoringService);
            }

            return false;
        }

        static async Task ThrowIfUnexpectedProvisioningState(CloudResourceOperationDto operation, ICloudResourceMonitoringService monitoringService)
        {
            var currentProvisioningState = await monitoringService.GetProvisioningState(operation.Resource);

            if (!String.IsNullOrWhiteSpace(currentProvisioningState))
            {
                if (currentProvisioningState == "Succeeded")
                {
                    return;
                }
            }

            throw new ProvisioningException($"Unexpected provisioning state for allready completed Resource: {currentProvisioningState}");
        }

        public static async Task WaitForOperationToCompleteAsync(ICloudResourceOperationReadService cloudResourceOperationReadService, int operationId, int timeoutInSeconds = 60) 
        {
            var timeout = TimeSpan.FromSeconds(timeoutInSeconds);
            var startTime = DateTime.UtcNow;

            while ((DateTime.UtcNow - startTime) < timeout)
            {
                Thread.Sleep(TimeSpan.FromSeconds(3));

                if (await cloudResourceOperationReadService.OperationIsFinishedAndSucceededAsync(operationId))
                {
                    return;
                }
                else if (await cloudResourceOperationReadService.OperationFailedOrAbortedAsync(operationId))
                {
                    throw new Exception("Awaited operation failed");
                }
            }

            throw new Exception("Awaited operation timed out");
        }
    }

 
}
