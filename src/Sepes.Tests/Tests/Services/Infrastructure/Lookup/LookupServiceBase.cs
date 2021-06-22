using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Tests.Services.DomainServices.Lookup
{
    public class LookupServiceBase : ServiceTestBaseWithInMemoryDb
    {
        public LookupServiceBase()
            : base()
        {
        }

        protected async Task<SepesDbContext> RefreshAndSeedTestDatabase(string roleType)
        {
            var db = await ClearTestDatabaseAddUser();

            var study = new Study()
            {
                Id = 1,
                Name = "Test Study with specific participants",
                StudyParticipants = new List<StudyParticipant>
                    {
                        new StudyParticipant(){ UserId = 1, RoleName = roleType }
                    }
            };

            db.Studies.Add(study);
            await db.SaveChangesAsync();
            return db;
        }
    }
}
