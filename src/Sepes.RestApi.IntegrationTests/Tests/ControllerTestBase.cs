using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Model;
using Sepes.RestApi.IntegrationTests.Dto;
using Sepes.RestApi.IntegrationTests.Setup;
using Sepes.RestApi.IntegrationTests.Setup.Scenarios;
using Sepes.RestApi.IntegrationTests.Setup.Seeding;
using Sepes.RestApi.IntegrationTests.TestHelpers;
using System.Threading.Tasks;
using Xunit;

namespace Sepes.RestApi.IntegrationTests
{
    [Collection("Integration tests collection")]
    public class ControllerTestBase : IAsyncLifetime
    {
        protected readonly TestHostFixture _testHostFixture;
        protected RestHelper _restHelper;

        public ControllerTestBase(TestHostFixture testHostFixture)
        {
            _testHostFixture = testHostFixture;
            _restHelper = new RestHelper(testHostFixture.Client);
        }

        protected void SetScenario(bool isEmployee = false, bool isAdmin = false, bool isSponsor = false, bool isDatasetAdmin = false)
        {
            var azureServices = new MockedAzureServiceSets();
            _testHostFixture.SetScenario(azureServices, isEmployee, isAdmin, isSponsor, isDatasetAdmin);
            _restHelper = new RestHelper(_testHostFixture.Client);
        }

        public Task InitializeAsync() => SliceFixture.ResetCheckpoint();

        public Task DisposeAsync() => Task.CompletedTask;

        protected async Task WithBasicSeeds()
        {
            await RegionSeed.Seed();
            await UserSeed.Seed();
        }

        protected async Task WithUserSeeds()
        {
            await UserSeed.Seed();
        }

        protected async Task<Study> WithStudy(bool createdByCurrentUser, bool restricted = false, string studyRole = null, bool addDatasets = false)
        {
            return createdByCurrentUser ? await StudySeed.CreatedByCurrentUser(restricted: restricted, currentUserRole: studyRole, addDatasets: addDatasets) : await StudySeed.CreatedByOtherUser(restricted: restricted, currentUserRole: studyRole, addDatasets: addDatasets);
        }

        protected async Task<Sandbox> WithSandbox(bool createdByCurrentUser, bool restricted = false, string studyRole = null, SandboxPhase phase = SandboxPhase.Open, bool addDatasets = false)
        {
            var study = await WithStudy(createdByCurrentUser, restricted, studyRole, addDatasets: addDatasets);
            var sandbox = await SandboxSeed.Create(study, phase: phase, addDatasets: addDatasets);
            sandbox.Study = study;
            study.Sandboxes.Add(sandbox);
            return sandbox;
        }

        protected async Task<Sandbox> WithFailedSandbox(bool createdByCurrentUser, bool restricted = false, string studyRole = null, bool addDatasets = false, int resourcesSucceeded = 0, string statusOfFailedResource = CloudResourceOperationState.FAILED, int tryCount = CloudResourceConstants.RESOURCE_MAX_TRY_COUNT, int maxTryCount = CloudResourceConstants.RESOURCE_MAX_TRY_COUNT)
        {
            var study = await WithStudy(createdByCurrentUser, restricted, studyRole, addDatasets: addDatasets);
            var sandbox = await SandboxSeed.CreateFailing(study, phase: SandboxPhase.Open, resourcesSucceeded: resourcesSucceeded, statusOfFailedResource: statusOfFailedResource, tryCount: tryCount, maxTryCount: maxTryCount, addDatasets: addDatasets);
            sandbox.Study = study;
            study.Sandboxes.Add(sandbox);
            return sandbox;
        }

        protected async Task<CloudResource> WithVirtualMachine(bool createdByCurrentUser, bool restricted = false, string studyRole = null, SandboxPhase phase = SandboxPhase.Open, bool addDatasets = false)
        {
            var sandbox = await WithSandbox(createdByCurrentUser, restricted, studyRole, phase, addDatasets: addDatasets);
            var vm = await VirtualMachineSeed.Create(sandbox);
            sandbox.Resources.Add(vm);
            vm.Sandbox = sandbox;
            return vm;
        }

        protected async Task<CloudResource> WithFailedVirtualMachine(bool createdByCurrentUser, bool restricted = false, string studyRole = null, SandboxPhase phase = SandboxPhase.Open, bool addDatasets = false)
        {
            var sandbox = await WithSandbox(createdByCurrentUser, restricted, studyRole, phase, addDatasets: addDatasets);
            var vm = await VirtualMachineSeed.CreateFailed(sandbox);
            sandbox.Resources.Add(vm);
            vm.Sandbox = sandbox;
            return vm;
        }

        protected async Task<Study> WithStudyCreatedByCurrentUser(bool restricted = false, string studyRole = null)
        {
            return await WithStudy(true, restricted, studyRole);
        }

        protected async Task<Study> WithStudyCreatedByOtherUser(bool restricted = false, string studyRole = null)
        {
            return await WithStudy(false, restricted, studyRole);
        }

        protected async Task<ApiResponseWrapper> ProcessWorkQueue()
        {
            //SetUserType(isAdmin: true); //If this test will be ran as non-admins, must find a way to set admin before running this

            var responseWrapper = await _restHelper.Get("api/provisioningqueue/lookforwork");

            Assert.Equal(System.Net.HttpStatusCode.OK, responseWrapper.StatusCode);

            return responseWrapper;
        }
    }
}
