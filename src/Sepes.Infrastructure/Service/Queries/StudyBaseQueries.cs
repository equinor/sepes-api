using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using System.Linq;

namespace Sepes.Infrastructure.Service.Queries
{
    public static class StudyBaseQueries
    {
        public static IQueryable<Study> ActiveStudiesBaseQueryable(SepesDbContext db)
        {
            return db.Studies.Where(s => s.Deleted.HasValue == false || (s.Deleted.HasValue && s.Deleted.Value == false));
        }

        public static IQueryable<Study> UnHiddenStudiesQueryable(SepesDbContext db)
        {
            return ActiveStudiesBaseQueryable(db).Where(s => !s.Restricted);
        }
        public static IQueryable<Study> ActiveStudiesMinimalIncludesQueryable(SepesDbContext db)
        {
            return ActiveStudiesBaseQueryable(db)
                .Include(s => s.StudyParticipants)
             .Include(s => s.Sandboxes);


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
