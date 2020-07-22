using Microsoft.Azure.Management.Network.Models;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Sepes.Infrastructure.Dto;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public interface ISandboxResourceService
    {   
        Task<SandboxResourceDto> Add(int sandboxId, string resourceGroupId, string resourceGroupName, string type, string resourceId, string resourceName);
        Task<SandboxResourceDto> Add(int sandboxId, string resourceGroupId, string resourceGroupName, Microsoft.Azure.Management.Network.Models.Resource resourceGroup);
        Task<SandboxResourceDto> Add(int sandboxId, string resourceGroupId, string resourceGroupName, IResource resource);
        Task<SandboxResourceDto> AddResourceGroup(int sandboxId, string resourceGroupId, string resourceGroupName, string type);
        Task<SandboxResourceDto> GetByIdAsync(int id);
        Task<SandboxResourceDto> MarkAsDeletedByIdAsync(int id);
        Task<SandboxResourceDto> Update(int resourceId, IResourceGroup updated);
        Task<SandboxResourceDto> Update(int resourceId, IResource updated);
    }
}
