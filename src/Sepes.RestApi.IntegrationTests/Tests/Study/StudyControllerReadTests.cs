using Sepes.Infrastructure.Constants;
using Sepes.RestApi.IntegrationTests.RequestHelpers;
using Sepes.RestApi.IntegrationTests.Setup;
using Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets;
using System.Threading.Tasks;
using Xunit;

namespace Sepes.RestApi.IntegrationTests.Tests
{
    [Collection("Integration tests collection")]
    public class StudyControllerReadTests : ControllerTestBase
    {
        public StudyControllerReadTests(TestHostFixture testHostFixture)
            : base(testHostFixture)
        {

        }

        [Theory]
        [InlineData(false, false, false, true)]
        [InlineData(false, false, true, false)]
        [InlineData(false, true, false, false)]
        [InlineData(true, false, false, true)]
        [InlineData(true, false, true, false)]         
        public async Task Read_Study_CreatedByMe_ShouldSucceed(bool restrictedStudy, bool isEmployee, bool isAdmin, bool isSponsor)
        {
            SetScenario(isEmployee: isEmployee, isAdmin: isAdmin, isSponsor: isSponsor);
            await WithBasicSeeds();
            var createdStudy = await WithStudyCreatedByCurrentUser(restricted: restrictedStudy);

            var studyCreateConversation = await StudyReader.ReadAndExpectSuccess(_restHelper, createdStudy.Id);

            ReadStudyAsserts.ExpectSuccess(studyCreateConversation.Response);
        }

        [Theory]
        [InlineData(false, true, false, false, null)]
        [InlineData(false, false, true, false, null)]

        //Sponsor needs relevant role
        [InlineData(false, false, false, true, StudyRoles.SponsorRep)]
        [InlineData(false, false, false, true, StudyRoles.VendorAdmin)]
        [InlineData(false, false, false, true, StudyRoles.VendorContributor)]
        [InlineData(false, false, false, true, StudyRoles.StudyViewer)]

        [InlineData(true, false, true, false, null)]
        [InlineData(true, true, false, true, StudyRoles.SponsorRep)]
        [InlineData(true, true, false, true, StudyRoles.VendorAdmin)]
        [InlineData(true, true, false, true, StudyRoles.VendorContributor)]
        [InlineData(true, true, false, true, StudyRoles.StudyViewer)]    
        public async Task Read_Study_CreatedByOther_ShouldSucceed(bool restrictedStudy, bool employee, bool isAdmin, bool isSponsor, string roleName)
        {
            SetScenario(isEmployee: employee, isAdmin: isAdmin, isSponsor: isSponsor);
            await WithBasicSeeds();
            var createdStudy = await WithStudyCreatedByOtherUser(restricted: restrictedStudy, roleName);

            var studyCreateConversation = await StudyReader.ReadAndExpectSuccess(_restHelper, createdStudy.Id);

            ReadStudyAsserts.ExpectSuccess(studyCreateConversation.Response);
        }


        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(true, true)]
        public async Task Read_Study_CreatedByOther_WithoutRelevantRoles_ShouldFail(bool restrictedStudy, bool employee, bool isAdmin, bool isSponsor, string roleName)
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
