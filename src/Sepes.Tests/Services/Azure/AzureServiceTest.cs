﻿using Azure.ResourceManager.Network.Models;
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
            Services.AddTransient<IAzureResourceGroupService, AzureResourceGroupService>();
            Services.AddTransient<IAzureNwSecurityGroupService, AzureNwSecurityGroupService>();
            Services.AddTransient<IAzureBastionService, AzureBastionService>();
            Services.AddTransient<IAzureVNetService, AzureVNetService>();
            Services.AddTransient<AzureService>();
            ServiceProvider = Services.BuildServiceProvider();
        }

        [Fact]    
        public async void CreatingAndDeletingSandboxShouldBeOk()
        {
            var sandboxService = ServiceProvider.GetService<AzureService>();

            var dateString = DateTime.UtcNow.ToString("yyyyMMddHHmm");
            var shortGuid = Guid.NewGuid().ToString().ToLower().Substring(0, 5);
            var studyName = $"utest-{dateString}-{shortGuid}";

            string sandboxName = null;
            string resourceGroupName = null;
       

            try
            {
                var sandbox = await sandboxService.CreateSandboxAsync(studyName, Region.EuropeWest);

                sandboxName = sandbox.SandboxName;
                resourceGroupName = sandbox.ResourceGroupName;

                Assert.NotNull(sandbox);
                Assert.IsType<AzureSandboxDto>(sandbox);

                Assert.NotNull(sandbox.StudyName);
                Assert.NotNull(sandbox.SandboxName);
                Assert.NotNull(sandbox.ResourceGroupName);

                Assert.NotNull(sandbox.VNet);
            }
            catch (Exception ex)
            {

                int i = 0;
            }
            finally
            {
                await sandboxService.NukeSandbox(studyName, sandboxName, resourceGroupName);
            }          
        }

    }
}