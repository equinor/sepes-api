using Sepes.Infrastructure.Dto;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Provisioning.Interface
{
    public interface IRoleProvisioningService
    {
        bool CanHandle(CloudResourceOperationDto operation);

        Task Handle(CloudResourceOperationDto operation);
    }
}
