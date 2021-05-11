using Microsoft.Azure.Management.Compute.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Azure.Service.Interface
{
    public interface IAzureResourceSkuService
    {
        Task<List<ResourceSku>> GetSKUsForRegion(string region, string resourceType = null, bool filterBasedOnResponseRestrictions = true, CancellationToken cancellationToken = default);
    }
}
