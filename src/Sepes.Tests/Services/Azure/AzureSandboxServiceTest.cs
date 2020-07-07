using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestExecutor;
using Sepes.Infrastructure.Service;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Tests.Setup;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Sepes.Tests.Services.Azure
{
    public class AzureSandboxServiceTest
    {
        public ServiceCollection Services { get; private set; }
        public ServiceProvider ServiceProvider { get; protected set; }

        public AzureSandboxServiceTest()
        {
            Services = BasicServiceCollectionFactory.GetServiceCollectionWithInMemory();
            Services.AddTransient<AzureSandboxService>();
            ServiceProvider = Services.BuildServiceProvider();
        }

        [Fact]    
        public async void CreatingSandboxShouldBeOk()
        {
            var sandboxService = ServiceProvider.GetService<AzureSandboxService>();

            var uniqueName = Guid.NewGuid().ToString().ToLower().Substring(0, 5);
            var studyName = $"utest-{uniqueName}";

            await sandboxService.CreateSandboxAsync(studyName, Region.EuropeWest);

            Assert.IsType<int>(result.Id);
        }

    }
}
