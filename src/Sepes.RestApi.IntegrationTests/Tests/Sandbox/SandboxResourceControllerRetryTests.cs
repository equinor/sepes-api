using Sepes.Common.Constants;
using Sepes.Common.Constants.CloudResource;
using Sepes.Common.Response.Sandbox;
using Sepes.Infrastructure.Model;
using Sepes.RestApi.IntegrationTests.TestHelpers.Requests;
using Sepes.RestApi.IntegrationTests.Setup;
using Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Sepes.RestApi.IntegrationTests.Tests
{
    [Collection("Integration tests collection")]
    public class SandboxResourceControllerRetryTests : ControllerTestBase
    {
        public SandboxResourceControllerRetryTests(TestHostFixture testHostFixture)
            : base(testHostFixture)
        {

        }

        [Theory]
        [InlineData(false, false, 0)]
        [InlineData(false, true, 0)]
        [InlineData(false, true, 1)]
        [InlineData(false, true, 2)]
        [InlineData(false, true, 3)]
        [InlineData(false, true, 4)]
        [InlineData(false, true, 4, CloudResourceOperationState.ABORTED, 6)]
        [InlineData(false, true, 0, CloudResourceOperationState.FAILED)]
        [InlineData(false, true, 1, CloudResourceOperationState.FAILED)]
        [InlineData(false, true, 2, CloudResourceOperationState.FAILED)]
        [InlineData(false, true, 3, CloudResourceOperationState.FAILED)]
        [InlineData(false, true, 4, CloudResourceOperationState.FAILED)]
        public async Task Retry_AsAdmin_ShouldSucceed(bool studyCreatedByCurrentUser, bool restrictedStudy, int resourcesSucceeded, string statusOfFailedResource = CloudResourceOperationState.ABORTED, int tryCount = 5, int maxTryCount = 5)
        {
            await WithBasicSeeds();
            var sandbox = await WithFailedSandbox(studyCreatedByCurrentUser, restrictedStudy, addDatasets: false, resourcesSucceeded: resourcesSucceeded, statusOfFailedResource: statusOfFailedResource, tryCount: tryCount, maxTryCount: maxTryCount);
            SetScenario(isAdmin: true);

            await PerformTestsExpectSuccess(sandbox.Id, resourcesSucceeded, tryCount, maxTryCount);
        }

        [Theory]
        [InlineData(false, 0)]
        [InlineData(true, 0)]
        [InlineData(true, 1)]
        [InlineData(true, 2)]
        [InlineData(true, 3)]
        [InlineData(true, 4)]
        [InlineData(true, 4, CloudResourceOperationState.ABORTED, 6)]
        [InlineData(true, 0, CloudResourceOperationState.FAILED)]
        [InlineData(true, 1, CloudResourceOperationState.FAILED)]
        [InlineData(true, 2, CloudResourceOperationState.FAILED)]
        [InlineData(true, 3, CloudResourceOperationState.FAILED)]
        [InlineData(true, 4, CloudResourceOperationState.FAILED)]
        public async Task Retry_ToOwnedStudy_AsSponsor_ShouldSucceed(bool restrictedStudy, int resourcesSucceeded, string statusOfFailedResource = CloudResourceOperationState.ABORTED, int tryCount = 5, int maxTryCount = 5)
        {
            await WithBasicSeeds();
            var sandbox = await WithFailedSandbox(true, restrictedStudy, addDatasets: false, resourcesSucceeded: resourcesSucceeded, statusOfFailedResource: statusOfFailedResource, tryCount: tryCount, maxTryCount: maxTryCount);

            SetScenario(isSponsor: true);

            await PerformTestsExpectSuccess(sandbox.Id, resourcesSucceeded, tryCount, maxTryCount);
        }

        [Theory]
        [InlineData(false, StudyRoles.SponsorRep)]
        [InlineData(true, StudyRoles.SponsorRep)]
        [InlineData(false, StudyRoles.VendorAdmin)]
        [InlineData(true, StudyRoles.VendorAdmin)]

        public async Task Retry_HavingCorrectStudyRoles_ShouldSucceed(bool restrictedStudy, string studyRole)
        {
            var RESOURCES_SUCCEEDED = 2;

            await WithBasicSeeds();

            var sandbox = await WithFailedSandbox(false, restrictedStudy, new List<string> { studyRole }, addDatasets: false, resourcesSucceeded: RESOURCES_SUCCEEDED);

            SetScenario();

            await PerformTestsExpectSuccess(sandbox.Id, RESOURCES_SUCCEEDED);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task Retry_NonOwnedStudy_AsSponsor_ShouldFail(bool restrictedStudy)
        {
            var RESOURCES_SUCCEEDED = 2;

            await WithBasicSeeds();

            var sandbox = await WithFailedSandbox(false, restrictedStudy, addDatasets: false, resourcesSucceeded: RESOURCES_SUCCEEDED);

            SetScenario(isEmployee: true, isSponsor: true);

            await PerformTestsExpectAuthFailure(sandbox, RESOURCES_SUCCEEDED);
        }

        [Theory]
        [InlineData(false, StudyRoles.VendorContributor)]
        [InlineData(false, StudyRoles.StudyViewer)]
        [InlineData(true, StudyRoles.VendorContributor)]
        [InlineData(true, StudyRoles.StudyViewer)]
        public async Task Retry_HavingWrongStudyRoles_ShouldFail(bool restrictedStudy, string studyRole)
        {
            var RESOURCES_SUCCEEDED = 2;

            await WithBasicSeeds();

            var sandbox = await WithFailedSandbox(false, restrictedStudy, new List<string> { studyRole }, addDatasets: false, resourcesSucceeded: RESOURCES_SUCCEEDED);

            SetScenario();

            await PerformTestsExpectAuthFailure(sandbox, RESOURCES_SUCCEEDED);
        }

        [Fact]
        public async Task Retry_SuceededSandbox_ShouldFail()
        {
            await WithBasicSeeds();
            var sandbox = await WithSandbox(true, true, addDatasets: false);
            SetScenario(isAdmin: true);
            await AttemptRetryOfSucceededSandbox(sandbox);
        }

        [Fact]
        public async Task Retry_FailedVm_ShouldSucceed()
        {
            var RESOURCES_SUCCEEDED = 5;

            await WithBasicSeeds();
            var vm = await WithFailedVirtualMachine(true, true, addDatasets: false);
            SetScenario(isAdmin: true);
            await PerformTestsExpectSuccess(vm.SandboxId.Value, RESOURCES_SUCCEEDED);          
        }

        [Fact]
        public async Task Retry_FinishedVm_ShouldFail()
        {
            await WithBasicSeeds();
            var vm = await WithVirtualMachine(true, true, addDatasets: false);
            SetScenario(isAdmin: true);
            await AttemptRetryOfSucceededResource(vm.Id);
        }      

        async Task PerformTestsExpectSuccess(int sandboxId, int resourcesSucceeded, int tryCount = CloudResourceConstants.RESOURCE_MAX_TRY_COUNT, int maxTryCount = CloudResourceConstants.RESOURCE_MAX_TRY_COUNT)
        {
            var resourceRetryLink = await GetResourceListAndAssert(sandboxId, resourcesSucceeded, tryCount, maxTryCount);

            //Retry the resource that failed
            var resourceRetryConversation = await GenericPutter.PutAndExpectSuccess<SandboxResourceLight>(_restHelper, resourceRetryLink);
            Assert.Contains(CloudResourceStatus.CREATING, resourceRetryConversation.Response.Content.Status);
        }        

        async Task AttemptRetryOfSucceededSandbox(Sandbox sandbox)
        {
            foreach (var curResource in sandbox.Resources)
            {
                await AttemptRetryOfSucceededResource(curResource.Id);
            }
        }

        async Task AttemptRetryOfSucceededResource(int resourceId)
        {
            var resourceRetryConversation = await GenericPutter.PutAndExpectFailure(_restHelper, GenericPutter.SandboxResourceRetry(resourceId));
            ApiResponseBasicAsserts.ExpectFailureWithMessage(resourceRetryConversation.Response, System.Net.HttpStatusCode.BadRequest, "Could not locate any relevant operation to retry");
        }

        async Task PerformTestsExpectAuthFailure(Sandbox sandbox, int resourcesSucceeded)
        {
            var failingResource = sandbox.Resources[resourcesSucceeded];

            var resourceRetryConversation = await GenericPutter.PutAndExpectFailure(_restHelper, GenericPutter.SandboxResourceRetry(failingResource.Id));
            ApiResponseBasicAsserts.ExpectForbiddenWithMessage(resourceRetryConversation.Response);
        }

        async Task<string> GetResourceListAndAssert(int sandboxId, int resourcesSucceeded, int tryCount = CloudResourceConstants.RESOURCE_MAX_TRY_COUNT, int maxTryCount = CloudResourceConstants.RESOURCE_MAX_TRY_COUNT)
        {
            //Get resources list
            var resourceListConversation = await GenericReader.ReadAndAssertExpectSuccess<List<SandboxResourceLight>>(_restHelper, GenericReader.SandboxResourcesUrl(sandboxId));

            string resourceRetryLink = null;
            var resourceIndex = 0;

            foreach (var curResource in resourceListConversation.Response.Content)
            {
                if (resourceIndex < resourcesSucceeded)
                {
                    Assert.Equal(CloudResourceStatus.OK, curResource.Status);
                    Assert.Null(curResource.RetryLink);
                }
                else if (resourceIndex == resourcesSucceeded)
                {
                    Assert.Contains(CloudResourceStatus.FAILED, curResource.Status);
                    Assert.Contains($"{tryCount}/{maxTryCount}", curResource.Status);

                    Assert.NotNull(curResource.RetryLink);

                    resourceRetryLink = curResource.RetryLink;
                }
                else
                {
                    Assert.Contains(CloudResourceStatus.IN_QUEUE, curResource.Status);
                    Assert.Null(curResource.RetryLink);
                }

                resourceIndex++;
            }

            return resourceRetryLink;
        }
    }
}
