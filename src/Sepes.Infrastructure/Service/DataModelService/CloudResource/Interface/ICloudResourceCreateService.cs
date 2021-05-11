using Sepes.Common.Dto.Sandbox;
using Sepes.Infrastructure.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService.Interface
{
    public interface ICloudResourceCreateService
    {       
        Task ValidateThatNameDoesNotExistThrowIfInvalid(string resourceName);

        //MORE SPECIFIC RESOURCE OPERATIONS
        Task<CloudResource> CreateStudySpecificResourceGroupEntryAsync(int studyId, string resourceGroupName, string region, Dictionary<string, string> tags);

        Task<CloudResource> CreateStudySpecificDatasetEntryAsync(int datasetId, int resourceGroupEntryId, string region, string resourceGroupName, string resourceName, Dictionary<string, string> tags);
        Task<CloudResource> CreateSandboxResourceGroupEntryAsync(SandboxResourceCreationAndSchedulingDto dto, string resourceGroupName);
        Task<CloudResource> CreateSandboxResourceEntryAsync(SandboxResourceCreationAndSchedulingDto dto, string resourceType, string resourceName, string configString = null, int dependsOn = 0);
        Task<CloudResource> CreateVmEntryAsync(int sandboxId, CloudResource resourceGroup, string region, Dictionary<string, string> tags, string vmName, int dependsOn, string configString);       
        
    }
}
