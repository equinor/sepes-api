using Sepes.Common.Response.Sandbox;
using Sepes.RestApi.IntegrationTests.Setup;
using Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets.Sandbox;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Sepes.RestApi.IntegrationTests.Tests
{
    [Collection("Integration tests collection")]
    public class SandboxResourceControllerTests : ControllerTestBase
    {
        public SandboxResourceControllerTests(TestHostFixture testHostFixture)
            : base(testHostFixture)
        {

        }

        [Fact]       
        public async Task ResourceListShouldContainAllResources()
        {
            await WithBasicSeeds();
            var vm = await WithVirtualMachine(true, true, addDatasets: false);
            SetScenario(isAdmin: true);
           
            var sandboxResourcesPreProvisioningResponseWrapper = await _restHelper.Get<List<SandboxResourceLight>>($"api/sandboxes/{vm.SandboxId}/resources");
            SandboxResourceListAsserts.AfterProvisioning(sandboxResourcesPreProvisioningResponseWrapper, vm.ResourceName);
        }

        [Fact]
        public async Task ResourceList_ShouldContain_DeletedResources_IfDeleteOperation_IsNotFinished()
        {
            await WithBasicSeeds();
            var vm = await WithVirtualMachine(true, true, addDatasets: false, deleted: true);
            SetScenario(isAdmin: true);

            var sandboxResourcesPreProvisioningResponseWrapper = await _restHelper.Get<List<SandboxResourceLight>>($"api/sandboxes/{vm.SandboxId}/resources");
            SandboxResourceListAsserts.AfterProvisioning_VmDeleted(sandboxResourcesPreProvisioningResponseWrapper, false, vm.ResourceName);
        }

        [Fact]
        public async Task ResourceList_ShouldNotContain_DeletedResources_IfDeleteOperation_IsFinished()
        {
            await WithBasicSeeds();
            var vm = await WithVirtualMachine(true, true, addDatasets: false, deleted: true, deleteSucceeded: true);
            SetScenario(isAdmin: true);

            var sandboxResourcesPreProvisioningResponseWrapper = await _restHelper.Get<List<SandboxResourceLight>>($"api/sandboxes/{vm.SandboxId}/resources");
            SandboxResourceListAsserts.AfterProvisioning_VmDeleted(sandboxResourcesPreProvisioningResponseWrapper, true, vm.ResourceName);
        }
    }
}
