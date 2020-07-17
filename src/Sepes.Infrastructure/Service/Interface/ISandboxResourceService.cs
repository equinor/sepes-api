using Microsoft.Azure.Management.Network.Models;
using Sepes.Infrastructure.Dto;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public interface ISandboxResourceService
    {
        
        Task<SandboxResourceDto> Add(string resourceGroupId, string resourceGroupName, string type, string resourceId, string resourceName);

        Task<SandboxResourceDto> Add(string resourceGroupId, string resourceGroupName, Resource resource);

        Task<SandboxResourceDto> AddResourceGroup(string resourceGroupId, string resourceGroupName, string type);
        Task<SandboxResourceDto> GetByIdAsync(int id);

        Task<SandboxResourceDto> MarkAsDeletedByIdAsync(int id);


    }
}
