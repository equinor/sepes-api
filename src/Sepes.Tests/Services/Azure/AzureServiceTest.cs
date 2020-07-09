using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Extensions.DependencyInjection;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Service;
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
            //Services.AddTransient<IAzure>();
            Services.AddTransient<CloudResourceService>();
            Services.AddTransient<AzureResourceGroupService>();
            Services.AddTransient<AzureService>();
            ServiceProvider = Services.BuildServiceProvider();
        }

        [Fact]    
        public async void CreatingAndDeletingSandboxShouldBeOk()
        {
            var sandboxService = ServiceProvider.GetService<AzureService>();

            var uniqueName = Guid.NewGuid().ToString().ToLower().Substring(0, 5);
            var studyName = $"utest-{uniqueName}";

            var sandbox = await sandboxService.CreateSandboxAsync(studyName, Region.EuropeWest);

            Assert.NotNull(sandbox);
            Assert.IsType<AzureSandboxDto>(sandbox);

            Assert.NotNull(sandbox.StudyName);
            Assert.NotNull(sandbox.SandboxName);
            Assert.NotNull(sandbox.ResourceGroupName);

            await sandboxService.NukeSandbox(studyName, sandbox.SandboxName, sandbox.ResourceGroupName);

           
        }

    }
}
