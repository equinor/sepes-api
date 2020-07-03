using Sepes.Infrastructure.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public interface IAzureResourceService
    {
        
        Task<AzureResourceDto> Add(string resourceGroupId, string resourceGroupName, string type, string resourceId, string resourceName);

        Task<AzureResourceDto> AddResourceGroup(string resourceGroupId, string resourceGroupName, string type);
        Task<AzureResourceDto> GetByIdAsync(int id);

    

  
    }
}
