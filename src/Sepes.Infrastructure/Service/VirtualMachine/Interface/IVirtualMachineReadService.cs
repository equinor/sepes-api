using Sepes.Common.Dto.VirtualMachine;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IVirtualMachineReadService
    {
        Task<List<VmDto>> VirtualMachinesForSandboxAsync(int sandboxId, CancellationToken cancellationToken = default);

        Task<VmExtendedDto> GetExtendedInfo(int vmId, CancellationToken cancellationToken = default);

        Task<VmExternalLink> GetExternalLink(int vmId, CancellationToken cancellationToken = default);
    }
}
