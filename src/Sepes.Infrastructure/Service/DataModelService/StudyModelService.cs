using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Common.Constants;
using Sepes.Common.Dto.Study;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Response;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Service.Queries;
using Sepes.Infrastructure.Util.Auth;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService
{
    public class StudyModelService : ModelServiceBase<Study>, IStudyModelService
    {
        readonly IMapper _mapper;

        public StudyModelService(IConfiguration configuration, SepesDbContext db, ILogger<StudyModelService> logger, IUserService userService, IMapper mapper)
            : base(configuration, db, logger, userService)
        {
            _mapper = mapper;
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

        public async Task<Study> GetByIdAsync(int studyId, UserOperation userOperation)
        {
            return await GetStudyFromQueryableThrowIfNotFoundOrNoAccess(StudyBaseQueries.ActiveStudiesWithParticipantsQueryable(_db), studyId, userOperation);          
        }

        public async Task<Study> GetForStudyDetailsAsync(int studyId)
        {
            return await GetStudyFromQueryableThrowIfNotFoundOrNoAccess(StudyBaseQueries.StudyDetailsQueryable(_db), studyId, UserOperation.Study_Read);
        }

        public async Task<Study> GetForDatasetsAsync(int studyId, UserOperation operation = UserOperation.Study_Read)
        {
            return await GetStudyFromQueryableThrowIfNotFoundOrNoAccess(StudyBaseQueries.StudyDatasetsQueryable(_db), studyId, operation);
        }

        public async Task<Study> GetWitParticipantsNoAccessCheck(int studyId)
        {
            return await StudyAccessUtil.GetStudyFromQueryableThrowIfNotFound(StudyBaseQueries.ActiveStudiesWithParticipantsQueryable(_db), studyId);
        }

        public async Task<Study> GetForParticpantOperationsAsync(int studyId, UserOperation operation, string roleBeingAddedOrRemoved = null)
        {
            return await GetStudyFromQueryableThrowIfNotFoundOrNoAccess(StudyBaseQueries.StudyParticipantOperationsQueryable(_db), studyId, operation, roleBeingAddedOrRemoved);
        }

        public async Task<Study> GetForCloseAsync(int studyId, UserOperation operation)
        {
            return await GetStudyFromQueryableThrowIfNotFoundOrNoAccess(StudyBaseQueries.StudyCloseQueryable(_db), studyId, operation);
        }

        public async Task<Study> GetForDeleteAsync(int studyId, UserOperation operation)
        {
            return await GetStudyFromQueryableThrowIfNotFoundOrNoAccess(StudyBaseQueries.StudyDeleteQueryable(_db), studyId, operation);
        }

        public async Task<Study> GetForSandboxCreateAndDeleteAsync(int studyId, UserOperation operation)
        {
            return await GetStudyFromQueryableThrowIfNotFoundOrNoAccess(StudyBaseQueries.StudySandboxCreationQueryable(_db), studyId, operation);
        }

        public async Task<Study> GetForDatasetCreationAsync(int studyId, UserOperation operation)
        {
            return await GetStudyFromQueryableThrowIfNotFoundOrNoAccess(StudyBaseQueries.StudyDatasetCreationQueryable(_db), studyId, operation);
        }

        public async Task<Study> GetForDatasetCreationNoAccessCheckAsync(int studyId)
        {
            return await StudyAccessUtil.GetStudyFromQueryableThrowIfNotFound(StudyBaseQueries.StudyDatasetCreationQueryable(_db), studyId);
        }

        async Task<Study> GetStudyFromQueryableThrowIfNotFoundOrNoAccess(IQueryable<Study> queryable, int studyId, UserOperation operation, string roleBeingAddedOrRemoved = null)
        {
            return await StudyAccessUtil.GetStudyFromQueryableThrowIfNotFoundOrNoAccess(_userService, queryable, studyId, operation, roleBeingAddedOrRemoved);
        }         
    }
}
