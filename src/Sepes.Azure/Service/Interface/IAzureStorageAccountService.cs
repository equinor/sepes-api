using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Sepes.Common.Interface.Service;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sepes.Azure.Dto;

namespace Sepes.Azure.Service.Interface
{
    public interface IAzureStorageAccountService : IHasProvisioningState, IServiceForTaggedResource, IPerformResourceProvisioning
    {
        Task Delete(string resourceGroupName, string storageAccountName, CancellationToken cancellationToken = default);

        Task<AzureStorageAccountDto> Create(Region region, string resourceGroupName, string storageAccountName, Dictionary<string, string> tags, List<string> onlyAllowAccessFrom = null, CancellationToken cancellationToken = default);
    }
}
