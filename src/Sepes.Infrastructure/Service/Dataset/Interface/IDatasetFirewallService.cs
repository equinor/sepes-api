using Sepes.Infrastructure.Model;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IDatasetFirewallService
    {
        Task EnsureDatasetHasFirewallRules(Dataset dataset, string clientIp);
        Task<bool> SetDatasetFirewallRules(Dataset dataset, string clientIp);
    }
}
