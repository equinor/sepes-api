using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Primitives;
using Sepes.Infrastructure.Model.Automapper;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service;
using System.Collections.Generic;

namespace Sepes.Tests.Setup
{
    public static class BasicServiceCollectionFactory
    {
        public static ServiceCollection GetServiceCollectionWithInMemory()
        {
            var services = new ServiceCollection();


            services.AddDbContext<SepesDbContext>(opt => opt.UseInMemoryDatabase(databaseName: "InMemoryDb"), ServiceLifetime.Scoped, ServiceLifetime.Scoped);
            IConfiguration config = new ConfigurationBuilder()
             .AddJsonFile("appsettings.json", optional: true)
             .AddJsonFile("appsettings.Development.json", optional: true)
             .AddEnvironmentVariables().Build();


            //config.GetSection("ConnectionStrings").Bind(new ConnectionStrings());
            services.AddSingleton<IConfiguration>(config);
            services.AddTransient<IAzureBlobStorageService, AzureBlobStorageService>();
            services.AddTransient<ILogger, NullLogger<string>>();
            services.AddAutoMapper(typeof(AutoMappingConfigs));

            return services;
        }
    }

    public class ConnectionStrings
    {
        public string AzureStorageConnectionString { get; set; }
    }

   
    

}
