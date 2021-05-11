using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Azure.Service.Interface
{
    public interface IAzureStorageAccountNetworkRuleService : IHasFirewallRules
    {
        Task AddStorageAccountToVNet(string resourceGroupForStorageAccount, string storageAccountName, string resourceGroupForVnet, string vNetName, CancellationToken cancellation);
        Task RemoveStorageAccountFromVNet(string resourceGroupForStorageAccount, string storageAccountName, string resourceGroupForVnet, string vNetName, CancellationToken cancellation);
    }
}
