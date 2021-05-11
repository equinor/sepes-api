using Sepes.Common.Constants.CloudResource;
using Sepes.Common.Dto;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Threading;
using System.Threading.Tasks;
using Sepes.Common.Exceptions;
using Sepes.Provisioning.Service.Interface;

namespace Sepes.Provisioning.Service
{
    public class OperationCompletedService : IOperationCompletedService
    {
        readonly ICloudResourceMonitoringService _cloudResourceMonitoringService;

        public OperationCompletedService(ICloudResourceMonitoringService resourceMonitoringService)
        {
            _cloudResourceMonitoringService = resourceMonitoringService;
        }
        
       public async Task<bool> HandledAsAllreadyCompletedAsync(CloudResourceOperationDto operation)
        {
            if (operation.Status == CloudResourceOperationState.DONE_SUCCESSFUL)
            {
                await ThrowIfUnexpectedProvisioningState(operation);
            }

            return false;
        }

       public async Task ThrowIfUnexpectedProvisioningState(CloudResourceOperationDto operation)
        {
            var currentProvisioningState = await _cloudResourceMonitoringService.GetProvisioningState(operation.Resource);

            if (!String.IsNullOrWhiteSpace(currentProvisioningState))
            {
                if (currentProvisioningState == "Succeeded")
                {
                    return;
                }
            }

            throw new ProvisioningException($"Unexpected provisioning state for allready completed Resource: {currentProvisioningState}");
        }

        public async Task WaitForOperationToCompleteAsync(ICloudResourceOperationReadService cloudResourceOperationReadService, int operationId, int timeoutInSeconds = 60) 
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
