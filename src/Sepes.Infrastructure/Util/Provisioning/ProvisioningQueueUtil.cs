using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Sandbox;
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
    }
}
