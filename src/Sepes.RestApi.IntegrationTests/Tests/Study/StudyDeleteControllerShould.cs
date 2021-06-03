using Sepes.Common.Constants;
using Sepes.RestApi.IntegrationTests.RequestHelpers;
using Sepes.RestApi.IntegrationTests.Setup;
using Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Sepes.RestApi.IntegrationTests.Tests
{
    [Collection("Integration tests collection")]
    public class StudyDeleteControllerShould : ControllerTestBase
    {
        public StudyDeleteControllerShould(TestHostFixture testHostFixture)
            : base(testHostFixture)
        {
           
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
        public async Task Throw_If_Only_Sponsor()
        {
            SetScenario(isSponsor: true);

            await WithUserSeeds();

            await PerformTestExpectForbidden(false, false);
            await PerformTestExpectForbidden(false, true);
            await PerformTestExpectForbidden(true, false);
            await PerformTestExpectForbidden(true, true);
        }

        [Theory]
        [InlineData(false, StudyRoles.SponsorRep, StudyRoles.StudyViewer, StudyRoles.VendorAdmin, StudyRoles.VendorContributor)]
        [InlineData(true, StudyRoles.SponsorRep, StudyRoles.StudyViewer, StudyRoles.VendorAdmin, StudyRoles.VendorContributor)]
        public async Task Throw_If_No_Relevant_Permission(bool restricted, params string[] studyRoles)
        {
            SetScenario();

            await WithUserSeeds();

            foreach (var curStudyRole in studyRoles)
            {
                await PerformTestExpectForbidden(false, restricted, curStudyRole);
            }
        }

        [Theory]   
        [InlineData(false, StudyRoles.SponsorRep, StudyRoles.StudyViewer, StudyRoles.VendorAdmin, StudyRoles.VendorContributor)]
        [InlineData(true, null, StudyRoles.SponsorRep, StudyRoles.StudyViewer, StudyRoles.VendorAdmin, StudyRoles.VendorContributor)]
        public async Task Throw_If_DatasetAdmin_And_NoOtherRelevant_Permission(bool restricted, params string[] studyRoles)
        {
            SetScenario(isDatasetAdmin: true);

            await WithUserSeeds();

            foreach(var curStudyRole in studyRoles)
            {
                await PerformTestExpectForbidden(false, restricted, curStudyRole);
            }          
        }

        [Theory]
        //ADMIN
        [InlineData(false, false, true, false)]
        [InlineData(false, true, true, false)]
        [InlineData(true, false, true, false)]
        [InlineData(true, true, true, false)]

        public async Task DeleteStudy_AsAdmin_ShouldSucceed(bool createdByCurrentUser, bool restricted, bool isAdmin, bool isSponsor, string studyRole = null)
        {
            SetScenario(isAdmin: isAdmin, isSponsor: isSponsor);
            await WithUserSeeds();
            await PerformTestExpectSuccess(createdByCurrentUser, restricted, studyRole);        
        }

        async Task PerformTestExpectSuccess(bool createdByCurrentUser, bool restricted, string studyRole = null)
        {
            var study = createdByCurrentUser ? await WithStudyCreatedByCurrentUser(restricted, new List<string> { studyRole }) : await WithStudyCreatedByOtherUser(restricted, new List<string> { studyRole });
            await PerformTestExpectSuccess(study.Id);
        }

        async Task PerformTestExpectSuccess(int studyId)
        {
            var studyDeleteConversation = await GenericDeleter.DeleteAndExpectSuccess(_restHelper, GenericDeleter.StudyUrl(studyId));
            ApiResponseBasicAsserts.ExpectNoContent(studyDeleteConversation.Response);
        }

        async Task PerformTestExpectForbidden(bool createdByCurrentUser, bool restricted, string studyRole = null)
        {
            var study = createdByCurrentUser ? await WithStudyCreatedByCurrentUser(restricted, new List<string> { studyRole }) : await WithStudyCreatedByOtherUser(restricted, new List<string> { studyRole });
            await PerformTestExpectForbidden(study.Id);
        }       

        async Task PerformTestExpectForbidden(int studyId)
        {
            var studyReadConversation = await GenericDeleter.DeleteAndExpectFailure(_restHelper, GenericDeleter.StudyUrl(studyId));
            ApiResponseBasicAsserts.ExpectForbiddenWithMessage(studyReadConversation.Response);
        }
    }
}
