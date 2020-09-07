using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.Storage.Fluent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public interface IAzureStorageAccountService : IHasProvisioningState, IHasTags, IHasExists
    {
        Task<IStorageAccount> CreateStorageAccount(Region region, string sandboxName, string resourceGroupName, Dictionary<string, string> tags);
        Task<IStorageAccount> CreateDiagnosticsStorageAccount(Region region, string sandboxName, string resourceGroupName, Dictionary<string, string> tags);
        Task DeleteStorageAccount(string resourceGroupName, string storageAccountName);

        // CreateStorageContainer(type);
        // DeleteStoragecontainer(type);

        // Methods for accessing storageContainers:
    }
}
