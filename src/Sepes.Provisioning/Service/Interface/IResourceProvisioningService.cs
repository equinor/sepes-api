using Sepes.Common.Dto.Sandbox;
using System.Threading.Tasks;

namespace Sepes.Provisioning.Service.Interface
{
    public interface IResourceProvisioningService
    {
        Task DequeueAndHandleWork();

        Task HandleWork(ProvisioningQueueParentDto work);       
    }
}
