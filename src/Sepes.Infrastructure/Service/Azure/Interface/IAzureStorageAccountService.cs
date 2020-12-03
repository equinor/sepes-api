using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.Storage.Fluent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure.Interface
{
    public interface IAzureStorageAccountService : IHasProvisioningState, IHasTags, IPerformCloudResourceCRUD
    {
        Task<IStorageAccount> CreateStorageAccount(Region region, string sandboxName, string resourceGroupName, Dictionary<string, string> tags);
        //Task<IStorageAccount> CreateDiagnosticsStorageAccount(Region region, string sandboxName, string resourceGroupName, Dictionary<string, string> tags);
        Task DeleteStorageAccount(string resourceGroupName, string storageAccountName);

        Task<IStorageAccount> CreateStorageAccount(Region region, string resourceGroupName, string name, Dictionary<string, string> tags, CancellationToken cancellationToken = default);

        // CreateStorageContainer(type);
        // DeleteStoragecontainer(type);

        // Methods for accessing storageContainers:
    }
}
