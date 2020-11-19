using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public static class StudyQueries
    {
        public static IQueryable<Study> ActiveStudiesBaseQueryable(SepesDbContext db)
        {
            return db.Studies.Where(s => s.Deleted.HasValue == false || (s.Deleted.HasValue && s.Deleted.Value == false));
        }

        public static IQueryable<Study> UnHiddenStudiesQueryable(SepesDbContext db)
        {
            return ActiveStudiesBaseQueryable(db).Where(s => !s.Restricted);
        }

        public static IQueryable<Study> ActiveStudiesWithIncludesQueryable(SepesDbContext db)
        {
            return ActiveStudiesBaseQueryable(db)
                 .Include(s => s.StudyDatasets)
                    .ThenInclude(sd => sd.Dataset)
                    .ThenInclude(sd => sd.SandboxDatasets)
                     .ThenInclude(sd => sd.Sandbox)
                .Include(s => s.StudyParticipants)
                    .ThenInclude(sp => sp.User)
                .Include(s => s.Sandboxes)
                    .ThenInclude(sp => sp.Resources)
                         .ThenInclude(sb => sb.Operations);
        }

        public static async Task<int> GetStudyIdByResourceIdAsync(SepesDbContext db, int resourceId)
        {
            var sandboxId = await GetSandboxIdByResourceIdAsync(db, resourceId);
            var studyId = await GetStudyIdBySandboxIdAsync(db, sandboxId);
            return studyId;
        }

        public static async Task<int> GetStudyIdBySandboxIdAsync(SepesDbContext db, int sandboxId)
        {
            var studyId = await db.Sandboxes.Where(sb => sb.Id == sandboxId).Select(sr => sr.StudyId).SingleOrDefaultAsync();
            return studyId;
        }

        public static async Task<int> GetSandboxIdByResourceIdAsync(SepesDbContext db, int resourceId)
        {
            var sandboxId = await db.SandboxResources.Where(sr => sr.Id == resourceId).Select(sr => sr.SandboxId).SingleOrDefaultAsync();
            return sandboxId;
        }

        public static async Task<Study> GetStudyByIdOrThrowAsync(SepesDbContext db, int studyId)
        {
            var studyFromDb = await ActiveStudiesWithIncludesQueryable(db)
                .FirstOrDefaultAsync(s => s.Id == studyId);

            if (studyFromDb == null)
            {
                throw NotFoundException.CreateForEntity("Study", studyId);
            }

            return studyFromDb;
        }

        public static async Task<Study> GetStudyBySandboxIdOrThrowAsync(SepesDbContext db, int sandboxId)
        {
            var studyId = await GetStudyIdBySandboxIdAsync(db, sandboxId);
            return await GetStudyByIdOrThrowAsync(db, studyId);           
        }

        public static async Task<Study> GetStudyByResourceIdOrThrowAsync(SepesDbContext db, int resourceId)
        {
            var studyId = await GetStudyIdByResourceIdAsync(db, resourceId);
            return await GetStudyByIdOrThrowAsync(db, studyId);
        }

        public static IQueryable<Study> ActiveStudiesLookupQueryable(SepesDbContext db)
        {
            return ActiveStudiesBaseQueryable(db)
            .Include(s => s.StudyDatasets)
                .ThenInclude(sd => sd.Dataset)
            .Include(s => s.Sandboxes)
            .Include(s => s.StudyParticipants)
                .ThenInclude(sp => sp.User);
        }
    }
}
