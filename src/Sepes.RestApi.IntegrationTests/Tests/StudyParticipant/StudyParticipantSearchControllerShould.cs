using Sepes.Common.Constants;
using Sepes.RestApi.IntegrationTests.TestHelpers.Requests;
using Sepes.RestApi.IntegrationTests.Setup;
using Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets;
using Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets.StudyParticipant;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Sepes.RestApi.IntegrationTests.Tests
{
    [Collection("Integration tests collection")]
    public class StudyParticipantSearchControllerShould : ControllerTestBase
    {
        public StudyParticipantSearchControllerShould(TestHostFixture testHostFixture)
            : base(testHostFixture)
        {

        }


        [Theory]
        [InlineData(false, false, true, false, StudyRoles.StudyViewer, StudyRoles.SponsorRep, StudyRoles.VendorAdmin, StudyRoles.VendorContributor)]
        [InlineData(false, true, true, false, StudyRoles.StudyViewer, StudyRoles.SponsorRep, StudyRoles.VendorAdmin, StudyRoles.VendorContributor)]    
        [InlineData(true, false, true, false, StudyRoles.StudyViewer, StudyRoles.SponsorRep, StudyRoles.VendorAdmin, StudyRoles.VendorContributor)]
        [InlineData(true, true, true, false, StudyRoles.StudyViewer, StudyRoles.SponsorRep, StudyRoles.VendorAdmin, StudyRoles.VendorContributor)]
        [InlineData(true, false, false, true, StudyRoles.StudyViewer, StudyRoles.SponsorRep, StudyRoles.VendorAdmin, StudyRoles.VendorContributor)]
        [InlineData(true, true, false, true, StudyRoles.StudyViewer, StudyRoles.SponsorRep, StudyRoles.VendorAdmin, StudyRoles.VendorContributor)]
        public async void AddingStudyParticipant_HavingRelevantAppRole_AndAsStudyOwner_ShouldSucceeed(bool studyCreatedByCurrentUser, bool restrictedStudy, bool isAdmin, bool isSponsor, params string[] rolesToAdd)
        {
            await WithBasicSeeds();
            var study = studyCreatedByCurrentUser ? await WithStudyCreatedByCurrentUser(restrictedStudy) : await WithStudyCreatedByOtherUser(restrictedStudy);

            SetScenario(isAdmin: isAdmin, isSponsor: isSponsor);

            await PerformTestsExpectSuccess(study.Id, rolesToAdd);
        }

        [Theory]
        [InlineData(false, false, false, StudyRoles.StudyViewer, StudyRoles.SponsorRep, StudyRoles.VendorAdmin, StudyRoles.VendorContributor)]
        [InlineData(false, false, true, StudyRoles.StudyViewer, StudyRoles.SponsorRep, StudyRoles.VendorAdmin, StudyRoles.VendorContributor)]
        [InlineData(false, true, false, StudyRoles.StudyViewer, StudyRoles.SponsorRep, StudyRoles.VendorAdmin, StudyRoles.VendorContributor)]
        [InlineData(false, true, true, StudyRoles.StudyViewer, StudyRoles.SponsorRep, StudyRoles.VendorAdmin, StudyRoles.VendorContributor)]
        [InlineData(true, false, false, StudyRoles.StudyViewer, StudyRoles.SponsorRep, StudyRoles.VendorAdmin, StudyRoles.VendorContributor)]
        [InlineData(true, false, true, StudyRoles.StudyViewer, StudyRoles.SponsorRep, StudyRoles.VendorAdmin, StudyRoles.VendorContributor)]
        [InlineData(true, true, false, StudyRoles.StudyViewer, StudyRoles.SponsorRep, StudyRoles.VendorAdmin, StudyRoles.VendorContributor)]
        [InlineData(true, true, true, StudyRoles.StudyViewer, StudyRoles.SponsorRep, StudyRoles.VendorAdmin, StudyRoles.VendorContributor)]
        public async void AddingStudyParticipantWithoutRelevantRoles_ShouldThrow(bool studyCreatedByCurrentUser, bool restrictedStudy, bool employee, params string[] rolesToAdd)
        {
            await WithBasicSeeds();
            var study = studyCreatedByCurrentUser ? await WithStudyCreatedByCurrentUser(restrictedStudy) : await WithStudyCreatedByOtherUser(restrictedStudy);

            SetScenario(isEmployee: employee);

            await PerformTestsExpectFailure(study.Id, rolesToAdd);
        }


        [Theory]
        [InlineData(false, false, StudyRoles.VendorAdmin, StudyRoles.VendorContributor)]
        [InlineData(false, true, StudyRoles.VendorAdmin, StudyRoles.VendorContributor)]
        [InlineData(true, false, StudyRoles.VendorAdmin, StudyRoles.VendorContributor)]
        [InlineData(true, true, StudyRoles.VendorAdmin, StudyRoles.VendorContributor)]
        public async void VendorAdmin_AddingVendorRoles_ShouldSucceed(bool employee, bool restrictedStudy, params string[] rolesToAdd)
        {
            await WithBasicSeeds(); 
            var study = await WithStudyCreatedByOtherUser(restrictedStudy, new List<string> { StudyRoles.VendorAdmin } );
            SetScenario(isEmployee: employee);
            await PerformTestsExpectSuccess(study.Id, rolesToAdd);
        }

        [Theory]
        [InlineData(false, false, StudyRoles.StudyViewer, StudyRoles.SponsorRep)]
        [InlineData(false, true, StudyRoles.StudyViewer, StudyRoles.SponsorRep)]
        [InlineData(true, false, StudyRoles.StudyViewer, StudyRoles.SponsorRep)]
        [InlineData(true, true, StudyRoles.StudyViewer, StudyRoles.SponsorRep)]
        public async void VendorAdmin_AddingNonVendorRoles_ShouldThrow(bool employee, bool restrictedStudy, params string[] rolesToAdd)
        {
            await WithBasicSeeds();
            var study = await WithStudyCreatedByOtherUser(restrictedStudy, new List<string> { StudyRoles.VendorAdmin } );
            SetScenario(isEmployee: employee);
            await PerformTestsExpectFailure(study.Id, rolesToAdd);
        }

     

        async Task PerformTestsExpectSuccess(int studyId, params string[] rolesToAdd)
        {
            var responseDto = StudyParticipantAdderAndRemover.CreateParticipantLookupDto();

            foreach (var curRole in rolesToAdd)
            {
                var studyParticipantAddConversation = await StudyParticipantAdderAndRemover.AddAndExpectSuccess(_restHelper, studyId, curRole, responseDto);
                AddStudyParticipantsAsserts.ExpectSuccess(curRole, studyParticipantAddConversation.Request, studyParticipantAddConversation.Response);
            }            
        }

        async Task PerformTestsExpectFailure(int studyId, params string[] rolesToAdd)
        {
            var responseDto = StudyParticipantAdderAndRemover.CreateParticipantLookupDto();

            foreach (var curRole in rolesToAdd)
            {
                var studyParticipantAddConversation = await StudyParticipantAdderAndRemover.AddAndExpectFailure(_restHelper, studyId, curRole, responseDto);
                ApiResponseBasicAsserts.ExpectForbiddenWithMessage(studyParticipantAddConversation.Response);
            }            
        }
    }
}
