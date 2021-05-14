using System.Threading.Tasks;
using Sepes.Azure.Service.Interface;
using Sepes.Common.Dto;

namespace Sepes.Provisioning.Service.Interface
{
    public interface IFirewallService
    {
        bool CanHandle(CloudResourceOperationDto operation);

        Task Handle(
            CloudResourceOperationDto operation,
            IHasFirewallRules networkRuleService);
    }
}