using Sepes.Common.Dto.VirtualMachine;
using Sepes.Infrastructure.Dto;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IVirtualMachineOperatingSystemService
    {  
        Task<IEnumerable<VmOsDto>> AvailableOperatingSystems(int sandboxId);    
        Task<VmImageDto> GetImage(int id);
    }
}
