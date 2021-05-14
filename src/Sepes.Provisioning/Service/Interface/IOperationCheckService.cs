using Sepes.Common.Dto;
using Sepes.Common.Dto.Sandbox;
using System.Threading.Tasks;

namespace Sepes.Provisioning.Service.Interface
{
   public interface IOperationCheckService
    {
        Task ThrowIfDependentOnUnfinishedOperationAsync(CloudResourceOperationDto operation, ProvisioningQueueParentDto queueParentItem);
        void ThrowIfPossiblyInProgress(CloudResourceOperationDto operation);
        void ThrowIfResourceIsDeletedAndOperationIsNotADelete(CloudResourceOperationDto operation);
        void ThrowIfTryCountExceededOrAborted(CloudResourceOperationDto operation);
    }
}
