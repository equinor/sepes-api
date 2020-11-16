using Microsoft.Azure.Management.Compute.Models;
using Sepes.Infrastructure.Dto.VirtualMachine;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure.Interface
{
    public interface IAzureVmSizeService
    {
        Task<List<VmSizeDto>> GetAvailableSizesAsync(string region, CancellationToken cancellationToken = default);

        Task<IEnumerable<VirtualMachineSize>> GetAvailableVmSizesAsync(string region, CancellationToken cancellationToken = default);
    }
}
