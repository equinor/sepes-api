using Sepes.Common.Constants.CloudResource;
using Sepes.Common.Dto;
using Sepes.Common.Dto.Sandbox;
using Sepes.Common.Exceptions;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Provisioning.Service.Interface;
using System;
using System.Threading.Tasks;

namespace Sepes.Provisioning.Service
{
    public class OperationCheckService : IOperationCheckService
    {
        readonly ICloudResourceOperationReadService _cloudResourceOperationReadService;

        public OperationCheckService(ICloudResourceOperationReadService cloudResourceOperationReadService)
        {
            _cloudResourceOperationReadService = cloudResourceOperationReadService ?? throw new ArgumentNullException(nameof(cloudResourceOperationReadService));
        }

        public void ThrowIfTryCountExceededOrAborted(CloudResourceOperationDto operation)
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

        public void ThrowIfResourceIsDeletedAndOperationIsNotADelete(CloudResourceOperationDto operation)
        {
            if (operation.Resource.Deleted && operation.OperationType != CloudResourceOperationType.DELETE)
            {
                throw new ProvisioningException($"Resource is marked for deletion in database", newOperationStatus: CloudResourceOperationState.ABANDONED, proceedWithOtherOperations: false, deleteFromQueue: true, logAsWarning: true, includeExceptionInWarningLog: false);
            }
        }

        public void ThrowIfPossiblyInProgress(CloudResourceOperationDto operation)
        {
            if (operation.Updated.AddSeconds(30) >= DateTime.UtcNow && operation.Status == CloudResourceOperationState.IN_PROGRESS) //If updated less than xx minutes ago, probably in progress
            {
                throw new ProvisioningException($"Possibly allready in progress", proceedWithOtherOperations: false, deleteFromQueue: false, postponeQueueItemFor: 60, logAsWarning: true, includeExceptionInWarningLog: false);
            }
        }

        public async Task ThrowIfDependentOnUnfinishedOperationAsync(CloudResourceOperationDto operation, ProvisioningQueueParentDto queueParentItem)
        {
            if (operation.DependsOnOperationId.HasValue && !await _cloudResourceOperationReadService.OperationIsFinishedAndSucceededAsync(operation.DependsOnOperationId.Value))
            {
                var increaseBy = CloudResourceConstants.INCREASE_QUEUE_INVISIBLE_WHEN_DEPENDENT_ON_NOT_FINISHED;

                bool storeQueueInformationOnOperation = queueParentItem.Children.Count == 1;

                throw new ProvisioningException($"Dependant operation {operation.DependsOnOperationId.Value} is not finished. Invisibility increased by {increaseBy}", proceedWithOtherOperations: false, deleteFromQueue: false, postponeQueueItemFor: increaseBy, storeQueueInfoOnOperation: storeQueueInformationOnOperation, logAsWarning: true, includeExceptionInWarningLog: false);

            }
        }
    }
}
