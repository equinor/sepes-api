using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Util.Provisioning
{
    public static class OperationCheckUtils
    {
        public static void ThrowIfTryCountExceededOrAborted(CloudResourceOperationDto operation)
        {
            if (operation.Status == CloudResourceOperationState.ABORTED)
            {
                throw new ProvisioningException($"Operation is aborted", proceedWithOtherOperations: false, deleteFromQueue: true);
            }
            else if (operation.TryCount >= operation.MaxTryCount)
            {
                throw new ProvisioningException($"Max retry count exceeded: {operation.TryCount}", newOperationStatus: CloudResourceOperationState.FAILED, proceedWithOtherOperations: false, deleteFromQueue: true);
            }
        }

        public static void ThrowIfResourceIsDeletedAndOperationIsNotADelete(CloudResourceOperationDto operation)
        {
            if (operation.OperationType != CloudResourceOperationType.DELETE && operation.Resource.Deleted.HasValue)
            {
                throw new ProvisioningException($"Resource is marked for deletion in database", newOperationStatus: CloudResourceOperationState.ABORTED, proceedWithOtherOperations: true, deleteFromQueue: true);
            }
        }

        public static void ThrowIfPossiblyInProgress(CloudResourceOperationDto operation)
        {
            if (operation.Status == CloudResourceOperationState.IN_PROGRESS)
            {
                if (operation.Updated.AddMinutes(2) >= DateTime.UtcNow) //If changed less than two minutes ago
                {
                    throw new ProvisioningException($"Possibly allready in progress", proceedWithOtherOperations: false, deleteFromQueue: false, postponeQueueItemFor: 60);
                }
            }
        }

        public static async Task ThrowIfDependentOnUnfinishedOperationAsync(CloudResourceOperationDto operation, ICloudResourceOperationReadService operationReadService)
        {
            if (operation.DependsOnOperationId.HasValue)
            {
                if (await operationReadService.OperationIsFinishedAndSucceededAsync(operation.DependsOnOperationId.Value) == false)
                {
                    var increaseBy = CloudResourceConstants.INCREASE_QUEUE_INVISIBLE_WHEN_DEPENDENT_ON_NOT_FINISHED;

                    throw new ProvisioningException($"Dependant operation {operation.DependsOnOperationId.Value} is not finished. Invisibility increased by {increaseBy}", proceedWithOtherOperations: false, deleteFromQueue: false, postponeQueueItemFor: increaseBy);                               

                }
              

                //If single operation
                //Add message details to operation for others to pick up
                //TODO: Just store this on every operation from beginning?
            }
        }


    }
}
