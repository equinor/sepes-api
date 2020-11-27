using Sepes.Infrastructure.Dto.Sandbox;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IProvisioningQueueService
    {
        Task SendMessageAsync(ProvisioningQueueParentDto message, TimeSpan? visibilityTimeout = null, CancellationToken cancellationToken = default(CancellationToken));

        Task<ProvisioningQueueParentDto> RecieveMessageAsync();     

        Task DeleteMessageAsync(ProvisioningQueueParentDto message);

        Task DeleteQueueAsync();

        Task IncreaseInvisibilityAsync(ProvisioningQueueParentDto message, int invisibleForInSeconds);

        Task ReQueueMessageAsync(ProvisioningQueueParentDto message, CancellationToken cancellationToken = default);
    }
}
