using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Common.Constants;
using Sepes.Common.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Service.Queries;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService
{
    public class StudyEfModelService : EfModelServiceBase<Study>, IStudyEfModelService
    {
        readonly IUserService _userService;

        public StudyEfModelService(IConfiguration configuration, SepesDbContext db, ILogger<StudyEfModelService> logger, IUserService userService, IStudyPermissionService studyPermissionService)
            : base(configuration, db, logger, studyPermissionService)
        {
            _userService = userService;
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
            return await GetStudyFromQueryableThrowIfNotFound(StudyBaseQueries.ActiveStudiesWithParticipantsQueryable(_db), studyId);
        }

        public async Task<Study> GetWithParticipantsAndUsersNoAccessCheck(int studyId)
        {
            return await GetStudyFromQueryableThrowIfNotFound(StudyBaseQueries.ActiveStudiesWithParticipantsAndUserQueryable(_db), studyId);
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
            return await GetStudyFromQueryableThrowIfNotFound(StudyBaseQueries.StudyDatasetCreationQueryable(_db), studyId);
        }

        public async Task<bool> HasActiveDatasetsAsync(int studyId)
        {
            return await _db.StudyDatasets.Where(sds => sds.StudyId == studyId && sds.Dataset.Deleted && sds.Dataset.StudySpecific).AnyAsync();
        }

        public async Task<bool> HasActiveSandboxesAsync(int studyId)
        {
            return await _db.Sandboxes.Where(sb => sb.StudyId == studyId && sb.Deleted).AnyAsync();
        }

        async Task<Study> GetStudyFromQueryableThrowIfNotFoundOrNoAccess(IQueryable<Study> queryable, int studyId, UserOperation operation, string roleBeingAddedOrRemoved = null)
        {
            return await GetStudyFromQueryableThrowIfNotFoundOrNoAccess(_userService, queryable, studyId, operation, roleBeingAddedOrRemoved);
        }

        async Task<Study> GetStudyFromQueryableThrowIfNotFoundOrNoAccess(IUserService userService, IQueryable<Study> queryable, int studyId, UserOperation operation, string roleBeingAddedOrRemoved = null)
        {
            var study = await GetStudyFromQueryableThrowIfNotFound(queryable, studyId);

            await _studyPermissionService.VerifyAccessOrThrow(study, operation, roleBeingAddedOrRemoved);

            return study;
        }

        async Task<Study> GetStudyFromQueryableThrowIfNotFound(IQueryable<Study> queryable, int studyId)
        {
            var study = await queryable.SingleOrDefaultAsync(s => s.Id == studyId);
           
            if (study == null)
            {
                throw NotFoundException.CreateForEntity("Study", studyId);
            }

            return study;
        }     
    }
}
