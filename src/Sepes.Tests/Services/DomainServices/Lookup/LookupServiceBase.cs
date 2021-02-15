using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Sepes.Tests.Services.DomainServices.Lookup
{
    public class LookupServiceBase : ServiceTestBase
    {
        public LookupServiceBase()
            : base()
        {
        }

        protected async Task<SepesDbContext> RefreshAndSeedTestDatabase(string roleType)
        {
            var db = await ClearTestDatabase();
            var participant = new Infrastructure.Model.StudyParticipant(){ UserId = 1, RoleName = roleType };
            var listOfParticipants = new List<Infrastructure.Model.StudyParticipant>();
            listOfParticipants.Add(participant);
            var study = new Study()
            {
                Id = 1,
                StudyParticipants = listOfParticipants
            };


            db.Studies.Add(study);
            //db.Users.Add(user2);
            await db.SaveChangesAsync();
            return db;
        }
    }
}
