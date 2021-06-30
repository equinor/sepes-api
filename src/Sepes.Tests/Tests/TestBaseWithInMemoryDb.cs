using Microsoft.Extensions.DependencyInjection;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Tests.Common.Constants;
using System.Threading.Tasks;

namespace Sepes.Tests.Tests
{
    public class TestBaseWithInMemoryDb : TestBase
    {
        protected SepesDbContext GetDatabase()
        {
          return _serviceProvider.GetService<SepesDbContext>();     
        }

        protected async Task<SepesDbContext> ClearTestDatabase()
        {
            var db = GetDatabase();
            await db.Database.EnsureDeletedAsync();
            await db.Database.EnsureCreatedAsync();

            return db;
        }

        protected async Task<SepesDbContext> ClearTestDatabaseAddUser()
        {
            var db = await ClearTestDatabase();
            db.Users.Add(new User()
            {
                Id = TestUserConstants.COMMON_CUR_USER_DB_ID,
                ObjectId = TestUserConstants.COMMON_CUR_USER_OBJECTID,
                FullName = TestUserConstants.COMMON_CUR_USER_FULL_NAME,
                EmailAddress = TestUserConstants.COMMON_CUR_USER_EMAIL,
                UserName = TestUserConstants.COMMON_CUR_USER_UPN
            });

            db.Users.Add(new User()
            {
                Id = TestUserConstants.COMMON_ALTERNATIVE_STUDY_OWNER_DB_ID,
                ObjectId = TestUserConstants.COMMON_ALTERNATIVE_STUDY_OWNER_OBJECTID,
                FullName = TestUserConstants.COMMON_ALTERNATIVE_STUDY_OWNER_FULL_NAME,
                EmailAddress = TestUserConstants.COMMON_ALTERNATIVE_STUDY_OWNER_EMAIL,
                UserName = TestUserConstants.COMMON_ALTERNATIVE_STUDY_OWNER_UPN
            });

            await db.SaveChangesAsync();
            return db;
        }
    }
}
