using Sepes.Infrastructure.Dto.VirtualMachine;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService.Interface
{
    public interface IVirtualMachineDiskSizeService
    {        
        Task<IEnumerable<VmDiskLookupDto>> AvailableDisks(int sandboxId, CancellationToken cancellationToken = default);     
    }
}
