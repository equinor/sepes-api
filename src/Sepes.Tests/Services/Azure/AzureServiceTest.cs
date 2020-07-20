using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Extensions.DependencyInjection;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Service;
using Sepes.Infrastructure.Service.Azure;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Util;
using Sepes.Tests.Setup;
using System;
using Xunit;

namespace Sepes.Tests.Services.Azure
{
    public class AzureServiceTest
    {
        public ServiceCollection Services { get; private set; }
        public ServiceProvider ServiceProvider { get; protected set; }

        public AzureServiceTest()
        {
            Services = BasicServiceCollectionFactory.GetServiceCollectionWithInMemory();        
            Services.AddTransient<ISandboxResourceService, SandboxResourceService>();
            Services.AddTransient<IAzureResourceGroupService, AzureResourceGroupService>();
            Services.AddTransient<IAzureNwSecurityGroupService, AzureNwSecurityGroupService>();
            Services.AddTransient<IAzureBastionService, AzureBastionService>();
            Services.AddTransient<IAzureVNetService, AzureVNetService>();
            Services.AddTransient<IAzureVMService, AzureVMService>();
            Services.AddTransient<IAzureStorageAccountService, AzureStorageAccountService>();
            Services.AddTransient<SandboxWorkerService>();
            ServiceProvider = Services.BuildServiceProvider();
        }

        [Fact]    
        public async void CreatingAndDeletingSandboxShouldBeOk()
        {
            var sandboxService = ServiceProvider.GetService<SandboxWorkerService>();

            var dateString = DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm");
            var shortGuid = Guid.NewGuid().ToString().ToLower().Substring(0, 5);
            var studyName = $"{SandboxWorkerService.UnitTestPrefix}-{dateString}-{shortGuid}";

            string sandboxName = null;
            string resourceGroupName = null;
       

            try
            {
                var tags = AzureResourceTagsFactory.CreateUnitTestTags(studyName);             
                var sandbox = await sandboxService.CreateBasicSandboxResourcesAsync(Region.NorwayEast, studyName, tags);       

                sandboxName = sandbox.SandboxName;
                resourceGroupName = sandbox.ResourceGroupName;

                Assert.NotNull(sandbox);
                Assert.IsType<AzureSandboxDto>(sandbox);

                Assert.NotNull(sandbox.StudyName);
                Assert.NotNull(sandbox.SandboxName);
                Assert.NotNull(sandbox.ResourceGroupName);

                Assert.NotNull(sandbox.VNet);
            }
            catch (Exception)
            {
                throw; 
            }
            finally
            {
                await sandboxService.NukeUnitTestSandboxes();
            }          
        }

    }
}
