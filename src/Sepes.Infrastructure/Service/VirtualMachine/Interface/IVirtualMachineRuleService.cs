using Sepes.Common.Dto.VirtualMachine;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IVirtualMachineRuleService
    { 
        Task<List<VmRuleDto>> SetRules(int vmId, List<VmRuleDto> currentRules, CancellationToken cancellationToken = default);

        Task<List<VmRuleDto>> GetRules(int vmId, CancellationToken cancellationToken = default);

        Task<VmRuleDto> GetRuleById(int vmId, string ruleId, CancellationToken cancellationToken = default);
       
        Task<bool> IsInternetVmRuleSetToDeny(int vmId);
        Task<VmRuleDto> GetInternetRule(int vmId);
        bool IsRuleSetToDeny(VmRuleDto rule);
    }
}
