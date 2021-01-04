using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure.Interface
{
    public interface IAzureCostManagementService
    {
        Task<double> GetVmPrice(string region, string size, CancellationToken cancellationToken = default(CancellationToken));
        Task<double> GetSizePrice(string size, CancellationToken cancellationToken = default(CancellationToken));
    }
}
