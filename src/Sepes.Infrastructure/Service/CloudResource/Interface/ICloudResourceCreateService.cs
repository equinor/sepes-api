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

        Task ValidateNameThrowIfInvalid(string resourceName);


        //MORE SPECIFIC RESOURCE OPERATIONS

        Task CreateSandboxResourceGroup(SandboxResourceCreationAndSchedulingDto dto);

        Task<CloudResourceDto> CreateVmEntryAsync(int sandboxId, CloudResource resourceGroup, Microsoft.Azure.Management.ResourceManager.Fluent.Core.Region region, Dictionary<string, string> tags, string vmName, int dependsOn, string configString);

     

    }
}
