using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepes.Infrastructure.Service
{
    public static class StudyQueries
    {
        public static IQueryable<Study> GetQueryableForStudiesLookup(SepesDbContext db)
        {
            return db.Studies
            .Include(s => s.StudyDatasets)
                .ThenInclude(sd => sd.Dataset)
            .Include(s => s.Sandboxes)
            .Include(s => s.StudyParticipants)
                .ThenInclude(sp => sp.Participant);
        }
    }
}
