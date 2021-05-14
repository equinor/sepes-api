using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sepes.Azure.Dto;

namespace Sepes.Azure.Service.Interface
{
    public interface IAzureDiskPriceService
    {
        Task<Dictionary<string, AzureDiskPriceForRegion>> GetDiskPrices(string region = null, CancellationToken cancellationToken = default);
    }
}