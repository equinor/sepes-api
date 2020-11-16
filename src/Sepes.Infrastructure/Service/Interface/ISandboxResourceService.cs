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

        Task<SandboxResourceDto> UpdateResourceIdAndName(int resourceId, string azureId, string azureName);

        Task CreateSandboxResourceGroup(SandboxResourceCreationAndSchedulingDto dto);    

        Task<SandboxResourceDto> Create(SandboxResourceCreationAndSchedulingDto dto, string type, string resourceName, bool sandboxControlled = true, string configString = null, int dependsOn = 0);

        Task<SandboxResource> GetByIdAsync(int id);
        Task<SandboxResourceDto> GetDtoByIdAsync(int id);
        Task<SandboxResourceDto> MarkAsDeletedAndScheduleDeletion(int resourceId);      

        Task<SandboxResourceDto> UpdateResourceGroup(int resourceId, SandboxResourceDto updated);
        Task<SandboxResourceDto> Update(int resourceId, SandboxResourceDto updated);

        Task<IEnumerable<SandboxResource>> GetDeletedResourcesAsync();

        Task<List<SandboxResource>> GetActiveResources();

        Task UpdateProvisioningState(int resourceId, string newProvisioningState);
        Task<SandboxResourceDto> CreateVmEntryAsync(int sandboxId, SandboxResource resourceGroup, Microsoft.Azure.Management.ResourceManager.Fluent.Core.Region region, Dictionary<string, string> tags, string vmName, int dependsOn, string configString);

        Task ValidateNameThrowIfInvalid(string resourceName);

        Task<bool> ResourceIsDeleted(int resourceId);
    }
}
