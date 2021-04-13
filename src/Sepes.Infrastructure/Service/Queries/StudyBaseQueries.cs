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

        public static IQueryable<Study> StudyDetailsQueryable(SepesDbContext db)
        {
            return ActiveStudiesBaseQueryable(db)
                .Include(s => s.StudyParticipants)
                .ThenInclude(sp=> sp.User)
                 .Include(s => s.Sandboxes)
                .Include(s => s.StudyDatasets)
                    .ThenInclude(sd=> sd.Dataset)
                        .ThenInclude(sd => sd.SandboxDatasets)
                            .ThenInclude(sd => sd.Sandbox)
                            .AsNoTracking();
        }

        public static IQueryable<Study> StudyParticipantOperationsQueryable(SepesDbContext db)
        {
            return ActiveStudiesBaseQueryable(db)
                .Include(s => s.StudyParticipants)
                .Include(s => s.Sandboxes)
                     .ThenInclude(sd => sd.Resources)
                .Include(s => s.Resources);
             
        }

        public static IQueryable<Study> StudyDatasetsQueryable(SepesDbContext db)
        {
            return ActiveStudiesBaseQueryable(db)
                .Include(s => s.StudyParticipants)
                      .ThenInclude(sp => sp.User)
                .Include(s => s.StudyDatasets)
                    .ThenInclude(sd => sd.Dataset)
                     .ThenInclude(sd => sd.Resources);
                      
        }

        public static IQueryable<Study> StudySandboxCreationQueryable(SepesDbContext db)
        {
            return ActiveStudiesBaseQueryable(db)
                .Include(s => s.StudyParticipants)
                    .ThenInclude(sp=> sp.User)
                .Include(s => s.Sandboxes);
        }

        public static IQueryable<Study> StudyDeleteQueryable(SepesDbContext db)
        {
            return ActiveStudiesWithParticipantsQueryable(db)
                .Include(s => s.Sandboxes)
                .Include(s=> s.Resources)
                    .ThenInclude(r=> r.ChildResources)
                .Include(s => s.StudyDatasets)
                    .ThenInclude(sds=> sds.Dataset);
        }

        public static IQueryable<Study> StudyDatasetCreationQueryable(SepesDbContext db)
        {
            return StudyDatasetsQueryable(db)
                .Include(s => s.Resources)
                .ThenInclude(r=> r.Operations);
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
                .Include(s => s.StudyParticipants)                   
                .Include(s => s.Sandboxes)
                    .ThenInclude(sb => sb.Resources)
                         .ThenInclude(cr => cr.Operations);
        } 
    }
}
