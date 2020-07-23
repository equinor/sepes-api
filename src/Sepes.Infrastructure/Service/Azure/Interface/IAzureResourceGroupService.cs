using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public interface IAzureResourceGroupService : IHasProvisioningState, IHasTags
    {
        //Task<IResourceGroup> CreateForStudy(string studyName, string sandboxName, Region region, Dictionary<string, string> tags);

        Task<IResourceGroup> Create(string resourceGroupName, Region region, Dictionary<string, string> tags);

        Task Delete(string resourceGroupName);

        Task<bool> Exists(string resourceGroupName);

        Task<string> GetProvisioningState(string resourceGroupName);

        Task<IPagedCollection<IResourceGroup>> GetResourceGroupsAsList();
    }
}
