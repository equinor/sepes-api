using Sepes.Common.Dto;
using Sepes.Common.Dto.Sandbox;
using Sepes.Infrastructure.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Provisioning.Service.Interface
{
    public interface IProvisioningQueueService
    {
        Task<ProvisioningQueueParentDto> SendMessageAsync(ProvisioningQueueParentDto message, TimeSpan? visibilityTimeout = null, CancellationToken cancellationToken = default);

        Task<ProvisioningQueueParentDto> ReceiveMessageAsync();     

        Task DeleteMessageAsync(ProvisioningQueueParentDto message);

        Task DeleteMessageAsync(string messageId, string popReceipt);

     

        Task IncreaseInvisibilityAsync(ProvisioningQueueParentDto message, int invisibleForInSeconds);

        Task ReQueueMessageAsync(ProvisioningQueueParentDto message, int? invisibleForInSeconds = default, CancellationToken cancellationToken = default);
        Task AddNewQueueMessageForOperation(CloudResourceOperation operation);      
        void CreateChildAndAdd(ProvisioningQueueParentDto parent, CloudResourceOperation operation);       
        
        Task CreateItemAndEnqueue(int operationId, string operationDescription);
        Task IncreaseInvisibleBasedOnResource(CloudResourceOperationDto currentOperation, ProvisioningQueueParentDto queueParentItem);
    }
}
