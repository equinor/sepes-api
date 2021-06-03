using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Common.Constants;
using Sepes.Common.Dto.Study;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util.Auth;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService
{
    public class StudyRawQueryModelService : DapperModelServiceBase, IStudyRawQueryModelService
    {
        public StudyRawQueryModelService(IConfiguration configuration, ILogger<StudyRawQueryModelService> logger, IUserService userService)
            : base(configuration, logger, userService)
        {
            
        }      

        public async Task<IEnumerable<StudyListItemDto>> GetListAsync()
        {
            IEnumerable<StudyListItemDto> studies;

            var user = await _userService.GetCurrentUserAsync();

            var studiesQuery = "SELECT DISTINCT [Id], [Name], [Description], [Vendor], [Restricted], [LogoUrl] FROM [dbo].[Studies] s";
                studiesQuery += " INNER JOIN [dbo].[StudyParticipants] sp on s.Id = sp.StudyId";
                studiesQuery += " WHERE s.Closed = 0";

            var studiesAccessWherePart = StudyAccessQueryBuilder.CreateAccessWhereClause(user, UserOperation.Study_Read);

            if (!string.IsNullOrWhiteSpace(studiesAccessWherePart))
            {
                studiesQuery += $" AND ({studiesAccessWherePart})";
            }

            studies = await RunDapperQueryMultiple<StudyListItemDto>(studiesQuery);
            return studies;
        }

        public async Task<StudyResultsAndLearningsDto> GetResultsAndLearningsAsync(int studyId)
        {  
            var user = await _userService.GetCurrentUserAsync();

            var resultsAndLearningsQuery = "SELECT DISTINCT [Id] as [StudyId], [ResultsAndLearnings] FROM [dbo].[Studies] s WHERE Id=@studyId AND s.Closed = 0";           

            var responseFromDbService = await RunSingleEntityQuery<StudyResultsAndLearnings>(user, resultsAndLearningsQuery, UserOperation.Study_Read_ResultsAndLearnings, new { studyId });           

            return new StudyResultsAndLearningsDto() { ResultsAndLearnings = responseFromDbService.ResultsAndLearnings };
        }              
    }
}
