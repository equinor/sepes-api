using Sepes.Infrastructure.Constants;
using Sepes.RestApi.IntegrationTests.RequestHelpers;
using Sepes.RestApi.IntegrationTests.Setup;
using Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Sepes.RestApi.IntegrationTests.Tests
{
    [Collection("Integration tests collection")]
    public class StudyControllerDeleteTests : ControllerTestBase
    {
        public StudyControllerDeleteTests(TestHostFixture testHostFixture)
            : base(testHostFixture)
        {
           
        }      

        [Theory]

        //No permissions at all
        [InlineData(false, false, false, false, false)]
        [InlineData(false, true, false, false, false)]

        //Employee
        [InlineData(false, false, true, false, false)]
        [InlineData(false, true, true, false, false)]

        //Sponsor can not delete studies
        [InlineData(false, false, false, true, false)]
        [InlineData(false, true, false, true, false)]
        [InlineData(false, false, true, true, false)]
        [InlineData(false, true, true, true, false)]
        [InlineData(true, false, false, true, false)]
        [InlineData(true, true, false, true, false)]
        [InlineData(true, false, true, true, false)]
        [InlineData(true, true, true, true, false)]    

        //Dataset admin cannot delete any studies
        [InlineData(false, false, false, false, true)]
        [InlineData(false, false, true, false, true)]
        [InlineData(false, true, false, false, true)]
        [InlineData(true, false, false, false, true)]

        //These roles cannot delete study 
        [InlineData(false, false, false, false, false, StudyRoles.SponsorRep)]
        [InlineData(false, false, false, false, false, StudyRoles.StudyViewer)]
        [InlineData(false, false, false, false, false, StudyRoles.VendorAdmin)]
        [InlineData(false, false, false, false, false, StudyRoles.VendorContributor)]

        [InlineData(false, true, false, false, false, StudyRoles.SponsorRep)]
        [InlineData(false, true, false, false, false, StudyRoles.StudyViewer)]
        [InlineData(false, true, false, false, false, StudyRoles.VendorAdmin)]
        [InlineData(false, true, false, false, false, StudyRoles.VendorContributor)]

        [InlineData(false, false, false, false, true, StudyRoles.SponsorRep)]
        [InlineData(false, false, false, false, true, StudyRoles.StudyViewer)]
        [InlineData(false, false, false, false, true, StudyRoles.VendorAdmin)]
        [InlineData(false, false, false, false, true, StudyRoles.VendorContributor)]


        public async Task DeleteStudy_WithoutRequiredStudyRole_ShouldFail(bool createdByCurrentUser, bool restricted, bool isEmployee, bool isSponsor, bool isDatasetAdmin, string studyRole = null)
        { 
            SetScenario(isEmployee: isEmployee, isSponsor: isSponsor, isDatasetAdmin: isDatasetAdmin);

            await WithUserSeeds();

            await PerformTestExpectForbidden(createdByCurrentUser, restricted, studyRole);            
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
