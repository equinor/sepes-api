using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sepes.Infrastructure.Model.Automapper;
using Sepes.Infrastructure.Model.Context;


namespace Sepes.Tests.Setup
{
    public static class BasicServiceCollectionFactory
    {
        public static ServiceCollection GetServiceCollectionWithInMemory()
        {
            var services = new ServiceCollection();
            services.AddDbContext<SepesDbContext>(opt => opt.UseInMemoryDatabase(databaseName: "InMemoryDb"),
             ServiceLifetime.Scoped,
             ServiceLifetime.Scoped);

            services.AddAutoMapper(typeof(AutoMappingConfigs));

            return services;
        }
    }
}
