using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Study;
using Sepes.Infrastructure.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService.Interface
{
    public interface IStudyModelService
    {
        Task<Study> AddAsync(Study study);

        Task<IEnumerable<StudyListItemDto>> GetStudyListAsync();

        Task<Study> GetByIdAsync(int studyId, UserOperation userOperation, bool withIncludes = false, bool disableTracking = false);

        Task<Study> GetByIdWithoutPermissionCheckAsync(int studyId, bool withIncludes = false, bool disableTracking = false);
    }
}
