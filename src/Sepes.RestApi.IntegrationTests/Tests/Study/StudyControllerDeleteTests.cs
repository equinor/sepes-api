using Sepes.Infrastructure.Constants;
using Sepes.RestApi.IntegrationTests.RequestHelpers;
using Sepes.RestApi.IntegrationTests.Setup;
using Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets;
using System.Threading.Tasks;
using Xunit;

namespace Sepes.RestApi.IntegrationTests.Tests
{
    [Collection("Integration tests collection")]
    public class StudyControllerAuthTests : ControllerTestBase
    {
        public StudyControllerAuthTests(TestHostFixture testHostFixture)
            : base(testHostFixture)
        {

        }

        [Theory]
        [InlineData(false, false, true)]
        [InlineData(false, true, false)]
        [InlineData(false, true, true)]
        [InlineData(true, false, true)]
        [InlineData(true, true, false)]
        [InlineData(true, true, true)]
        public async Task AddStudy_WithRequiredRole_ShouldSucceed(bool isEmployee, bool isAdmin, bool isSponsor)
        {
            SetScenario(isEmployee: isEmployee, isAdmin: isAdmin, isSponsor: isSponsor);

            var studyCreateConversation = await StudyCreator.CreateAndExpectSuccess(_restHelper);

            CreateStudyAsserts.ExpectSuccess(studyCreateConversation.Request, studyCreateConversation.Response);
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(true, true)]
        public async Task AddStudy_WithoutRequiredRole_ShouldFail(bool isEmployee, bool isDatasetAdmin)
        {
            SetScenario(isEmployee: isEmployee, isDatasetAdmin: isDatasetAdmin);

            var studyCreateConversation = await StudyCreator.CreateAndExpectFailure(_restHelper);

             ApiResponseBasicAsserts.ExpectForbiddenWithMessage(studyCreateConversation.Response, "does not have permission to perform operation");
        }

        [Theory]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]

        public async Task DeleteStudy_CreatedByMe_WithRequiredRole_ShouldSucceed(bool isAdmin, bool isSponsor)
        {
            SetScenario(isAdmin: isAdmin, isSponsor: isSponsor);
            await WithBasicSeeds();
            var createdStudy = await WithStudyCreatedByCurrentUser();
            var studyDeleteConversation = await StudyDeleter.DeleteAndExpectSuccess(_restHelper, createdStudy.Id);

            DeleteBasicAsserts.ExpectNoContent(studyDeleteConversation.Response);
        }

        [Theory]
        [InlineData(false, StudyRoles.SponsorRep)]
        [InlineData(true, StudyRoles.StudyOwner)]
        [InlineData(true, StudyRoles.SponsorRep)]
        [InlineData(true, StudyRoles.VendorAdmin)]
        [InlineData(true, StudyRoles.VendorContributor)]
        [InlineData(true, StudyRoles.StudyViewer)]

        public async Task DeleteStudy_CreatedByOther_WithRequiredRole_ShouldSucceed(bool isAdmin, string myRole)
        {
            SetScenario(isAdmin: isAdmin);
            await WithBasicSeeds();
            var createdStudy = await WithStudyCreatedByOtherUser(myRole: myRole);

            var studyDeleteConversation = await StudyDeleter.DeleteAndExpectSuccess(_restHelper, createdStudy.Id);

            DeleteBasicAsserts.ExpectNoContent(studyDeleteConversation.Response);
        }


        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(true, true)]
        public async Task DeleteStudy_CreatedByMe_WithoutRequiredRole_ShouldFail(bool isEmployee, bool isDatasetAdmin)
        {
            SetScenario(isEmployee: isEmployee, isDatasetAdmin: isDatasetAdmin);
            await WithBasicSeeds();
            var createdStudy = await WithStudyCreatedByCurrentUser();

            var studyDeleteConversation = await StudyDeleter.DeleteAndExpectFailure(_restHelper, createdStudy.Id);

            ApiResponseBasicAsserts.ExpectForbiddenWithMessage(studyDeleteConversation.Response, "does not have permission to perform operation");
        }

        [Theory]
        [InlineData(false, false, StudyRoles.VendorAdmin)]
        [InlineData(false, false, StudyRoles.VendorContributor)]
        [InlineData(false, false, StudyRoles.StudyViewer)]

        [InlineData(false, true, StudyRoles.VendorAdmin)]
        [InlineData(false, true, StudyRoles.VendorContributor)]
        [InlineData(false, true, StudyRoles.StudyViewer)]

        [InlineData(true, false, StudyRoles.VendorAdmin)]
        [InlineData(true, false, StudyRoles.VendorContributor)]
        [InlineData(true, false, StudyRoles.StudyViewer)]

        [InlineData(true, true, StudyRoles.VendorAdmin)]
        [InlineData(true, true, StudyRoles.VendorContributor)]
        [InlineData(true, true, StudyRoles.StudyViewer)]

        public async Task DeleteStudy_CreatedByOther_WithoutRequiredRole_ShouldFail(bool sponsor, bool datasetAdmin, string myRole)
        {
            SetScenario(isSponsor: sponsor, isDatasetAdmin: datasetAdmin);
            await WithBasicSeeds();
            var createdStudy = await WithStudyCreatedByOtherUser(myRole: myRole);

            var studyDeleteConversation = await StudyDeleter.DeleteAndExpectFailure(_restHelper, createdStudy.Id);

             ApiResponseBasicAsserts.ExpectForbiddenWithMessage(studyDeleteConversation.Response, "does not have permission to perform operation");
        }
    }
}
