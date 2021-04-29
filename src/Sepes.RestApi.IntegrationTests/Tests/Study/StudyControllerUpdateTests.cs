using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Study;
using Sepes.RestApi.IntegrationTests.RequestHelpers;
using Sepes.RestApi.IntegrationTests.Setup;
using Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Sepes.RestApi.IntegrationTests.Tests
{
    [Collection("Integration tests collection")]
    public class StudyControllerUpdateTests : ControllerTestBase
    {
        public StudyControllerUpdateTests(TestHostFixture testHostFixture)
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
        public async Task UpdateStudyMetadata_WithRequiredRole_ShouldSucceed(bool isEmployee, bool isAdmin, bool isSponsor)
        {
            SetScenario(isEmployee: isEmployee, isAdmin: isAdmin, isSponsor: isSponsor);

            var studyCreateConversation = await StudyCreator.CreateAndExpectSuccess(_restHelper);

            CreateStudyAsserts.ExpectSuccess(studyCreateConversation.Request, studyCreateConversation.Response);
        }
       
        [Theory]
        [InlineData(false, false, false, false, false, null)]
        [InlineData(false, false, false, false, true, null)]
        [InlineData(false, false, false, true, false, null)]
        [InlineData(false, false, true, false, false, null)]
        [InlineData(false, true, false, false, false, null)]
        [InlineData(false, true, false, false, true, null)]
        [InlineData(false, true, false, true, false, null)]
        [InlineData(false, true, true, false, false, null)]
        [InlineData(false, false, true, false, false, StudyRoles.VendorAdmin)]
        [InlineData(false, false, true, false, false, StudyRoles.VendorContributor)]
        [InlineData(false, false, true, false, false, StudyRoles.StudyViewer)]
        [InlineData(false, false, true, false, true, StudyRoles.VendorAdmin)]
        [InlineData(false, false, true, false, true, StudyRoles.VendorContributor)]
        [InlineData(false, false, true, false, true, StudyRoles.StudyViewer)]
        [InlineData(true, false, false, false, false, null)]
        [InlineData(true, false, false, false, true, null)]
        [InlineData(true, false, true, false, false, null)]
        [InlineData(true, true, false, false, false, null)]
        [InlineData(true, true, false, false, true, null)]
        [InlineData(true, true, true, false, false, null)]
        [InlineData(true, false, true, false, false, StudyRoles.VendorAdmin)]
        [InlineData(true, false, true, false, false, StudyRoles.VendorContributor)]
        [InlineData(true, false, true, false, false, StudyRoles.StudyViewer)]
        [InlineData(true, false, true, false, true, StudyRoles.VendorAdmin)]
        [InlineData(true, false, true, false, true, StudyRoles.StudyViewer)]
        public async Task UpdateStudyMetadata_WithoutRequiredRole_ShouldFail(bool createdByCurrentUser, bool restricted, bool isEmployee, bool sponsor, bool isDatasetAdmin, string studyRole = null)
        {
            SetScenario(isEmployee: isEmployee, isSponsor: sponsor, isDatasetAdmin: isDatasetAdmin);
            await WithUserSeeds();
            var createdStudy = createdByCurrentUser ?  await WithStudyCreatedByCurrentUser(restricted, new List<string> { studyRole }) : await WithStudyCreatedByOtherUser(restricted, new List<string> { studyRole });
            var updateRequst = new StudyDto() { Name = "newName", Vendor = "newVendor" };
            var studyDeleteConversation = await StudyUpdater.UpdateAndExpectFailure(_restHelper, createdStudy.Id, updateRequst);

            ApiResponseBasicAsserts.ExpectForbiddenWithMessage(studyDeleteConversation.Response, "does not have permission to perform operation");
        }       
    }
}
