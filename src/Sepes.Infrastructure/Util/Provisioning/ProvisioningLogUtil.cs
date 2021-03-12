using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Sandbox;

namespace Sepes.Infrastructure.Util.Provisioning
{
    public static class ProvisioningLogUtil
    {
        public static string HandlingQueueParent(ProvisioningQueueParentDto queueParentItem)
        {
            return $"Handling: {queueParentItem.MessageId} - {queueParentItem.Description}";
        }

        public static string QueueParentProgress(ProvisioningQueueParentDto queueParentItem, string suffix)
        {
            return $"MessageId: {queueParentItem.MessageId} - {queueParentItem.Description} | {suffix}";
        }

        public static string Operation(CloudResourceOperationDto currentResourceOperation, string suffix)
        {
            return $"{currentResourceOperation.Resource.SandboxName} | {currentResourceOperation.Id} | {currentResourceOperation.Resource.ResourceType} | {currentResourceOperation.OperationType.ToUpper()} | attempt ({currentResourceOperation.TryCount}/{currentResourceOperation.MaxTryCount}) | {suffix}";
        }
    }
}
