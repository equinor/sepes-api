using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sepes.Infrastructure.Interface;
using Sepes.Infrastructure.Model.Context;
using System;
using System.Linq;

namespace Sepes.API.IntegrationTests.Setup
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        //Inspired by: https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-3.0#customize-webapplicationfactory
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            string connectionStringSetting = "SqlDatabaseIntegrationTests";
            builder.ConfigureTestServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbContextOptions<SepesDbContext>));

                services.Remove(descriptor);

                services.AddScoped<ICurrentUserService, IntegrationTestUserService>();
                //services.AddScoped<IAzureADUsersService, IntegrationTestAzureADUService>();
                services.AddAuthentication("IntegrationTest")
                    .AddScheme<AuthenticationSchemeOptions, IntegrationTestAuthenticationHandler>(
                      "IntegrationTest",
                      options => { }
                    );

                IConfiguration configuration;
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    configuration = scopedServices.GetRequiredService<IConfiguration>();
                }

                if (string.IsNullOrEmpty(configuration.GetConnectionString(connectionStringSetting)))
                    throw new ArgumentException($"Could not find a connection string in any configuration provider for {connectionStringSetting}");

                services.AddDbContext<SepesDbContext>(options =>
                    options.UseSqlServer(
                        configuration.GetConnectionString(connectionStringSetting),
                        options =>
                        {
                            options.MigrationsAssembly("Thelma.API");
                            options.EnableRetryOnFailure(10, TimeSpan.FromSeconds(10), null);
                        }
                    ));

                sp = services.BuildServiceProvider();
                SepesDbContext dbContext;
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    dbContext = scopedServices.GetRequiredService<SepesDbContext>();
                    dbContext.Database.Migrate();
                }
            });
        }
    }
}
