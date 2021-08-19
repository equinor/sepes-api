using Sepes.Azure.Service.Interface;
using Sepes.Common.Dto;
using Sepes.Common.Dto.Sandbox;
using System.Threading.Tasks;

namespace Sepes.Provisioning.Service.Interface
{
    public interface ITagProvisioningService
    {
        bool CanHandle(CloudResourceOperationDto operation);

        Task Handle(ProvisioningQueueParentDto queueParentItem, CloudResourceOperationDto operation, IServiceForTaggedResource tagService);
    }
}