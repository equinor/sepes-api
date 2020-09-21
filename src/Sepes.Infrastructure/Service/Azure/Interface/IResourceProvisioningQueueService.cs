using Sepes.Infrastructure.Dto;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public interface IResourceProvisioningQueueService
    {
        Task SendMessageAsync(ProvisioningQueueParentDto message);

        Task<ProvisioningQueueParentDto> RecieveMessageAsync();     

        Task DeleteMessageAsync(ProvisioningQueueParentDto message);

        Task DeleteQueueAsync();

        Task IncreaseInvisibilityAsync(ProvisioningQueueParentDto message, int invisibleForInSeconds);
    }
}
