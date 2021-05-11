using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Azure.Service.Interface
{
    public interface IAzureCostManagementService
    {
        Task<double> GetVmPrice(string region, string size, CancellationToken cancellationToken = default);   
    }
}
