using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Model;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService.Interface
{
    public interface IStudyModelService
    {
        Task<Study> GetByIdAsync(int studyId, UserOperation userOperation, bool withIncludes = false, bool disableTracking = false);

        Task<Study> GetByIdWithoutPermissionCheckAsync(int studyId, bool withIncludes = false, bool disableTracking = false);
    }
}
