using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Sepes.Infrastructure.Dto.Azure;
using Sepes.Infrastructure.Service.Azure.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public interface IAzureResourceGroupService : IHasProvisioningState, IHasTags, IPerformCloudResourceCRUD
    {
        Task<AzureResourceGroupDto> Create(string resourceGroupName, Region region, Dictionary<string, string> tags);

        Task Delete(string resourceGroupName);

        //Task<bool> Exists(string resourceGroupName);

        Task<string> GetProvisioningState(string resourceGroupName);   
    }
}
