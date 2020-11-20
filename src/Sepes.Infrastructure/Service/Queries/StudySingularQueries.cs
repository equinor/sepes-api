using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Queries
{
    public static class StudySingularQueries
    {

        #region Public Methods

        public static async Task<Study> GetStudyByIdThrowIfNotFoundAsync(SepesDbContext db, int studyId, bool withIncludes = false)
        {
            var studyFromDb = await
                (withIncludes ? StudyBaseQueries.ActiveStudiesWithIncludesQueryable(db) : StudyBaseQueries.ActiveStudiesMinimalIncludesQueryable(db))
                .FirstOrDefaultAsync(s => s.Id == studyId);

            if (studyFromDb == null)
            {
                throw NotFoundException.CreateForEntity("Study", studyId);
            }

            return studyFromDb;
        }

        public static async Task<Study> GetStudyByIdCheckAccessOrThrow(SepesDbContext db, IUserService userService, int studyId, UserOperations operation, bool withIncludes = false)
        {
            var studyFromDb = await GetStudyByIdThrowIfNotFoundAsync(db, studyId, withIncludes);
            return await StudyAccessUtil.CheckStudyAccessOrThrow(userService, studyFromDb, operation);
        }

        public static async Task<Study> GetStudyBySandboxIdCheckAccessOrThrow(SepesDbContext db, IUserService userService, int sandboxId, UserOperations operation)
        {
            var studyFromDb = await GetStudyBySandboxIdOrThrowAsync(db, sandboxId);
            return await StudyAccessUtil.CheckStudyAccessOrThrow(userService, studyFromDb, operation);
        }

        public static async Task<Study> GetStudyByResourceIdCheckAccessOrThrow(SepesDbContext db, IUserService userService, int resourceId, UserOperations operation, bool withIncludes = false)
        {
            var studyFromDb = await GetStudyByResourceIdOrThrowAsync(db, resourceId, withIncludes);
            return await StudyAccessUtil.CheckStudyAccessOrThrow(userService, studyFromDb, operation);
        }

        #endregion    

        static async Task<int> GetStudyIdByResourceIdAsync(SepesDbContext db, int resourceId)
        {
            var sandboxId = await GetSandboxIdByResourceIdAsync(db, resourceId);
            var studyId = await GetStudyIdBySandboxIdAsync(db, sandboxId);
            return studyId;
        }

        static async Task<int> GetStudyIdBySandboxIdAsync(SepesDbContext db, int sandboxId)
        {
            var studyId = await db.Sandboxes.Where(sb => sb.Id == sandboxId).Select(sr => sr.StudyId).SingleOrDefaultAsync();
            return studyId;
        }

        static async Task<int> GetSandboxIdByResourceIdAsync(SepesDbContext db, int resourceId)
        {
            var sandboxId = await db.SandboxResources.Where(sr => sr.Id == resourceId).Select(sr => sr.SandboxId).SingleOrDefaultAsync();
            return sandboxId;
        }      

        static async Task<Study> GetStudyBySandboxIdOrThrowAsync(SepesDbContext db, int sandboxId, bool withIncludes = false)
        {
            var studyId = await GetStudyIdBySandboxIdAsync(db, sandboxId);
            return await GetStudyByIdThrowIfNotFoundAsync(db, studyId, withIncludes);           
        }

        static async Task<Study> GetStudyByResourceIdOrThrowAsync(SepesDbContext db, int resourceId, bool withIncludes = false)
        {
            var studyId = await GetStudyIdByResourceIdAsync(db, resourceId);
            return await GetStudyByIdThrowIfNotFoundAsync(db, studyId, withIncludes);
        }    
    }
}
