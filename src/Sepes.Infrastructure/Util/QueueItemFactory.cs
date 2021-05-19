using Sepes.Common.Dto;
using Sepes.Common.Dto.Sandbox;
using Sepes.Infrastructure.Model;

namespace Sepes.Infrastructure.Util
{
    public static class QueueItemFactory
    {
        public static ProvisioningQueueParentDto CreateParent(string description)
        {
            var queueParentItem = new ProvisioningQueueParentDto
            {
                Description = description
            };

            return queueParentItem;
        }

        public static ProvisioningQueueParentDto CreateParent(int operationId, string operationDescription)
        {
            var queueParentItem = CreateParent(operationDescription);

            queueParentItem.Children.Add(new ProvisioningQueueChildDto() { ResourceOperationId = operationId });
            return queueParentItem;
        }

        public static ProvisioningQueueParentDto CreateParent(CloudResourceOperation operation)
        {
            return CreateParent(operation.Id, operation.Description);
        }

        public static ProvisioningQueueParentDto CreateParent(CloudResourceOperationDto operation)
        {
            return CreateParent(operation.Id, operation.Description);
        }
    }
}
