using Sepes.Common.Dto;
using System.Threading.Tasks;

namespace Sepes.Provisioning.Service.Interface
{
    public interface IRoleProvisioningService
    {
        bool CanHandle(CloudResourceOperationDto operation);

        Task Handle(CloudResourceOperationDto operation);
    }
}
