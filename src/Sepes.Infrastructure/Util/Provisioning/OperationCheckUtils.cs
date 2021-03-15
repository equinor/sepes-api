using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Sandbox;
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
                throw new ProvisioningException($"Operation is aborted", proceedWithOtherOperations: false, deleteFromQueue: true, logAsWarning: true, includeExceptionInWarningLog: false);
            }
            else if (operation.TryCount >= operation.MaxTryCount)
            {
                throw new ProvisioningException($"Max retry count exceeded: {operation.TryCount}", newOperationStatus: CloudResourceOperationState.ABORTED, proceedWithOtherOperations: false, deleteFromQueue: true, logAsWarning: true, includeExceptionInWarningLog: false);
            }
        }

        public static void ThrowIfResourceIsDeletedAndOperationIsNotADelete(CloudResourceOperationDto operation)
        {
            if (operation.Resource.Deleted && operation.OperationType != CloudResourceOperationType.DELETE)
            {
                throw new ProvisioningException($"Resource is marked for deletion in database", newOperationStatus: CloudResourceOperationState.ABORTED, proceedWithOtherOperations: false, deleteFromQueue: true, logAsWarning: true, includeExceptionInWarningLog: false);
            }
        }

        public static void ThrowIfPossiblyInProgress(CloudResourceOperationDto operation)
        {
            if (operation.Status == CloudResourceOperationState.IN_PROGRESS)
            {
                if (operation.Updated.AddSeconds(30) >= DateTime.UtcNow) //If updated less than xx minutes ago, probably in progress
                {
                    throw new ProvisioningException($"Possibly allready in progress", proceedWithOtherOperations: false, deleteFromQueue: false, postponeQueueItemFor: 60, logAsWarning: true, includeExceptionInWarningLog: false);
                }
            }
        }

        public static async Task ThrowIfDependentOnUnfinishedOperationAsync(CloudResourceOperationDto operation, ProvisioningQueueParentDto queueParentItem, ICloudResourceOperationReadService operationReadService)
        {
            if (operation.DependsOnOperationId.HasValue)
            {
                if (!(await operationReadService.OperationIsFinishedAndSucceededAsync(operation.DependsOnOperationId.Value)))
                {
                    var increaseBy = CloudResourceConstants.INCREASE_QUEUE_INVISIBLE_WHEN_DEPENDENT_ON_NOT_FINISHED;

                    bool storeQueueInformationOnOperation = queueParentItem.Children.Count == 1;

                    throw new ProvisioningException($"Dependant operation {operation.DependsOnOperationId.Value} is not finished. Invisibility increased by {increaseBy}", proceedWithOtherOperations: false, deleteFromQueue: false, postponeQueueItemFor: increaseBy, storeQueueInfoOnOperation: storeQueueInformationOnOperation, logAsWarning: true, includeExceptionInWarningLog: false);                               

                }
            }
        }
    }
}
