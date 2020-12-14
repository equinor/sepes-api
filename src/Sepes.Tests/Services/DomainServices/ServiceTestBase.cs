using Microsoft.Extensions.DependencyInjection;

using Sepes.Infrastructure.Model.Context;
using Sepes.Tests.Constants;
using Sepes.Tests.Setup;

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

        protected SepesDbContext ClearTestDatabase()
        {
            var db = GetDatabase();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            return db;
        }

        protected SepesDbContext ClearTestDatabaseAddUser()
        {
            var db = ClearTestDatabase();
            db.Users.Add(new Infrastructure.Model.User() { Id = UserConstants.COMMON_CUR_USER_DB_ID, ObjectId = UserConstants.COMMON_CUR_USER_OBJECTID, FullName = UserConstants.COMMON_CUR_USER_FULL_NAME, EmailAddress = UserConstants.COMMON_CUR_USER_EMAIL, UserName = UserConstants.COMMON_CUR_USER_UPN  });
            db.SaveChangesAsync();
            return db;
        }
    }
}
