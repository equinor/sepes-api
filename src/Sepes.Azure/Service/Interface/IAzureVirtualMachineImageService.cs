using Microsoft.Azure.Management.Compute.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Azure.Service.Interface
{
    public interface IAzureVirtualMachineImageService
    {
        Task<List<VirtualMachineImageResource>> GetImagesAsync(string region, string publisher, string offer, string sku, CancellationToken cancellationToken = default);
    }
}
