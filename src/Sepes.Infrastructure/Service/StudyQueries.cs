﻿using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static async Task<Study> GetStudyOrThrowAsync(int studyId, SepesDbContext db)
        {
            var studyFromDb = await db.Studies
                .Include(s => s.StudyDatasets)
                    .ThenInclude(sd => sd.Dataset)
                .Include(s => s.Sandboxes)
                .Include(s => s.StudyParticipants)
                    .ThenInclude(sp => sp.Participant)
                .FirstOrDefaultAsync(s => s.Id == studyId);

            if (studyFromDb == null)
            {
                throw NotFoundException.CreateForIdentity("Study", studyId);
            }

            return studyFromDb;
        }
    }
}