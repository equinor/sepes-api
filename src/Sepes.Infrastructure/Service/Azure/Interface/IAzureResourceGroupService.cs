using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Sepes.Infrastructure.Dto.Azure;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure.Interface
{
    public interface IAzureResourceGroupService : IHasProvisioningState, IHasTags, IPerformCloudResourceCRUD
    {
        Task<AzureResourceGroupDto> Create(string resourceGroupName, Region region, Dictionary<string, string> tags);

        Task Delete(string resourceGroupName, CancellationToken cancellationToken = default);
        Task<AzureResourceGroupDto> EnsureCreated(string resourceGroupName, Region region, Dictionary<string, string> tags, CancellationToken cancellationToken = default);

        //Task<bool> Exists(string resourceGroupName);

        Task<string> GetProvisioningState(string resourceGroupName);   
    }
}
