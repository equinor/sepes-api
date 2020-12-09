using Microsoft.Extensions.DependencyInjection;
using Sepes.Infrastructure.Model.Context;
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

        protected SepesDbContext GetFreshTestDatabase()
        {
            var db = GetDatabase();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            return db;
        } 
    }
}
