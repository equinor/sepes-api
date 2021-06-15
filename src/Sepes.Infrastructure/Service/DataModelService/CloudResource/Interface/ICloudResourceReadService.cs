using Sepes.Common.Constants;
using Sepes.Infrastructure.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService.Interface
{
    public interface ICloudResourceReadService
    {
        Task<CloudResource> GetByIdAsync(int id, UserOperation operation);

        Task<CloudResource> GetByIdNoAccessCheckAsync(int id);

        Task<CloudResource> GetByStudyIdForDeletionNoAccessCheckAsync(int id);

        Task<List<int>> GetSandboxResourceGroupIdsForStudy(int studyId);

        Task<List<int>> GetDatasetResourceGroupIdsForStudy(int studyId);
       

        Task<IEnumerable<CloudResource>> GetDeletedResourcesAsync();

        Task<List<CloudResource>> GetAllActiveResources();

        Task<bool> ResourceIsDeleted(int resourceId);
        Task<List<CloudResource>> GetSandboxResourcesForDeletion(int sandboxId);
        Task<List<int>> GetDatasetStorageAccountIdsForStudy(int studyId);
    }
}
