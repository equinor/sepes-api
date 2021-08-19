using System.Threading.Tasks;
using Sepes.Azure.Service.Interface;
using Sepes.Common.Dto;
using Sepes.Common.Dto.Sandbox;

namespace Sepes.Provisioning.Service.Interface
{
    public interface ICorsRuleProvisioningService
    {
        bool CanHandle(CloudResourceOperationDto operation);

        Task Handle(
             ProvisioningQueueParentDto queueParentItem,
            CloudResourceOperationDto operation,
            IHasCorsRules corsRuleService);
    }
}