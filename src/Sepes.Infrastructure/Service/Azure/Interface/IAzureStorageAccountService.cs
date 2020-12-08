using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.Storage.Fluent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure.Interface
{
    public interface IAzureStorageAccountService : IHasProvisioningState, IHasTags, IPerformCloudResourceCRUD
    {
        //Task<IStorageAccount> CreateStorageAccount(Region region, string sandboxName, string resourceGroupName, Dictionary<string, string> tags);
        //Task<IStorageAccount> CreateDiagnosticsStorageAccount(Region region, string sandboxName, string resourceGroupName, Dictionary<string, string> tags);
        Task DeleteStorageAccount(string resourceGroupName, string storageAccountName, CancellationToken cancellationToken = default);

        Task<IStorageAccount> CreateStorageAccount(Region region, string resourceGroupName, string storageAccountName, Dictionary<string, string> tags, List<string> onlyAllowAccessFrom = null, CancellationToken cancellationToken = default);

        Task<IStorageAccount> SetStorageAccountAllowedIPs(string resourceGroupName, string storageAccountName, List<string> onlyAllowAccessFrom = null, CancellationToken cancellationToken = default);
    }
}
