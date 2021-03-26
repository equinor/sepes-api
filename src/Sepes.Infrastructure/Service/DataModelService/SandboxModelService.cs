using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Service.Queries;
using Sepes.Infrastructure.Util.Auth;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService
{
    public class SandboxModelService : ModelServiceBase<Sandbox>, ISandboxModelService
    {
        public SandboxModelService(IConfiguration configuration, SepesDbContext db, ILogger<SandboxModelService> logger, IUserService userService)
            : base(configuration, db, logger, userService)
        {

        }

        public async Task<Sandbox> GetByIdWithoutPermissionCheckAsync(int sandboxId)
        {
            return await SandboxBaseQueries.AllSandboxesBaseQueryable(_db).SingleOrDefaultAsync(sb => sb.Id == sandboxId);
        }

        public async Task<Sandbox> GetByIdAsync(int id, UserOperation userOperation, bool withIncludes = false, bool disableTracking = false)
        {
            var sandboxQueryable =
               (withIncludes ? SandboxBaseQueries.ActiveSandboxesWithIncludesQueryable(_db) : SandboxBaseQueries.ActiveSandboxesMinimalIncludesQueryable(_db));

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
            var sandboxQueryable = SandboxBaseQueries.SandboxWithResourceAndOperations(_db).AsNoTracking();
            var sandbox = await GetSandboxFromQueryableThrowIfNotFoundOrNoAccess(sandboxQueryable, sandboxId, UserOperation.Study_Read);

            if(sandbox.Deleted && sandbox.DeletedAt.HasValue && sandbox.DeletedAt.Value.AddMinutes(15) < DateTime.UtcNow)
            {
                throw NotFoundException.CreateForEntity("Sandbox", sandboxId);
            }

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

        async Task CheckAccesAndThrowIfMissing(Sandbox sandbox, UserOperation operation)
        {
            await CheckAccesAndThrowIfMissing(sandbox.Study, operation);
        }

        async Task<Sandbox> GetSandboxFromQueryableThrowIfNotFoundOrNoAccess(IQueryable<Sandbox> queryable, int sandboxId, UserOperation operation)
        {
            var sandbox = await queryable.SingleOrDefaultAsync(s => s.Id == sandboxId);

            if (sandbox == null)
            {
                throw NotFoundException.CreateForEntity("Sandbox", sandboxId);
            }

            await CheckAccesAndThrowIfMissing(sandbox, operation);

            return sandbox;
        }

        public async Task<Sandbox> GetDetailsByIdAsync(int id)
        {
            return await GetSandboxFromQueryableThrowIfNotFoundOrNoAccess(SandboxBaseQueries.SandboxDetailsQueryable(_db), id, UserOperation.Study_Read);
        }

      
    }
}
