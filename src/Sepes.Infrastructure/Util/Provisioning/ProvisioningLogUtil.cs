using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Sandbox;

namespace Sepes.Infrastructure.Util.Provisioning
{
    public static class ProvisioningLogUtil
    {
        public static string QueueParent(ProvisioningQueueParentDto queueParentItem)
        {
            return $"Handling: {queueParentItem.MessageId} - {queueParentItem.Description}";
        }

        public static string Operation(CloudResourceOperationDto currentResourceOperation, string suffix)
        {
            return $"{currentResourceOperation.Id} | {currentResourceOperation.Resource.ResourceType} | {currentResourceOperation.OperationType.ToUpper()} | attempt ({currentResourceOperation.TryCount}/{currentResourceOperation.MaxTryCount}) | ";
        }
    }
}
