using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface ISandboxResourceService
    {

        Task<SandboxResource> GetByIdAsync(int id);
        Task<SandboxResourceDto> GetDtoByIdAsync(int id);

        Task<IEnumerable<SandboxResource>> GetDeletedResourcesAsync();

        Task<List<SandboxResource>> GetAllActiveResources();


        Task<bool> ResourceIsDeleted(int resourceId);

        Task<List<SandboxResourceDto>> GetSandboxResources(int sandboxId, CancellationToken cancellation = default);

        Task<List<SandboxResourceLightDto>> GetSandboxResourcesLight(int sandboxId);

    }
}
