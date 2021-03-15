﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Study;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
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
        public StudyModelService(IConfiguration configuration, SepesDbContext db, ILogger<StudyModelService> logger, IUserService userService)
            : base(configuration, db, logger, userService)
        {           
           
        }      

        public async Task<IEnumerable<StudyListItemDto>> GetStudyListAsync()
        {
            IEnumerable<StudyListItemDto> studies;

            var user = await _userService.GetCurrentUserAsync();

            var studiesQuery = "SELECT DISTINCT [Id] as [StudyId], [Name], [Description], [Vendor], [Restricted], [LogoUrl] FROM [dbo].[Studies] s";
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

        public async Task<StudyResultsAndLearningsDto> GetStudyResultsAndLearningsAsync(int studyId)
        {  
            var user = await _userService.GetCurrentUserAsync();

            var resultsAndLearningsQuery = "SELECT DISTINCT [Id] as [StudyId], [ResultsAndLearnings] FROM [dbo].[Studies] s WHERE Id=@studyId AND s.Closed = 0";           

            var responseFromDbService = await RunSingleEntityQuery<StudyResultsAndLearnings>(user, resultsAndLearningsQuery, UserOperation.Study_Read_ResultsAndLearnings, new { studyId });           

            return new StudyResultsAndLearningsDto() { ResultsAndLearnings = responseFromDbService.ResultsAndLearnings };
        }

        public async Task<Study> GetByIdAsync(int studyId, UserOperation userOperation, bool withIncludes = false, bool disableTracking = false)
        {
            return await StudySingularQueries.GetStudyByIdCheckAccessOrThrow(_db, _userService, studyId, userOperation, withIncludes, disableTracking);
        }

        public async Task<Study> GetByIdWithoutPermissionCheckAsync(int studyId, bool withIncludes = false, bool disableTracking = false)
        {
            return await StudySingularQueries.GetStudyByIdNoAccessCheck(_db, studyId, withIncludes, disableTracking);
        }
      

        public async Task<Study> GetStudyForStudyDetailsAsync(int studyId)
        {
            return await GetStudyFromQueryableThrowIfNotFoundOrNoAccess(StudyBaseQueries.StudyDetailsQueryable(_db), studyId, UserOperation.Study_Read);
        }


        async Task<Study> GetStudyFromQueryableThrowIfNotFoundOrNoAccess(IQueryable<Study> queryable, int studyId, UserOperation operation)
        {
            return await StudyAccessUtil.GetStudyFromQueryableThrowIfNotFoundOrNoAccess(_userService, queryable, studyId, operation);
        }     

       
    }
}
