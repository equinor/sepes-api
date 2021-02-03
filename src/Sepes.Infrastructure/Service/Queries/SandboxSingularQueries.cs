using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util.Auth;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Queries
{
    public static class SandboxSingularQueries
    {

        #region Public Methods

        public static async Task<Sandbox> GetSandboxByIdNoChecks(SepesDbContext db, int sandboxId)
        {
            return await SandboxBaseQueries.AllSandboxesBaseQueryable(db).SingleOrDefaultAsync(sb=> sb.Id == sandboxId);
        }

        public static async Task<bool> SandboxWithNameAllreadyExists(SepesDbContext db, string name)
        {
            return await SandboxBaseQueries.ActiveSandboxesBaseQueryable(db).Where(sb => sb.Name == name).AnyAsync();
        }

        public static async Task<Sandbox> GetSandboxByIdThrowIfNotFoundAsync(SepesDbContext db, int sandboxId, bool withIncludes = false)
        {
            var sandboxQueryable =
                (withIncludes ? SandboxBaseQueries.ActiveSandboxesWithIncludesQueryable(db) : SandboxBaseQueries.ActiveSandboxesMinimalIncludesQueryable(db));

            var sandbox = await sandboxQueryable.SingleOrDefaultAsync(s => s.Id == sandboxId);

            if (sandbox == null)
            {
                throw NotFoundException.CreateForEntity("Sandbox", sandboxId);
            }

            return sandbox;
        }

        public static async Task<Sandbox> GetSandboxByIdCheckAccessOrThrow(SepesDbContext db, IUserService userService, int sandboxId, UserOperation operation, bool withIncludes = false)
        {
            var sandboxFromDb = await GetSandboxByIdThrowIfNotFoundAsync(db, sandboxId, withIncludes);

            if (await StudyAccessUtil.HasAccessToOperationForStudyAsync(userService, sandboxFromDb.Study, operation))
            {
                return sandboxFromDb;
            }

            throw new ForbiddenException($"User {(await userService.GetCurrentUserAsync()).EmailAddress} does not have permission to perform operation {operation} on study {sandboxFromDb.StudyId}");          
        }

        public static async Task<Sandbox> GetSandboxByResourceIdCheckAccessOrThrow(SepesDbContext db, IUserService userService, int resourceId, UserOperation operation, bool withIncludes = false)
        {
            var sandboxId = await GetSandboxIdByResourceIdAsync(db, resourceId);
            var sandbox = await GetSandboxByIdCheckAccessOrThrow(db, userService, sandboxId, operation, withIncludes);

            return sandbox;
           
        }

        #endregion                 

        static async Task<int> GetSandboxIdByResourceIdAsync(SepesDbContext db, int resourceId)
        {
            var sandboxId = await db.CloudResources.Where(sr => sr.Id == resourceId).Select(sr => sr.SandboxId).SingleOrDefaultAsync();

            if (sandboxId.HasValue)
            {
                return sandboxId.Value;
            }

            return default(int);        
        }         
    }
}
