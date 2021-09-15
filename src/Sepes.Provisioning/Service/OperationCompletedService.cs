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

            if (!String.IsNullOrWhiteSpace(currentProvisioningState) && currentProvisioningState == "Succeeded")
            {
                return;
            }

            throw new ProvisioningException($"Unexpected provisioning state for allready completed Resource: {currentProvisioningState}");
        }        
    }
}
