using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Sepes.Infrastructure.Dto.Azure;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure.Interface
{
    public interface IAzureStorageAccountService : IHasProvisioningState, IHasTags, IPerformCloudResourceCRUD
    {
        Task DeleteStorageAccount(string resourceGroupName, string storageAccountName, CancellationToken cancellationToken = default);

        Task<AzureStorageAccountDto> CreateStorageAccount(Region region, string resourceGroupName, string storageAccountName, Dictionary<string, string> tags, List<string> onlyAllowAccessFrom = null, CancellationToken cancellationToken = default);

        Task<AzureStorageAccountDto> SetStorageAccountAllowedIPs(string resourceGroupName, string storageAccountName, List<string> onlyAllowAccessFrom = null, CancellationToken cancellationToken = default);
        Task AddStorageAccountToVNet(string resourceGroupForStorageAccount, string storageAccountName, string resourceGroupForVnet, string vNetName, CancellationToken cancellation);
        Task RemoveStorageAccountFromVNet(string resourceGroupForStorageAccount, string storageAccountName, string resourceGroupForVnet, string vNetName, CancellationToken cancellation);
    }
}
