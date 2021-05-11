using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sepes.Common.Constants;
using Sepes.Infrastructure.Interface;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service;
using Sepes.Infrastructure.Service.Interface;
using Sepes.RestApi.IntegrationTests.Services;
using Sepes.Tests.Common.Extensions;
using Sepes.Tests.Common.Mocks.Azure;
using Sepes.Tests.Common.ServiceMocks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sepes.RestApi.IntegrationTests.Setup
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        readonly bool _isEmployee;
        readonly bool _isAdmin;
        readonly bool _isSponsor;
        readonly bool _isDatasetAdmin;

        readonly IMockServicesForScenarioProvider _mockServicesForScenarioProvider;

        public CustomWebApplicationFactory(IMockServicesForScenarioProvider mockServicesForScenarioProvider = null, bool isEmployee = false, bool isAdmin = false, bool isSponsor = false, bool isDatasetAdmin = false)
            :base()
        {
            _isEmployee = isEmployee;
            _isAdmin = isAdmin;
            _isSponsor = isSponsor;
            _isDatasetAdmin = isDatasetAdmin;
            _mockServicesForScenarioProvider = mockServicesForScenarioProvider;
        }     

        //Inspired by: https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-3.0#customize-webapplicationfactory
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            //Environment.SetEnvironmentVariable("SEPES_IS_INTEGRATION_TEST", "true");

            builder.ConfigureTestServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbContextOptions<SepesDbContext>));

                services.Remove(descriptor);
              
                services.AddSingleton<IPrincipalService>(new PrincipalServiceMock(_isEmployee, _isAdmin, _isSponsor, _isDatasetAdmin));
                services.AddScoped<ICurrentUserService, CurrentUserServiceMock>();
                services.AddScoped<IAzureUserService, AzureUserServiceMock>();

                services.SwapTransientWithSingleton<IAzureQueueService, AzureQueueServiceMock>();

                if (_mockServicesForScenarioProvider != null)
                {
                    _mockServicesForScenarioProvider.RegisterServices(services);
                }                            

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

                var dbConnectionString = ConnectionStringUtil.GetDatabaseConnectionString(configuration);

                services.AddDbContext<SepesDbContext>(options =>
                    options.UseSqlServer(
                        dbConnectionString,
                        options =>
                        {
                            options.MigrationsAssembly("Sepes.Infrastructure");
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

            builder.ConfigureAppConfiguration((context, configBuilder) => {

                configBuilder.AddInMemoryCollection(
                           new Dictionary<string, string>
                           {
                               [ConfigConstants.IS_INTEGRATION_TEST] = "true",
                               ["AllowCorsDomains"] = "http://localhost:80",
                               ["CostAllocationTypeTagName"] = "INTTEST-CostAllocationType",
                               ["CostAllocationCodeTagName"] = "INTTEST-CostAllocationCode"
                           });


            });
        }
    }
}
