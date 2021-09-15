using System.Threading.Tasks;
using Sepes.Azure.Service.Interface;
using Sepes.Common.Dto;
using Sepes.Common.Dto.Sandbox;

namespace Sepes.Provisioning.Service.Interface
{
    public interface IFirewallService
    {
        bool CanHandle(CloudResourceOperationDto operation);

        Task Handle(
            ProvisioningQueueParentDto queueParentItem,
            CloudResourceOperationDto operation,
            IHasFirewallRules networkRuleService);
    }
}