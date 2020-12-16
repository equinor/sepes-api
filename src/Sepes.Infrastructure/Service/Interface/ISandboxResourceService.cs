using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface ISandboxResourceService
    {

        Task<SandboxResource> GetByIdAsync(int id);
        Task<SandboxResourceDto> GetDtoByIdAsync(int id);

        Task<IEnumerable<SandboxResource>> GetDeletedResourcesAsync();

        Task<List<SandboxResource>> GetActiveResources();


        Task<bool> ResourceIsDeleted(int resourceId);


        Task<List<SandboxResourceLightDto>> GetSandboxResources(int sandboxId);

    }
}
