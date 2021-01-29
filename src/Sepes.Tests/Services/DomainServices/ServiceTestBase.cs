using Microsoft.Extensions.DependencyInjection;
using Sepes.Infrastructure.Constants.Auth;
using Sepes.Infrastructure.Model.Context;
using Sepes.Tests.Common.Constants;
using Sepes.Tests.Setup;
using System.Threading.Tasks;

namespace Sepes.Tests.Services
{
    public class ServiceTestBase
    {
        protected readonly ServiceCollection _services;
        protected readonly ServiceProvider _serviceProvider;

        public ServiceTestBase()
        {
            _services = BasicServiceCollectionFactory.GetServiceCollectionWithInMemory();
            _serviceProvider = _services.BuildServiceProvider();
        }

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
            db.Users.Add(new Infrastructure.Model.User() { Id = TestUserConstants.COMMON_CUR_USER_DB_ID, ObjectId = TestUserConstants.COMMON_CUR_USER_OBJECTID, FullName = TestUserConstants.COMMON_CUR_USER_FULL_NAME, EmailAddress = TestUserConstants.COMMON_CUR_USER_EMAIL, UserName = TestUserConstants.COMMON_CUR_USER_UPN  });
            await db.SaveChangesAsync();
            return db;
        }
    }
}
