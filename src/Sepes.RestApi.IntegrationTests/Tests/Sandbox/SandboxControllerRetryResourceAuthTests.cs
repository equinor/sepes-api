using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Response.Sandbox;
using Sepes.RestApi.IntegrationTests.RequestHelpers;
using Sepes.RestApi.IntegrationTests.Setup;
using Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets;
using Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets.Sandbox;
using System.Threading.Tasks;
using Xunit;

namespace Sepes.RestApi.IntegrationTests.Tests
{
    [Collection("Integration tests collection")]
    public class SandboxControllerRetryResourceAuthTests : ControllerTestBase
    {
        public SandboxControllerRetryResourceAuthTests(TestHostFixture testHostFixture)
            : base(testHostFixture)
        {
           
        }    

        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public async Task RetryCreateSandbox_AsAdmin_ShouldSucceed(bool createdByCurrentUser, bool restrictedStudy)
        {
            await WithBasicSeeds();
            SetScenario(isAdmin: true);

            var sandbox = await WithSandbox(createdByCurrentUser, restrictedStudy);

            await PerformTestsExpectSuccess(sandbox.Id);         
        }

        [Theory]       
        [InlineData(false)]
        [InlineData(true)]
        public async Task RetryCreateSandbox_ToOwnedStudy_AsSponsor_ShouldSucceed(bool restrictedStudy)
        {
            await WithBasicSeeds();

            SetScenario(isSponsor: true);

            var study = await WithStudyCreatedByCurrentUser(restrictedStudy);

            await PerformTestsExpectSuccess(study.Id);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task RetryCreateSandbox_ToNonOwnedStudy_AsSponsor_ShouldFail(bool restrictedStudy)
        {
            await WithBasicSeeds();

            SetScenario(isSponsor: true);

            var study = await WithStudyCreatedByOtherUser(restrictedStudy);

            await PerformTestsExpectFailure(study.Id);
        }

        [Theory]
        [InlineData(false, StudyRoles.SponsorRep)]
        [InlineData(false, StudyRoles.VendorAdmin)]
        [InlineData(true, StudyRoles.SponsorRep)]
        [InlineData(true, StudyRoles.VendorAdmin)]      
        public async Task RetryCreateSandbox_HavingCorrectStudyRoles_ShouldSucceed(bool restrictedStudy, string studyRole)
        {
            await WithBasicSeeds();

            SetScenario();

            var study = await WithStudyCreatedByOtherUser(restrictedStudy, studyRole);

            await PerformTestsExpectSuccess(study.Id);
        }

        [Theory]
        [InlineData(false, StudyRoles.VendorContributor)]
        [InlineData(false, StudyRoles.StudyViewer)] 
        [InlineData(true, StudyRoles.VendorContributor)]
        [InlineData(true, StudyRoles.StudyViewer)]
        public async Task RetryCreateSandbox_HavingWrongStudyRoles_ShouldFail(bool restrictedStudy, string studyRole)
        {
            await WithBasicSeeds();

            SetScenario();

            var study = await WithStudyCreatedByOtherUser(restrictedStudy, studyRole);

            await PerformTestsExpectFailure(study.Id);
        }

        async Task PerformTestsExpectSuccess(int resourceId)
        {
            var sandboxRetryConversation = await GenericPutter.PutAndExpectSuccess<SandboxResourceLight>(_restHelper, GenericPutter.SandboxResourceRetry(resourceId));
            SandboxResourceRetryAsserts.ExpectSuccess(sandboxRetryConversation.Response, "creating");
            //get from database
            //Asser correct
        }      

        async Task PerformTestsExpectFailure(int studyId)
        {
            var sandboxCreateConversation = await SandboxCreator.CreateAndExpectFailure(_restHelper, studyId);
            ApiResponseBasicAsserts.ExpectForbiddenWithMessage(sandboxCreateConversation.Response);
        }            
    }
}
