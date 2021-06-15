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

        public static IQueryable<Study> ActiveStudiesWithParticipantsAndUserQueryable(SepesDbContext db)
        {
            return ActiveStudiesBaseQueryable(db)
                .Include(s => s.StudyParticipants)
                .ThenInclude(sp=> sp.User).AsNoTracking();
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
                  .ThenInclude(sp => sp.User);             
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

        public static IQueryable<Study> StudyCloseQueryable(SepesDbContext db)
        {
            return ActiveStudiesWithParticipantsQueryable(db)
                .Include(s => s.Sandboxes);    
               
        }

        public static IQueryable<Study> StudyDeleteQueryable(SepesDbContext db)
        {
            return ActiveStudiesWithParticipantsQueryable(db)
                .Include(s => s.Sandboxes);
        }

        public static IQueryable<Study> StudyDatasetCreationQueryable(SepesDbContext db)
        {
            return StudyDatasetsQueryable(db)
                .Include(s => s.Resources)
                .ThenInclude(r=> r.Operations);
        }     
    }
}
