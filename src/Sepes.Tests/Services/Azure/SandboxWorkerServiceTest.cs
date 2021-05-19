//using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
//using Microsoft.Extensions.DependencyInjection;
//using Sepes.Common.Dto;
//using Sepes.Infrastructure.Service;
//using Sepes.Infrastructure.Service.Interface;
//using Sepes.Common.Util;
//using Sepes.Tests.Mocks;
//using Sepes.Tests.Setup;
//using System;
//using Xunit;

//namespace Sepes.Tests.Services.Azure
//{
//    public class SandboxWorkerServiceTest
//    {
//        public ServiceCollection Services { get; private set; }
//        public ServiceProvider ServiceProvider { get; protected set; }

//        public SandboxWorkerServiceTest()
//        {
//            Services = BasicServiceCollectionFactory.GetServiceCollectionWithInMemory();

//            ServiceProvider = Services.BuildServiceProvider();
//        }

//        [Fact]
//        public async void CreatingAndDeletingSandboxShouldBeOk()
//        {
//            var sandboxWorkerService = ServiceProvider.GetService<ISandboxResourceProvisioningService>();
//            var resourceGroupService = ServiceProvider.GetService<IAzureResourceGroupService>();
//            var diagStorageAccountService = ServiceProvider.GetService<IAzureStorageAccountService>();
//            var networkService = ServiceProvider.GetService<IAzureVNetService>();
//            var nsgService = ServiceProvider.GetService<IAzureNwSecurityGroupService>();


//            var dateString = DateTime.UtcNow.ToString("HH-mm");
//            var shortGuid = Guid.NewGuid().ToString().ToLower().Substring(0, 3);
//            var studyName = $"{SandboxResourceProvisioningService.UnitTestPrefix}-{dateString}-{shortGuid}";

//            try
//            {
//                var tags = AzureResourceTagsFactory.CreateUnitTestTags(studyName);
//                var sandbox = await sandboxWorkerService.CreateBasicSandboxResourcesAsync(1, Region.NorwayEast, studyName, tags);

//                Assert.NotNull(sandbox);
//                Assert.IsType<SandboxWithCloudResourcesDto>(sandbox);

//                Assert.NotNull(sandbox.StudyName);
//                Assert.NotNull(sandbox.ResourceGroupId);
//                Assert.NotNull(sandbox.SandboxName);
//                Assert.NotNull(sandbox.ResourceGroupName);
//                Assert.NotNull(sandbox.DiagnosticsStorage);
//                Assert.NotNull(sandbox.NetworkSecurityGroup);
//                Assert.NotNull(sandbox.VNet);

//                var resourceGroupProvisioningState = await resourceGroupService.GetProvisioningState(sandbox.ResourceGroupName);
//                Assert.NotNull(resourceGroupProvisioningState);
//                Assert.Equal("Succeeded", resourceGroupProvisioningState);

//                var diagStorageAccountState = await diagStorageAccountService.GetProvisioningState(sandbox.ResourceGroupName, sandbox.DiagnosticsStorage.Name);
//                Assert.NotNull(diagStorageAccountState);
//                Assert.Equal("Succeeded", diagStorageAccountState);

//                var networkProvisioningState = await networkService.GetProvisioningState(sandbox.ResourceGroupName, sandbox.VNet.Name);
//                Assert.NotNull(networkProvisioningState);
//                Assert.Equal("Succeeded", networkProvisioningState);

//                var nsgProvisioningState = await nsgService.GetProvisioningState(sandbox.ResourceGroupName, sandbox.NetworkSecurityGroup.Name);
//                Assert.NotNull(nsgProvisioningState);
//                Assert.Equal("Succeeded", nsgProvisioningState);
//            }
//            catch (Exception ex)
//            {
//                throw;
//            }
//            finally
//            {
//                await sandboxWorkerService.NukeUnitTestSandboxes();
//            }
//        }

//    }
//}
