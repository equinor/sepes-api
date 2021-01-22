using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface ICloudResourceCreateService
    {
        //GENERAL METHODS
        Task<CloudResourceDto> Create(SandboxResourceCreationAndSchedulingDto dto, string type, string resourceName, bool sandboxControlled = true, string configString = null, int dependsOn = 0);

        Task ValidateThatNameDoesNotExistThrowIfInvalid(string resourceName);

        //MORE SPECIFIC RESOURCE OPERATIONS
        Task<CloudResourceDto> CreateStudySpecificResourceGroupEntryAsync(int studyId, string resourceGroupName, string region, Dictionary<string, string> tags);
        Task<CloudResourceDto> CreateSandboxResourceGroupEntryAsync(SandboxResourceCreationAndSchedulingDto dto, string resourceGroupName);
        Task<CloudResourceDto> CreateVmEntryAsync(int sandboxId, CloudResource resourceGroup, string region, Dictionary<string, string> tags, string vmName, int dependsOn, string configString);

    }
}
