using Sepes.Infrastructure.Dto.Sandbox;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IProvisioningQueueService
    {
        Task<ProvisioningQueueParentDto> SendMessageAsync(ProvisioningQueueParentDto message, TimeSpan? visibilityTimeout = null, CancellationToken cancellationToken = default);

        Task<ProvisioningQueueParentDto> RecieveMessageAsync();     

        Task DeleteMessageAsync(ProvisioningQueueParentDto message);

        Task DeleteMessageAsync(string messageId, string popReceipt);

        Task DeleteQueueAsync();

        Task IncreaseInvisibilityAsync(ProvisioningQueueParentDto message, int invisibleForInSeconds);

        Task ReQueueMessageAsync(ProvisioningQueueParentDto message, int? invisibleForInSeconds = default, CancellationToken cancellationToken = default);
    }
}
