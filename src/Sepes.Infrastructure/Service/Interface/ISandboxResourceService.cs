using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface ISandboxResourceService
    {

        Task<SandboxResourceDto> UpdateMissingDetailsAfterCreation(int resourceId, string azureId, string azureName);

        Task CreateSandboxResourceGroup(SandboxResourceCreationAndSchedulingDto dto);
        Task<SandboxResourceDto> Create(SandboxResourceCreationAndSchedulingDto dto, string type, string resourceName);

        Task<SandboxResourceDto> GetByIdAsync(int id);
        Task<SandboxResourceDto> MarkAsDeletedByIdAsync(int id);
        Task<SandboxResourceDto> UpdateResourceGroup(int resourceId, SandboxResourceDto updated);
        Task<SandboxResourceDto> Update(int resourceId, SandboxResourceDto updated);

        Task<IEnumerable<SandboxResource>> GetDeletedResourcesAsync();

        Task<List<SandboxResource>> GetActiveResources();

        Task UpdateProvisioningState(int resourceId, string newProvisioningState);

    }
}
