using Sepes.Infrastructure.Dto;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IProvisioningQueueService
    {
        Task SendMessageAsync(ProvisioningQueueParentDto message);

        Task<ProvisioningQueueParentDto> RecieveMessageAsync();     

        Task DeleteMessageAsync(ProvisioningQueueParentDto message);

        Task DeleteQueueAsync();

        Task IncreaseInvisibilityAsync(ProvisioningQueueParentDto message, int invisibleForInSeconds);
    }
}
