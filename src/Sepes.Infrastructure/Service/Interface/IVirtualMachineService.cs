using Sepes.Infrastructure.Dto.VirtualMachine;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IVirtualMachineService
    {
        Task<VmDto> CreateAsync(int sandboxId, CreateVmUserInputDto newSandbox);

        Task<VmDto> UpdateAsync(int sandboxId, CreateVmUserInputDto newSandbox);

        Task<VmDto> DeleteAsync(int id);

        Task<List<VmDto>> VirtualMachinesForSandboxAsync(int sandboxId, CancellationToken cancellationToken = default(CancellationToken));

        Task<VmExtendedDto> GetExtendedInfo(int vmId);

        string CalculateName(string studyName, string sandboxName, string userPrefix);

        Task<List<VmSizeLookupDto>> AvailableSizes(int sandboxId, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<VmDiskLookupDto>> AvailableDisks();


        Task<List<VmOsDto>> AvailableOperatingSystems(int sandboxId, CancellationToken cancellationToken = default(CancellationToken));
        Task<List<VmOsDto>> AvailableOperatingSystems(string region, CancellationToken cancellationToken = default(CancellationToken));

        Task<double> CalculatePrice(int sandboxId, CalculateVmPriceUserInputDto userInput);
        
        //RULES
        
        Task<VmRuleDto> AddRule(int vmId, VmRuleDto input, CancellationToken cancellationToken = default(CancellationToken));

        Task<VmRuleDto> UpdateRule(int vmId, VmRuleDto input, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<VmRuleDto>> GetRules(int vmId, CancellationToken cancellationToken = default(CancellationToken));

        Task<VmRuleDto> GetRuleById(int vmId, string ruleId, CancellationToken cancellationToken = default(CancellationToken));

        Task<VmRuleDto> DeleteRule(int vmId, string ruleId, CancellationToken cancellationToken = default(CancellationToken));
    }
}
