using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure.Interface
{
    public interface IAzureDiskPriceService
    {
        Task<double> GetDiskPrice(string region, string size, CancellationToken cancellationToken = default(CancellationToken));
    }
}
