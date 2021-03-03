using Sepes.Infrastructure.Constants;
using Sepes.RestApi.IntegrationTests.RequestHelpers;
using Sepes.RestApi.IntegrationTests.Setup;
using Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets;
using Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets.Sandbox;
using System.Threading.Tasks;
using Xunit;

namespace Sepes.RestApi.IntegrationTests.Tests
{
    [Collection("Integration tests collection")]
    public class SandboxControllerCreateAndRemoveTests : ControllerTestBase
    {
        public SandboxControllerCreateAndRemoveTests(TestHostFixture testHostFixture)
            : base(testHostFixture)
        {
           
        }    

        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public async Task AddAndRemoveSandbox_AsAdmin_ShouldSucceed(bool studyCreatedByCurrentUser, bool restrictedStudy)
        {
            await WithBasicSeeds();
            SetScenario(isAdmin: true);           

            var study = studyCreatedByCurrentUser ? await WithStudyCreatedByCurrentUser(restrictedStudy) : await WithStudyCreatedByOtherUser(restrictedStudy);

            await PerformTestsExpectSuccess(study.Id);         
        }

        [Theory]       
        [InlineData(false)]
        [InlineData(true)]
        public async Task AddAndRemove_ToOwnedStudy_AsSponsor_ShouldSucceed(bool restrictedStudy)
        {
            await WithBasicSeeds();

            SetScenario(isSponsor: true);

            var study = await WithStudyCreatedByCurrentUser(restrictedStudy);

            await PerformTestsExpectSuccess(study.Id);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task AddAndRemove_ToNonOwnedStudy_AsSponsor_ShouldFail(bool restrictedStudy)
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
        public async Task AddAndRemove_HavingCorrectStudyRoles_ShouldSucceed(bool restrictedStudy, string studyRole)
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
        public async Task AddAndRemove_HavingWrongStudyRoles_ShouldFail(bool restrictedStudy, string studyRole)
        {
            await WithBasicSeeds();

            SetScenario();

            var study = await WithStudyCreatedByOtherUser(restrictedStudy, studyRole);

            await PerformTestsExpectFailure(study.Id);
        }

        async Task PerformTestsExpectSuccess(int studyId)
        {
            var sandboxCreateConversation = await SandboxCreator.CreateAndExpectSuccess(_restHelper, studyId);
            SandboxDetailsAsserts.NewlyCreatedExpectSuccess(sandboxCreateConversation.Request, sandboxCreateConversation.Response);           

            var sandboxRemoveConversation = await SandboxDeleter.DeleteAndExpectSuccess(_restHelper, studyId);
            DeleteBasicAsserts.ExpectNoContent(sandboxRemoveConversation.Response);
        }      

        async Task PerformTestsExpectFailure(int studyId)
        {
            var sandboxCreateConversation = await SandboxCreator.CreateAndExpectFailure(_restHelper, studyId);
            ApiResponseBasicAsserts.ExpectForbiddenWithMessage(sandboxCreateConversation.Response);
        }            
    }
}
