using Sepes.Azure.Service.Interface;
using Sepes.Common.Dto;
using System.Threading.Tasks;

namespace Sepes.Provisioning.Service.Interface
{
    public interface ITagProvisioningService
    {
        bool CanHandle(CloudResourceOperationDto operation);

        Task Handle(CloudResourceOperationDto operation, IServiceForTaggedResource tagService);
    }
}