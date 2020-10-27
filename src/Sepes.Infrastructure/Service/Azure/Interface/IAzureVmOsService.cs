using Sepes.Infrastructure.Dto.VirtualMachine;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure.Interface
{
    public interface IAzureVmOsService
    {
        Task<List<VmOsDto>> GetAvailableOperatingSystemsAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
