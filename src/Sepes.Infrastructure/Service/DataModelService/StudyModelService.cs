using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Study;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Service.Queries;
using Sepes.Infrastructure.Util.Auth;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService
{
    public class StudyModelService : ModelServiceBase<Study>, IStudyModelService
    {
        public StudyModelService(IConfiguration configuration, SepesDbContext db, ILogger<StudyModelService> logger, IUserService userService)
            : base(configuration, db, logger, userService)
        {           
           
        }

        public async Task<IEnumerable<StudyListItemDto>> GetStudyListAsync()
        {
            IEnumerable<StudyListItemDto> studies;

            var user = await _userService.GetCurrentUserWithStudyParticipantsAsync();

            var studiesQuery = "SELECT DISTINCT [Id], [Name], [Description], [Vendor], [Restricted], [LogoUrl] FROM [dbo].[Studies] s";
                studiesQuery += " INNER JOIN [dbo].[StudyParticipants] sp on s.Id = sp.StudyId";
                studiesQuery += " WHERE s.Closed = 0";

            var studiesAccessWherePart = StudyAccessQueryBuilder.CreateAccessWhereClause(user, UserOperation.Study_Read);

            if (!string.IsNullOrWhiteSpace(studiesAccessWherePart))
            {
                studiesQuery += $" AND ({studiesAccessWherePart})";
            }

            using (var connection = new SqlConnection(GetDbConnectionString()))
            {
                if(connection.State != System.Data.ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                studies = await connection.QueryAsync<StudyListItemDto>(studiesQuery);
            }          

            return studies;
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
