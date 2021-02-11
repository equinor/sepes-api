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
            return db.Studies.Where(s => !s.Closed);
        }      
      
        public static IQueryable<Study> ActiveStudiesWithParticipantsQueryable(SepesDbContext db)
        {
            return ActiveStudiesBaseQueryable(db)
                .Include(s => s.StudyParticipants);
        }

        public static IQueryable<Study> AllStudiesWithParticipantsQueryable(SepesDbContext db)
        {
            return db.Studies
                .Include(s => s.StudyParticipants);
        }

        public static IQueryable<Study> ActiveStudiesMinimalIncludesQueryable(SepesDbContext db)
        {
            return ActiveStudiesWithParticipantsQueryable(db)            
             .Include(s => s.Sandboxes);
        }       

        public static IQueryable<Study> ActiveStudiesWithIncludesQueryable(SepesDbContext db)
        {
            return ActiveStudiesBaseQueryable(db)
                .Include(s => s.Resources)
                  .ThenInclude(s => s.ChildResources)

                 .Include(s => s.StudyDatasets)
                    .ThenInclude(sd => sd.Dataset)
                    .ThenInclude(sd => sd.SandboxDatasets)
                     .ThenInclude(sd => sd.Sandbox)
                .Include(s => s.StudyParticipants)
                    .ThenInclude(sp => sp.User)
                .Include(s => s.Sandboxes)
                    .ThenInclude(sb => sb.Resources)
                         .ThenInclude(cr => cr.Operations);
        }
    }
}
