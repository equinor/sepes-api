using Sepes.Infrastructure.Dto.VirtualMachine;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IVirtualMachineDiskSizeService
    {        
        Task<List<VmDiskLookupDto>> AvailableDisks(CancellationToken cancellationToken = default);     
    }
}
