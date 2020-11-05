using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sepes.Infrastructure.Interface;
using Sepes.Infrastructure.Model.Automapper;
using Sepes.Infrastructure.Model.Config;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Tests.Mocks;
using Sepes.Tests.Mocks.Azure;

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

            if (!string.IsNullOrWhiteSpace(config[ConfigConstants.APPI_KEY]))
            {    //https://docs.microsoft.com/en-us/azure/azure-monitor/app/worker-service         
                services.AddApplicationInsightsTelemetryWorkerService(config[ConfigConstants.APPI_KEY]);
            }
            services.AddLogging();
            //services.AddTransient<ILogger, NullLogger<string>>();
            services.AddAutoMapper(typeof(AutoMappingConfigs));
            services.AddTransient<SandboxResourceOperationService>();
            services.AddTransient<IUserService, UserServiceMock>();
            services.AddTransient<IRequestIdService, HasRequestIdMock>();
            services.AddTransient<IStudyParticipantService, StudyParticipantService>();

            //Sepes Services
            services.AddTransient<ISandboxResourceService, SandboxResourceService>();
            services.AddTransient<IVariableService, VariableService>();
            services.AddTransient<IStudyService, StudyService>();

            //Resource provisioning services
            services.AddSingleton<IProvisioningQueueService, ProvisioningQueueService>();
            services.AddTransient<ISandboxResourceOperationService, SandboxResourceOperationService>();
            services.AddTransient<ISandboxResourceProvisioningService, SandboxResourceProvisioningService>();

            //Azure resource services
            services.AddTransient<IAzureResourceGroupService, AzureResourceGroupServiceMock>();
            services.AddTransient<IAzureNetworkSecurityGroupService, AzureNetworkSecurityGroupService>();
            services.AddTransient<IAzureBastionService, AzureBastionService>();
            services.AddTransient<IAzureVNetService, AzureVNetService>();
            services.AddTransient<IAzureVMService, AzureVMService>();
            services.AddTransient<IAzureQueueService, AzureQueueServiceMock>();
            services.AddTransient<IAzureStorageAccountService, AzureStorageAccountService>();                 

            return services;
        } 
    } 
}
