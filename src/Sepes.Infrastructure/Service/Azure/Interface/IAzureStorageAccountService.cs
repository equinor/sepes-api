using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Sepes.Infrastructure.Dto.Azure;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure.Interface
{
    public interface IAzureStorageAccountService : IHasProvisioningState, IHasTags, IPerformResourceProvisioning
    {
        Task Delete(string resourceGroupName, string storageAccountName, CancellationToken cancellationToken = default);

        Task<AzureStorageAccountDto> Create(Region region, string resourceGroupName, string storageAccountName, Dictionary<string, string> tags, List<string> onlyAllowAccessFrom = null, CancellationToken cancellationToken = default);


      
    }
}
