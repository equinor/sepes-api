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
            return db.Sandboxes.Where(s => !s.Deleted);
        }
      
        public static IQueryable<Sandbox> ActiveSandboxesMinimalIncludesQueryable(SepesDbContext db)
        {
            return ActiveSandboxesBaseQueryable(db)
                .Include(s => s.Study)
                .ThenInclude(s=> s.StudyParticipants)
                .ThenInclude(s => s.User)
                .Include(sb=> sb.PhaseHistory);
        }

        public static IQueryable<Sandbox> ActiveSandboxesWithIncludesQueryable(SepesDbContext db)
        {
            return ActiveSandboxesMinimalIncludesQueryable(db)
                  .Include(sb => sb.SandboxDatasets)
                    .ThenInclude(sd => sd.Dataset)
                    .ThenInclude(ds => ds.Resources)
                .Include(sb => sb.Resources)
                    .ThenInclude(r => r.Operations)              
                .Include(sb=> sb.PhaseHistory);
        }

        public static IQueryable<Sandbox> AllSandboxesBaseQueryable(SepesDbContext db)
        {
            return db.Sandboxes.Include(s => s.Study)
                .ThenInclude(s => s.StudyParticipants)
                 .Include(sb => sb.SandboxDatasets)
                    .ThenInclude(sd => sd.Dataset)
                .Include(sb => sb.Resources)
                    .ThenInclude(r => r.Operations)
                .Include(sb => sb.PhaseHistory);
        }
    }
}
