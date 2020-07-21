using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Extensions.DependencyInjection;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Service;
using Sepes.Infrastructure.Util;
using Sepes.Tests.Setup;
using System;
using Xunit;

namespace Sepes.Tests.Services.Azure
{
    public class SandboxWorkerServiceTest
    {
        public ServiceCollection Services { get; private set; }
        public ServiceProvider ServiceProvider { get; protected set; }

        public SandboxWorkerServiceTest()
        {
            Services = BasicServiceCollectionFactory.GetServiceCollectionWithInMemory();        
            Services.AddTransient<ISandboxResourceService, SandboxResourceService>();
            Services.AddTransient<IAzureResourceGroupService, AzureResourceGroupService>();
            Services.AddTransient<IAzureNwSecurityGroupService, AzureNwSecurityGroupService>();
            Services.AddTransient<IAzureBastionService, AzureBastionService>();
            Services.AddTransient<IAzureVNetService, AzureVNetService>();
            Services.AddTransient<IAzureVMService, AzureVMService>();
            Services.AddTransient<IAzureQueueService, AzureQueueService>();
            Services.AddTransient<IVariableService, VariableService>();
            Services.AddTransient<IAzureStorageAccountService, AzureStorageAccountService>();
            Services.AddTransient<SandboxWorkerService>();
            ServiceProvider = Services.BuildServiceProvider();
        }

        [Fact]    
        public async void CreatingAndDeletingSandboxShouldBeOk()
        {
            var sandboxWorkerService = ServiceProvider.GetService<SandboxWorkerService>();
            var resourceGroupService = ServiceProvider.GetService<IAzureResourceGroupService>();
            var diagStorageAccountService = ServiceProvider.GetService<IAzureStorageAccountService>();
            var networkService = ServiceProvider.GetService<IAzureVNetService>();
            var nsgService = ServiceProvider.GetService<IAzureNwSecurityGroupService>();
          

            var dateString = DateTime.UtcNow.ToString("HH-mm");
            var shortGuid = Guid.NewGuid().ToString().ToLower().Substring(0, 3);
            var studyName = $"{SandboxWorkerService.UnitTestPrefix}-{dateString}-{shortGuid}";
            
            try
            {
                var tags = AzureResourceTagsFactory.CreateUnitTestTags(studyName);             
                var sandbox = await sandboxWorkerService.CreateBasicSandboxResourcesAsync(Region.NorwayEast, studyName, tags);  

                Assert.NotNull(sandbox);
                Assert.IsType<AzureSandboxDto>(sandbox);

                Assert.NotNull(sandbox.StudyName);
                Assert.NotNull(sandbox.ResourceGroupId);
                Assert.NotNull(sandbox.SandboxName);
                Assert.NotNull(sandbox.ResourceGroupName);
                Assert.NotNull(sandbox.DiagnosticsStorage);
                Assert.NotNull(sandbox.NetworkSecurityGroup);                
                Assert.NotNull(sandbox.VNet);

                var resourceGroupProvisioningState = await resourceGroupService.GetProvisioningState(sandbox.ResourceGroupName);
                Assert.NotNull(resourceGroupProvisioningState);
                Assert.Equal("Succeeded", resourceGroupProvisioningState);

                var diagStorageAccountState = await diagStorageAccountService.GetProvisioningState(sandbox.ResourceGroupName, sandbox.DiagnosticsStorage.Name);
                Assert.NotNull(diagStorageAccountState);
                Assert.Equal("Succeeded", diagStorageAccountState);

                var networkProvisioningState = await networkService.GetProvisioningState(sandbox.ResourceGroupName, sandbox.VNet.Name);
                Assert.NotNull(networkProvisioningState);
                Assert.Equal("Succeeded", networkProvisioningState);

                var nsgProvisioningState = await nsgService.GetProvisioningState(sandbox.ResourceGroupName, sandbox.NetworkSecurityGroup.Name);
                Assert.NotNull(nsgProvisioningState);
                Assert.Equal("Succeeded", nsgProvisioningState);            
            }
            catch (Exception ex)
            {            
                throw; 
            }
            finally
            {
                await sandboxWorkerService.NukeUnitTestSandboxes();
            }          
        }

    }
}
