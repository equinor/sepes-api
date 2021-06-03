using Sepes.Azure.Dto;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Azure.Service.Interface
{
    public interface IAzureResourceSkuService
    {
        Task<List<AzureResourceSku>> GetSKUsForRegion(string region, string resourceType = null, bool filterBasedOnResponseRestrictions = true, CancellationToken cancellationToken = default);
    }
}
