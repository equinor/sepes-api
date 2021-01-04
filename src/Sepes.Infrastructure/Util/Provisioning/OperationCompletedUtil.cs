using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Service.Interface;
using System;
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
    }

 
}
