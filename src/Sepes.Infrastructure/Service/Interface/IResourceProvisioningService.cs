using Sepes.Infrastructure.Dto.Sandbox;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IResourceProvisioningService
    {
        Task DequeueWorkAndPerformIfAny();

        Task HandleQueueItem(ProvisioningQueueParentDto work);       
    }
}
