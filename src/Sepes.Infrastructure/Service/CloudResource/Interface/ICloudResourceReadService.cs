using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface ICloudResourceReadService
    {

        Task<CloudResource> GetByIdAsync(int id);
        Task<CloudResourceDto> GetDtoByIdAsync(int id);

        Task<IEnumerable<CloudResource>> GetDeletedResourcesAsync();

        Task<List<CloudResource>> GetAllActiveResources();


        Task<bool> ResourceIsDeleted(int resourceId);

        Task<List<CloudResourceDto>> GetSandboxResources(int sandboxId, CancellationToken cancellation = default);

        Task<List<SandboxResourceLightDto>> GetSandboxResourcesLight(int sandboxId);

        Task<string> GetSandboxCostanlysis(int sandboxId, CancellationToken cancellation = default);

    }
}
