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
using Sepes.Infrastructure.Util;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService
{
    public class SandboxModelService : EfModelServiceBase<Sandbox>, ISandboxModelService
    {
        public SandboxModelService(IConfiguration configuration, SepesDbContext db, ILogger<SandboxModelService> logger, IUserService userService, IStudyPermissionService studyPermissionService)
            : base(configuration, db, logger, userService, studyPermissionService)
        {

        }

        public async Task<bool> NameIsTaken(int studyId, string sandboxName)
        {
            return await _db.Sandboxes.Where(sb => sb.StudyId == studyId && sb.Name == sandboxName && !sb.Deleted).AnyAsync();
        }

        public async Task HardDeleteAsync(int sandboxId)
        {
            var sandbox = await _db.Sandboxes.FirstOrDefaultAsync(sb => sb.Id == sandboxId);

            if (sandbox != null)
            {
                _db.Sandboxes.Remove(sandbox);
                await _db.SaveChangesAsync();
            }
        }

        public async Task SoftDeleteAsync(int sandboxId)
        {            
            var sandbox = await GetByIdAsync(sandboxId, UserOperation.Study_Crud_Sandbox, false);          

            if (sandbox != null)
            {
                var user = await _userService.GetCurrentUserAsync();
                SoftDeleteUtil.MarkAsDeleted(sandbox, user);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<Sandbox> AddAsync(Study study, Sandbox newSandbox)
        {
            var user = await _userService.GetCurrentUserAsync();

            newSandbox.CreatedBy = user.UserName;
            newSandbox.TechnicalContactName = user.FullName;
            newSandbox.TechnicalContactEmail = user.EmailAddress;

            SandboxPhaseUtil.InitiatePhaseHistory(newSandbox, user);
            
            newSandbox.Study = study;
            study.Sandboxes.Add(newSandbox);

            await _db.SaveChangesAsync();

            return newSandbox;
        }

        public async Task<Sandbox> GetByIdAsync(int id, UserOperation userOperation, bool withIncludes = false, bool disableTracking = false)
        {
            var sandboxQueryable =
               (withIncludes ? SandboxBaseQueries.ActiveSandboxesWithIncludesQueryable(_db) : SandboxBaseQueries.ActiveSandboxesMinimalIncludesQueryable(_db));

            var sandbox = await GetSandboxFromQueryableThrowIfNotFoundOrNoAccess(sandboxQueryable, id, userOperation);

            return sandbox;
        }

        public async Task<Sandbox> GetByIdForResourceCreationAsync(int id, UserOperation userOperation)
        {
            var sandboxQueryable = SandboxBaseQueries.ForResourceCreation(_db);

            var sandbox = await GetSandboxFromQueryableThrowIfNotFoundOrNoAccess(sandboxQueryable, id, userOperation);

            return sandbox;
        }

        public async Task<Sandbox> GetByIdForPhaseShiftAsync(int id, UserOperation userOperation)
        {
            var sandboxQueryable = SandboxBaseQueries.SandboxForPhaseShift(_db);
            var sandbox = await GetSandboxFromQueryableThrowIfNotFoundOrNoAccess(sandboxQueryable, id, userOperation);

            return sandbox;
        }

        public async Task<Sandbox> GetByIdForResourcesAsync(int sandboxId)
        {
            var sandboxQueryable = SandboxBaseQueries.SandboxWithStudyParticipantResourceAndOperations(_db).AsNoTracking();
            var sandbox = await GetSandboxFromQueryableThrowIfNotFoundOrNoAccess(sandboxQueryable, sandboxId, UserOperation.Study_Read);

            if (sandbox.Deleted && sandbox.DeletedAt.HasValue && sandbox.DeletedAt.Value.AddMinutes(15) < DateTime.UtcNow)
            {
                throw NotFoundException.CreateForEntity("Sandbox", sandboxId);
            }

            return sandbox;
        }

        public async Task<Sandbox> GetByIdForCostAnalysisLinkAsync(int id, UserOperation userOperation)
        {
            var sandboxQueryable = SandboxBaseQueries.SandboxWithResources(_db).AsNoTracking();
            var sandbox = await GetSandboxFromQueryableThrowIfNotFoundOrNoAccess(sandboxQueryable, id, userOperation);
            return sandbox;

        }

        public async Task<Sandbox> GetByIdForReScheduleCreateAsync(int sandboxId)
        {
            var sandboxQueryable = SandboxBaseQueries.ActiveSandboxWithResourceAndOperations(_db);
            var sandbox = await GetSandboxFromQueryableThrowIfNotFoundOrNoAccess(sandboxQueryable, sandboxId, UserOperation.Study_Crud_Sandbox);

            return sandbox;
        }

        public async Task<string> GetRegionByIdAsync(int sandboxId, UserOperation userOperation)
        {
            var sandbox = await GetSandboxFromQueryableThrowIfNotFoundOrNoAccess(SandboxBaseQueries.ActiveSandboxesMinimalIncludesQueryable(_db).AsNoTracking(), sandboxId, userOperation);
            return sandbox.Region;
        }

        async Task<Sandbox> GetSandboxFromQueryableThrowIfNotFound(IQueryable<Sandbox> queryable, int sandboxId)
        {
            var sandbox = await queryable.SingleOrDefaultAsync(s => s.Id == sandboxId);

            CheckExistenceAndThrowIfMissing(sandboxId, sandbox);           

            return sandbox;
        }


        async Task<Sandbox> GetSandboxFromQueryableThrowIfNotFoundOrNoAccess(IQueryable<Sandbox> queryable, int sandboxId, UserOperation operation)
        {
            var sandbox = await GetSandboxFromQueryableThrowIfNotFound(queryable, sandboxId);

            await CheckAccesAndThrowIfNotAllowed(sandbox, operation);

            return sandbox;
        }

        void CheckExistenceAndThrowIfMissing(int sandboxId, Sandbox sandbox)
        {
            if (sandbox == null)
            {
                throw NotFoundException.CreateForEntity("Sandbox", sandboxId);
            }
        }

        async Task CheckAccesAndThrowIfNotAllowed(Sandbox sandbox, UserOperation operation)
        {
            await CheckAccesAndThrowIfNotAllowed(sandbox.Study, operation);
        }

        public async Task<Sandbox> GetDetailsByIdAsync(int id)
        {
            return await GetSandboxFromQueryableThrowIfNotFoundOrNoAccess(SandboxBaseQueries.SandboxDetailsQueryable(_db), id, UserOperation.Study_Read);
        }

        public async Task<Sandbox> GetWithResourcesNoPermissionCheckAsync(int sandboxId)
        {
            var queryable = SandboxBaseQueries.SandboxWithResourceAndOperations(_db);

            return await GetSandboxFromQueryableThrowIfNotFound(queryable, sandboxId);
        }
    }
}
