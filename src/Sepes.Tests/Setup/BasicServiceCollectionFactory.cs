using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sepes.Infrastructure.Interface;
using Sepes.Infrastructure.Model.Automapper;
using Sepes.Infrastructure.Model.Config;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service;
using Sepes.Infrastructure.Service.Azure;
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
            services.AddTransient<CloudResourceOperationReadService>();            
            services.AddTransient<IRequestIdService, HasRequestIdMock>();
            //services.AddTransient<IStudyParticipantCreateService, StudyParticipantLookupService>();

            //Sepes Services
            services.AddTransient<ICloudResourceReadService, CloudResourceReadService>();
            services.AddTransient<IVariableService, VariableService>();
            services.AddTransient<IStudyReadService, StudyReadService>();

            //Resource provisioning services
            services.AddSingleton<IProvisioningQueueService, ProvisioningQueueService>();
            services.AddTransient<ICloudResourceOperationReadService, CloudResourceOperationReadService>();
            services.AddTransient<IResourceProvisioningService, ResourceProvisioningService>();

            //Azure resource services
            services.AddTransient<IAzureResourceGroupService, AzureResourceGroupService>();
            services.AddTransient<IAzureNetworkSecurityGroupService, AzureNetworkSecurityGroupService>();
            services.AddTransient<IAzureBastionService, AzureBastionService>();
            services.AddTransient<IAzureVNetService, AzureVNetService>();
            services.AddTransient<IAzureVmService, AzureVmService>();
            services.AddTransient<IAzureQueueService, AzureQueueServiceMock>();
            services.AddTransient<IAzureStorageAccountService, AzureStorageAccountService>();
            services.AddTransient<IAzureRoleAssignmentService, AzureRoleAssignmentService>();


            return services;
        } 
    } 
}
