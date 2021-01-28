using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Service.Queries;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService
{
    public class StudyModelService : ModelServiceBase<Study>, IStudyModelService
    {
        public StudyModelService(SepesDbContext db, ILogger<StudyModelService> logger, IUserService userService)
            : base(db, logger, userService)
        {           
           
        }

        public async Task<Study> GetByIdAsync(int studyId, UserOperation userOperation, bool withIncludes = false, bool disableTracking = false)
        {
            return await StudySingularQueries.GetStudyByIdCheckAccessOrThrow(_db, _userService, studyId, userOperation, withIncludes, disableTracking);
        }

        public async Task<Study> GetByIdWithoutPermissionCheckAsync(int studyId, bool withIncludes = false, bool disableTracking = false)
        {
            return await StudySingularQueries.GetStudyByIdNoAccessCheck(_db, studyId, withIncludes, disableTracking);
        }
    }
}
