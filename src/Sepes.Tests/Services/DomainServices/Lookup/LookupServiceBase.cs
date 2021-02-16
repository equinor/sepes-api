using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using System.Collections.Generic;
using System.Threading.Tasks;

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

            var study = new Study()
            {
                Id = 1,
                Name= "Test Study with specific participants",
                StudyParticipants = new List<Infrastructure.Model.StudyParticipant>
                    {
                    
                        new Infrastructure.Model.StudyParticipant(){ StudyId = 1, UserId = 1, RoleName = roleType }
                    }
            };

            db.Studies.Add(study);
            await db.SaveChangesAsync();
            return db;
        }
    }
}
