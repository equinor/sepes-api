using Sepes.Common.Dto.VirtualMachine;
using Sepes.Infrastructure.Dto;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IVirtualMachineOperatingSystemService
    {  
        Task<IEnumerable<VmOsDto>> AvailableOperatingSystems(int sandboxId, CancellationToken cancellationToken = default);
        Task<IEnumerable<VmOsDto>> AvailableOperatingSystems(string region, CancellationToken cancellationToken = default);
        Task<VmImageDto> GetImage(int id);
    }
}
