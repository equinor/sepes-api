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

        Task DeleteAsync(int id);

        Task<List<VmDto>> VirtualMachinesForSandboxAsync(int sandboxId, CancellationToken cancellationToken = default);

        Task<VmExtendedDto> GetExtendedInfo(int vmId, CancellationToken cancellationToken = default);

        Task<VmExternalLink> GetExternalLink(int vmId);

        string ValidateVmPassword(string password);
    }
}
