using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Tests.Setup.ModelFactory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sepes.Tests.Setup
{
    public static class StudyPopulator
    {
        public static void Add(SepesDbContext db, string studyName, string vendor, string wbs, int ownerId)
        {
            var newStudy = StudyFactory.Create(studyName, vendor, wbs, new List<StudyParticipant>() { new StudyParticipant() { UserId = ownerId, RoleName = StudyRoles.StudyOwner } });


            db.Studies.Add(newStudy);
        }
    }
}
