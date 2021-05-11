using System.Threading.Tasks;
using Sepes.Azure.Service.Interface;
using Sepes.Common.Dto;

namespace Sepes.Provisioning.Service.Interface
{
    public interface ICorsRuleProvisioningService
    {
        bool CanHandle(CloudResourceOperationDto operation);

        Task Handle(
            CloudResourceOperationDto operation,
            IHasCorsRules corsRuleService);
    }
}