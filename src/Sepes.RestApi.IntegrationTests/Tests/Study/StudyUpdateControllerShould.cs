using Sepes.Common.Constants;
using Sepes.Common.Dto.Study;
using Sepes.RestApi.IntegrationTests.TestHelpers.Requests;
using Sepes.RestApi.IntegrationTests.Setup;
using Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Sepes.RestApi.IntegrationTests.Tests
{
    [Collection("Integration tests collection")]
    public class StudyUpdateControllerShould : ControllerTestBase
    {
        public StudyUpdateControllerShould(TestHostFixture testHostFixture)
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
        public async Task UpdateStudyMetadata_IfRequiredRole(bool isEmployee, bool isAdmin, bool isSponsor)
        {
            SetScenario(isEmployee: isEmployee, isAdmin: isAdmin, isSponsor: isSponsor);

            var studyCreateConversation = await StudyCreator.CreateAndExpectSuccess(_restHelper);

            CreateStudyAsserts.ExpectSuccess(studyCreateConversation.Request, studyCreateConversation.Response);
        }     
          

        [Theory]
        [InlineData(false, StudyRoles.StudyViewer, StudyRoles.VendorAdmin, StudyRoles.VendorContributor)]
        [InlineData(true, StudyRoles.StudyViewer, StudyRoles.VendorAdmin, StudyRoles.VendorContributor)]
        public async Task Throw_If_No_Relevant_Permission(bool restricted, params string[] studyRoles)
        {
            SetScenario();

            await WithUserSeeds();

            foreach (var curStudyRole in studyRoles)
            {
                await PerformTestExpectForbidden(false, restricted, curStudyRole);
                await PerformTestExpectForbidden(true, restricted, curStudyRole);
            }
        }

        [Fact]
        public async Task Throw_If_No_Permission()
        {
            SetScenario();

            await WithUserSeeds();

            await PerformTestExpectForbidden(false, false);
            await PerformTestExpectForbidden(false, true);
            await PerformTestExpectForbidden(true, false);
            await PerformTestExpectForbidden(true, true);
        }

        [Fact]
        public async Task Throw_If_Only_Employee()
        {
            SetScenario(isEmployee: true);

            await WithUserSeeds();

            await PerformTestExpectForbidden(false, false);
            await PerformTestExpectForbidden(false, true);
            await PerformTestExpectForbidden(true, false);
            await PerformTestExpectForbidden(true, true);
        }       

        [Fact]
        public async Task Throw_If_Sponsor_AndNotCreatedByCurrent()
        {
            SetScenario(isSponsor: true);

            await WithUserSeeds();

            await PerformTestExpectForbidden(false, false);
            await PerformTestExpectForbidden(false, true);          
        }

        [Theory]
        [InlineData(null, StudyRoles.StudyViewer, StudyRoles.VendorAdmin, StudyRoles.VendorContributor)]     
        public async Task Throw_If_DatasetAdmin_And_NoOtherRelevant_Permission(params string[] studyRoles)
        {
            SetScenario(isDatasetAdmin: true);

            await WithUserSeeds();

            foreach (var curStudyRole in studyRoles)
            {
                await PerformTestExpectForbidden(false, false, curStudyRole);
                await PerformTestExpectForbidden(true, false, curStudyRole);
            }
        }

        async Task PerformTestExpectForbidden(bool createdByCurrentUser, bool restricted, string studyRole = null)
        {
            var study = createdByCurrentUser ? await WithStudyCreatedByCurrentUser(restricted, new List<string> { studyRole }) : await WithStudyCreatedByOtherUser(restricted, new List<string> { studyRole });
            await PerformTestExpectForbidden(study.Id);
        }

        async Task PerformTestExpectForbidden(int studyId)
        {
            var updateRequest = new StudyDto() { Name = "newName", Vendor = "newVendor" };
            var studyDeleteConversation = await StudyUpdater.UpdateAndExpectFailure(_restHelper, studyId, updateRequest);
            ApiResponseBasicAsserts.ExpectForbiddenWithMessage(studyDeleteConversation.Response, "does not have permission to perform operation");
        }
    }
}
