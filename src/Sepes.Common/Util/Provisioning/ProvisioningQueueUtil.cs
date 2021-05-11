using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Service.Interface;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Util.Provisioning
{
    public static class ProvisioningQueueUtil
    {
        public  static async Task IncreaseInvisibleBasedOnResource(CloudResourceOperationDto currentOperation, ProvisioningQueueParentDto queueParentItem, IProvisioningQueueService queue)
        {
            var increaseBy = AzureResourceProivisoningTimeoutResolver.GetTimeoutForOperationInSeconds(currentOperation.Resource.ResourceType, currentOperation.OperationType);
            await queue.IncreaseInvisibilityAsync(queueParentItem, increaseBy);
        }

        public static async Task CreateItemAndEnqueue(IProvisioningQueueService workQueue, int operationId, string operationDescription)
        {
            var queueParentItem = new ProvisioningQueueParentDto();
            queueParentItem.Description = operationDescription;
            queueParentItem.Children.Add(new ProvisioningQueueChildDto() { ResourceOperationId = operationId });

            await workQueue.SendMessageAsync(queueParentItem);
        }

        public static async Task CreateItemAndEnqueue(IProvisioningQueueService workQueue, CloudResourceOperation operation)
        {
            await CreateItemAndEnqueue(workQueue, operation.Id, operation.Description);
        }

        public static async Task CreateItemAndEnqueue(IProvisioningQueueService workQueue, CloudResourceOperationDto operation)
        {
            await CreateItemAndEnqueue(workQueue, operation.Id, operation.Description);
        }

        public static void CreateChildAndAdd(ProvisioningQueueParentDto parent, CloudResourceOperationDto operation)
        {            
            parent.Children.Add(new ProvisioningQueueChildDto() { ResourceOperationId = operation.Id });
        }

        public static void CreateChildAndAdd(ProvisioningQueueParentDto parent, CloudResourceOperation operation)
        {
            parent.Children.Add(new ProvisioningQueueChildDto() { ResourceOperationId = operation.Id });
        }

        public static void CreateChildAndAdd(ProvisioningQueueParentDto parent, CloudResource resource)
        {
            var createOperation = CloudResourceOperationUtil.GetCreateOperation(resource);
            CreateChildAndAdd(parent, createOperation);                
        }
    }
}
