using System.Threading.Tasks;
using Sepes.Common.Dto;
using Sepes.Common.Dto.Provisioning;
using Sepes.Common.Dto.Sandbox;
using Sepes.Common.Interface.Service;

namespace Sepes.Provisioning.Service.Interface
{
    public interface IDeleteOperationService
    {
        bool CanHandle(CloudResourceOperationDto operation);

        Task<ResourceProvisioningResult> Handle(
             ProvisioningQueueParentDto queueParentItem,
            CloudResourceOperationDto operation,
            ResourceProvisioningParameters currentCrudInput,
            IPerformResourceProvisioning provisioningService
        );
    }
}