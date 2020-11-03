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
            return db.Studies.Where(s => s.DeletedAt.HasValue == false);
        }     

        public static IQueryable<Study> UnHiddenStudiesQueryable(SepesDbContext db)
        {
            return ActiveStudiesBaseQueryable(db).Where(s => !s.Restricted); ;
        }

        public static async Task<Study> GetStudyOrThrowAsync(int studyId, SepesDbContext db)
        {
            var studyFromDb = await ActiveStudiesBaseQueryable(db)
                .Include(s => s.StudyDatasets)
                    .ThenInclude(sd => sd.Dataset)
                .Include(s => s.StudyParticipants)
                    .ThenInclude(sp => sp.User)
                .Include(s => s.Sandboxes)
                    .ThenInclude(sp => sp.Resources)
                         .ThenInclude(sb => sb.Operations)
                .FirstOrDefaultAsync(s => s.Id == studyId);

            if (studyFromDb == null)
            {
                throw NotFoundException.CreateForEntity("Study", studyId);
            }

            return studyFromDb;
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
