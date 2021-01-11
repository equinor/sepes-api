using Sepes.Infrastructure.Dto.Azure;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure.Interface
{
    public interface IAzureDiskPriceService
    {
        Task<Dictionary<string, AzureDiskPriceForRegion>> GetDiskPrices(string region = null, CancellationToken cancellationToken = default);
    }
}