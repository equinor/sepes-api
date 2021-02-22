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

        protected async Task<Study> WithStudyCreatedByCurrentUser(bool restricted = false)
        {
            return await StudySeed.CreatedByCurrentUser(restricted: restricted);
        }

        protected async Task<Study> WithStudyCreatedByOtherUser(bool restricted = false, string myRole = null)
        {
            return await StudySeed.CreatedByOtherUser(restricted: restricted, currentUserRole: myRole);
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
