using System.Threading.Tasks;
using Sepes.Common.Dto;
using Sepes.Common.Dto.Provisioning;
using Sepes.Common.Interface.Service;

namespace Sepes.Provisioning.Service.Interface
{
    public interface ICreateAndUpdateService
    {
        bool CanHandle(CloudResourceOperationDto operation);

        Task<ResourceProvisioningResult> Handle(
            CloudResourceOperationDto operation,
            ResourceProvisioningParameters currentCrudInput,
            IPerformResourceProvisioning provisioningService
        );
    }
}