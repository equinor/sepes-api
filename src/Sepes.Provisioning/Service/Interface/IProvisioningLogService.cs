using System;
using Sepes.Common.Dto;
using Sepes.Common.Dto.Sandbox;

namespace Sepes.Provisioning.Service.Interface
{
    public interface IProvisioningLogService
    {
        void HandlingQueueParent(ProvisioningQueueParentDto queueParentItem);
        void QueueParentProgressInformation(ProvisioningQueueParentDto queueParentItem, string suffix);
        void OperationInformation(CloudResourceOperationDto currentResourceOperation, string suffix);
        void OperationWarning(CloudResourceOperationDto currentResourceOperation, string suffix, Exception exeption = null);
        void OperationError(Exception exeption, CloudResourceOperationDto currentResourceOperation, string suffix);
        void QueueParentProgressWarning(ProvisioningQueueParentDto queueParentItem, string suffix);
        void QueueParentProgressError(ProvisioningQueueParentDto queueParentItem, string suffix, Exception exeption = null);
        void QueueParentProgressCritical(ProvisioningQueueParentDto queueParentItem, string suffix);
    }
}