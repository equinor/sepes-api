using Sepes.Common.Constants;
using Sepes.RestApi.IntegrationTests.Setup;
using Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets;
using Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets.Dataset;
using Sepes.RestApi.IntegrationTests.TestHelpers.Requests;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Sepes.RestApi.IntegrationTests.Tests
{
    [Collection("Integration tests collection")]
    public class StudySpecificDatasetControllerShould : ControllerTestBase
    {
        public StudySpecificDatasetControllerShould(TestHostFixture testHostFixture)
            : base(testHostFixture)
        {

        }

        [Fact]
        public async Task CreateAndUpdateAndDelete_If_UserIsAdmin()
        {
            SetScenario(isAdmin: true);

            await WithUserSeeds();

            await PerformTestExpectSuccess(false, false);
            await PerformTestExpectSuccess(true, false);
        }

        [Fact]
        public async Task CreateAndUpdateAndDelete_If_UserIsSponsorAndOwner()
        {
            SetScenario(isSponsor: true);

            await WithUserSeeds();

            await PerformTestExpectSuccess(true, false);
            await PerformTestExpectSuccess(true, true);
        }

        [Theory]
        [InlineData(false, StudyRoles.SponsorRep)]
        [InlineData(true, StudyRoles.SponsorRep)]
        public async Task CreateAndUpdateAndDelete_If_UserHasRelevantPermissions(bool restricted, params string[] studyRoles)
        {
            SetScenario();

            await WithUserSeeds();

            foreach (var curStudyRole in studyRoles)
            {
                await PerformTestExpectSuccess(false, restricted, curStudyRole);
                await PerformTestExpectSuccess(true, restricted, curStudyRole);
            }
        }   


        [Theory]
        [InlineData(false, StudyRoles.StudyViewer, StudyRoles.VendorAdmin, StudyRoles.VendorContributor)]
        [InlineData(true, StudyRoles.StudyViewer, StudyRoles.VendorAdmin, StudyRoles.VendorContributor)]
        public async Task Prevent_CreateAndUpdateAndDelete_If_No_Relevant_Permission(bool restricted, params string[] studyRoles)
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
        public async Task Prevent_CreateAndUpdateAndDelete_If_No_Permission()
        {
            SetScenario();

            await WithUserSeeds();

            await PerformTestExpectForbidden(false, false);
            await PerformTestExpectForbidden(false, true);
            await PerformTestExpectForbidden(true, false);
            await PerformTestExpectForbidden(true, true);
        }

        [Fact]
        public async Task Prevent_CreateAndUpdateAndDelete_If_Only_Employee()
        {
            SetScenario(isEmployee: true);

            await WithUserSeeds();

            await PerformTestExpectForbidden(false, false);
            await PerformTestExpectForbidden(false, true);
            await PerformTestExpectForbidden(true, false);
            await PerformTestExpectForbidden(true, true);
        }

        [Fact]
        public async Task Prevent_CreateAndUpdateAndDelete_If_Sponsor_AndNotCreatedByCurrent()
        {
            SetScenario(isSponsor: true);

            await WithUserSeeds();

            await PerformTestExpectForbidden(false, false);
            await PerformTestExpectForbidden(false, true);
        }

        [Theory]
        [InlineData(null, StudyRoles.StudyViewer, StudyRoles.VendorAdmin, StudyRoles.VendorContributor)]
        public async Task Prevent_CreateAndUpdateAndDelete_If_DatasetAdmin_And_NoOtherRelevant_Permission(params string[] studyRoles)
        {
            SetScenario(isDatasetAdmin: true);

            await WithUserSeeds();

            foreach (var curStudyRole in studyRoles)
            {
                await PerformTestExpectForbidden(false, false, curStudyRole);
                await PerformTestExpectForbidden(true, false, curStudyRole);
            }
        }

        async Task PerformTestExpectSuccess(bool createdByCurrentUser, bool restricted, string studyRole = null)
        {
            var study = createdByCurrentUser ? await WithStudyCreatedByCurrentUser(restricted, new List<string> { studyRole }, addDatasets: true) : await WithStudyCreatedByOtherUser(restricted, new List<string> { studyRole }, addDatasets: true);
            await PerformTestExpectSuccess(study.Id);
        }

        async Task PerformTestExpectSuccess(int studyId)
        {    
            //Create
            var createRequest = await StudySpecificDatasetCreateUpdateDelete.CreateExpectSuccess(_restHelper, studyId);
            CreateDatasetAsserts.ExpectSuccess(createRequest.Request, createRequest.Response);
                   
            //Update
            var updateRequest = await StudySpecificDatasetCreateUpdateDelete.UpdateExpectSuccess(_restHelper, studyId, createRequest.Response.Content.Id);
            CreateDatasetAsserts.ExpectSuccess(updateRequest.Request, updateRequest.Response);

            //Delete
            var deleteRequest = await StudySpecificDatasetCreateUpdateDelete.DeleteExpectSuccess(_restHelper, createRequest.Response.Content.Id);
            CreateDatasetAsserts.ExpectDeleteSuccess(deleteRequest.Response);
        }

        async Task PerformTestExpectForbidden(bool createdByCurrentUser, bool restricted, string studyRole = null)
        {
            var study = createdByCurrentUser ? await WithStudyCreatedByCurrentUser(restricted, new List<string> { studyRole }, addDatasets: true) : await WithStudyCreatedByOtherUser(restricted, new List<string> { studyRole }, addDatasets: true);
            var dataset = study.StudyDatasets.FirstOrDefault();
            await PerformTestExpectForbidden(study.Id, dataset.DatasetId);
        }

        async Task PerformTestExpectForbidden(int studyId, int datasetId)
        {
            //Create
            var createRequest = await StudySpecificDatasetCreateUpdateDelete.CreateExpectFailure(_restHelper, studyId);
            ApiResponseBasicAsserts.ExpectForbiddenWithMessage(createRequest.Response, "does not have permission to perform operation");

            //Update         
            var updateRequest = await StudySpecificDatasetCreateUpdateDelete.UpdateExpectFailure(_restHelper, studyId, datasetId);
            ApiResponseBasicAsserts.ExpectForbiddenWithMessage(updateRequest.Response, "does not have permission to perform operation");

            //Delete
            var deleteRequest = await StudySpecificDatasetCreateUpdateDelete.DeleteExpectFailure(_restHelper, datasetId);
            ApiResponseBasicAsserts.ExpectForbiddenWithMessage(deleteRequest.Response, "does not have permission to perform operation");
        }
    }
}
