using Sepes.Common.Dto.VirtualMachine;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Azure.Service.Interface
{
    public interface IAzureVirtualNetworkOperatingSystemService
    {
        Task<List<VmOsDto>> GetAvailableOperatingSystemsAsync(string region, CancellationToken cancellationToken = default);
    }
}
