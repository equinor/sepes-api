using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using System.Linq;

namespace Sepes.Infrastructure.Service.Queries
{
    public static class SandboxBaseQueries
    {
        public static IQueryable<Sandbox> ActiveSandboxesBaseQueryable(SepesDbContext db)
        {
            return db.Sandboxes.Where(s => s.Deleted.HasValue == false || (s.Deleted.HasValue && s.Deleted.Value == false));
        }
      
        public static IQueryable<Sandbox> ActiveSandboxesMinimalIncludesQueryable(SepesDbContext db)
        {
            return ActiveSandboxesBaseQueryable(db)
                .Include(s => s.Study)
                .ThenInclude(s=> s.StudyParticipants);
        }

        public static IQueryable<Sandbox> ActiveStudiesWithIncludesQueryable(SepesDbContext db)
        {
            return ActiveSandboxesMinimalIncludesQueryable(db)
                  .Include(sb => sb.SandboxDatasets)
                    .ThenInclude(sd => sd.Dataset)
                .Include(sb => sb.Resources)
                    .ThenInclude(r => r.Operations);
        } 
    }
}
