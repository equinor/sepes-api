using Microsoft.Azure.Management.Compute.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure.Interface
{
    public interface IAzureResourceSkuService
    {
        Task<List<ResourceSku>> GetSKUsForRegion(string region, string resourceType = null, CancellationToken cancellationToken = default);
    }
}
