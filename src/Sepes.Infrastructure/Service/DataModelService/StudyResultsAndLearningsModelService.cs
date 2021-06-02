using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Common.Constants;
using Sepes.Common.Dto.Study;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService
{
    public class StudyResultsAndLearningsModelService : DapperModelServiceBase, IStudyResultsAndLearningsModelService
    {
        public StudyResultsAndLearningsModelService(IConfiguration configuration, ILogger<StudyResultsAndLearningsModelService> logger, IUserService userService, IStudyPermissionService studyPermissionService)
            : base(configuration, logger, userService, studyPermissionService)
        {

        }        

        public async Task<StudyResultsAndLearningsDto> GetAsync(int studyId)
        {
            var user = await _userService.GetCurrentUserAsync();

            var resultsAndLearningsQuery = "SELECT DISTINCT [Id] as [StudyId], [ResultsAndLearnings] FROM [dbo].[Studies] s WHERE Id=@studyId AND s.Closed = 0";

            var responseFromDbService = await RunSingleEntityQueryWithPermissionCheck<StudyResultsAndLearnings>(user, resultsAndLearningsQuery, UserOperation.Study_Read_ResultsAndLearnings, new { studyId });

            return new StudyResultsAndLearningsDto() { ResultsAndLearnings = responseFromDbService.ResultsAndLearnings };
        }
    }
}
