using Sepes.Common.Constants;
using Sepes.RestApi.IntegrationTests.TestHelpers.Requests;
using Sepes.RestApi.IntegrationTests.Setup;
using Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using System.Linq;

namespace Sepes.RestApi.IntegrationTests.Tests
{
    [Collection("Integration tests collection")]
    public class DatasetFileControllerShould : ControllerTestBase
    {
        public DatasetFileControllerShould(TestHostFixture testHostFixture)
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

        //Sponsor can not close studies he did not create
        [InlineData(false, false, false, true, false)]
        [InlineData(false, true, false, true, false)]
        [InlineData(false, false, true, true, false)]
        [InlineData(false, true, true, true, false)]

        //Dataset admin cannot close any studies
        [InlineData(false, false, false, false, true)]
        [InlineData(false, false, true, false, true)]
        [InlineData(false, true, false, false, true)]
        [InlineData(true, false, false, false, true)]

        //These roles cannot close study
        [InlineData(false, false, false, false, false, StudyRoles.StudyViewer)]
        [InlineData(false, false, false, false, false, StudyRoles.VendorAdmin)]
        [InlineData(false, false, false, false, false, StudyRoles.VendorContributor)]

        [InlineData(false, true, false, false, false, StudyRoles.StudyViewer)]
        [InlineData(false, true, false, false, false, StudyRoles.VendorAdmin)]
        [InlineData(false, true, false, false, false, StudyRoles.VendorContributor)]

        [InlineData(false, false, false, false, true, StudyRoles.StudyViewer)]
        [InlineData(false, false, false, false, true, StudyRoles.VendorAdmin)]
        [InlineData(false, false, false, false, true, StudyRoles.VendorContributor)]


        public async Task ThrowOn_GetFileUploadSasToken_IfPermissionMissing(bool createdByCurrentUser, bool restricted, bool isEmployee, bool isSponsor, bool isDatasetAdmin, string studyRole  = null)
        { 
            SetScenario(isEmployee: isEmployee, isSponsor: isSponsor, isDatasetAdmin: isDatasetAdmin);

            await WithUserSeeds();

            await PerformTestExpectForbidden(createdByCurrentUser, restricted, studyRole);            
        }     

        async Task PerformTestExpectForbidden(bool createdByCurrentUser, bool restricted, string studyRole = null)
        {
            var study = createdByCurrentUser? await WithStudyCreatedByCurrentUser(restricted, new List<string> { studyRole }, addDatasets: true) : await WithStudyCreatedByOtherUser(restricted, new List<string> { studyRole }, addDatasets: true);
            var datasetId = study.StudyDatasets.FirstOrDefault().DatasetId;

            await PerformTestExpectForbidden(GenericReader.DatasetFileUploadUrl(datasetId));
            await PerformTestExpectForbidden(GenericReader.DatasetFileDeleteUrl(datasetId));
        }

        async Task PerformTestExpectForbidden(string url)
        {
            var studyCloseConversation = await GenericReader.ReadExpectFailure(_restHelper, url);           
            ApiResponseBasicAsserts.ExpectForbiddenWithMessage(studyCloseConversation.Response);
        }

        [Theory]
        //ADMIN
        [InlineData(false, false, true, false)]
        [InlineData(false, true, true, false)]
        [InlineData(true, false, true, false)]
        [InlineData(true, true, true, false)]

        //SPONSOR      
        [InlineData(true, false, false, true)]
        [InlineData(true, true, false, true)]

        //STUDY SPECIFIC ROLES
        [InlineData(false, false, false, false, StudyRoles.SponsorRep)]
        [InlineData(false, true, false, false, StudyRoles.SponsorRep)]
        [InlineData(true, false, false, false, StudyRoles.SponsorRep)]
        [InlineData(true, true, false, false, StudyRoles.SponsorRep)]


        public async Task Allow_GetFileUploadSasToken_IfPermissionMissing(bool createdByCurrentUser, bool restricted, bool isAdmin, bool isSponsor, string studyRole = null)
        {
            SetScenario(isAdmin: isAdmin, isSponsor: isSponsor);
            await WithUserSeeds();
            await PerformTestExpectSuccess(createdByCurrentUser, restricted, studyRole);        
        }

        async Task PerformTestExpectSuccess(bool createdByCurrentUser, bool restricted, string studyRole = null)
        {
            var study = createdByCurrentUser ? await WithStudyCreatedByCurrentUser(restricted, new List<string> { studyRole }, addDatasets: true) : await WithStudyCreatedByOtherUser(restricted, new List<string> { studyRole }, addDatasets: true);
            var datasetId = study.StudyDatasets.FirstOrDefault().DatasetId;
            await PerformTestExpectSuccess(GenericReader.DatasetFileUploadUrl(datasetId));
            await PerformTestExpectSuccess(GenericReader.DatasetFileDeleteUrl(datasetId));
        }

        async Task PerformTestExpectSuccess(string url)
        {
            var studyDeleteConversation = await GenericPutter.PutAndExpectSuccess(_restHelper, url);
            ApiResponseBasicAsserts.ExpectNoContent(studyDeleteConversation.Response);
        }        
    }
}
