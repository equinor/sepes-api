using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Study;
using Sepes.RestApi.IntegrationTests.RequestHelpers;
using Sepes.RestApi.IntegrationTests.Setup;
using Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets;
using System.Collections.Generic;
using System.Linq;
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
        [InlineData(false)]
        [InlineData(true)]    

        public async Task Read_StudyList_WithoutRelevantRoles_ShouldFail(bool datasetAdmin)
        {       
            SetScenario(isDatasetAdmin: datasetAdmin);
            await WithUserSeeds();
            _ = await WithStudyCreatedByCurrentUser(false);
            _ = await WithStudyCreatedByCurrentUser(true);
            _ = await WithStudyCreatedByOtherUser(false);
            _ = await WithStudyCreatedByOtherUser(true);

            var studyReadConversation = await GenericReader.ReadExpectSuccess<List<StudyListItemDto>>(_restHelper, GenericReader.StudiesUrl());
            ApiResponseBasicAsserts.ExpectSuccess<List<StudyListItemDto>>(studyReadConversation.Response);
            Assert.Empty(studyReadConversation.Response.Content);
        }

        [Theory] 
        [InlineData(true, StudyRoles.SponsorRep)]
        [InlineData(true, StudyRoles.VendorAdmin)]
        [InlineData(true, StudyRoles.VendorContributor)]
        [InlineData(true, StudyRoles.StudyViewer)]      

        public async Task Read_StudyList_ShouldOnlyContainRelevantRestrictedStudies(bool employee, string myRole)
        {
            SetScenario(isEmployee: employee);
            await WithUserSeeds();

            var studyThisUserShouldSee = await WithStudyCreatedByOtherUser(true, myRole);
            var studyThisUserShouldNotSee = await WithStudyCreatedByOtherUser(true);

            var studyReadConversation = await GenericReader.ReadExpectSuccess<List<StudyListItemDto>>(_restHelper, GenericReader.StudiesUrl());
            ApiResponseBasicAsserts.ExpectSuccess<List<StudyListItemDto>>(studyReadConversation.Response);
            Assert.NotEmpty(studyReadConversation.Response.Content);
            Assert.NotNull(studyReadConversation.Response.Content.FirstOrDefault(s => s.Id == studyThisUserShouldSee.Id));
            Assert.Null(studyReadConversation.Response.Content.FirstOrDefault(s => s.Id == studyThisUserShouldNotSee.Id));
        }

        [Theory]             
        [InlineData(true, true, false)]
        [InlineData(true, false, true)] 
        [InlineData(true, false, false)] 
        [InlineData(false, false, false)]
        [InlineData(false, false, true)] 
        public async Task Read_Study_WithoutRelevantRoles_ShouldFail(bool employee, bool isSponsor, bool datasetAdmin)
        {
            SetScenario(isEmployee: employee, isSponsor: isSponsor, isDatasetAdmin: datasetAdmin);
            await WithUserSeeds();

            var createdStudy = await WithStudyCreatedByOtherUser(restricted: true);         

            var studyReadConversation = await GenericReader.ReadExpectFailure(_restHelper, GenericReader.StudyUrl(createdStudy.Id));
            ApiResponseBasicAsserts.ExpectForbiddenWithMessage(studyReadConversation.Response, "does not have permission to perform operation");
        }

        [Theory]
        [InlineData(false, false, null)]
        [InlineData(true, true, null)]
        [InlineData(true, true, StudyRoles.VendorAdmin)]
        [InlineData(true, true, StudyRoles.VendorContributor)]

        public async Task Read_Study_ResultsAndLearnings_WithoutRelevantRoles_ShouldFail(bool restricted, bool employee, string studyRole = null)
        {
            SetScenario(isEmployee: employee);
            await WithUserSeeds();

            var createdStudy = await WithStudyCreatedByOtherUser(restricted, studyRole);

            var studyReadConversation = await GenericReader.ReadExpectFailure(_restHelper, GenericReader.StudyResultsAndLearningsUrl(createdStudy.Id));

            ApiResponseBasicAsserts.ExpectForbiddenWithMessage(studyReadConversation.Response, "does not have permission to perform operation");
        }
    }
}
