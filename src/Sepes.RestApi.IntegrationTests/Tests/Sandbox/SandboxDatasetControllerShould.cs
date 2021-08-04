using Sepes.Common.Constants;
using Sepes.Infrastructure.Model;
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
    public class SandboxDatasetControllerShould : ControllerTestBase
    {
        public SandboxDatasetControllerShould(TestHostFixture testHostFixture)
            : base(testHostFixture)
        {
           
        }    

        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public async Task AddAndRemove_AsAdmin(bool studyCreatedByCurrentUser, bool restrictedStudy)
        {
            await WithBasicSeeds();
            SetScenario(isAdmin: true);

            var sandbox = await WithSandbox(studyCreatedByCurrentUser, restrictedStudy, addDatasetsToStudy: true);

            await PerformTestsExpectSuccess(sandbox);         
        }

        [Theory]       
        [InlineData(false)]
        [InlineData(true)]
        public async Task AddAndRemove_AsSponsor(bool restrictedStudy)
        {
            await WithBasicSeeds();

            SetScenario(isSponsor: true);

            var sandbox = await WithSandbox(true, restrictedStudy, addDatasetsToStudy: true);

            await PerformTestsExpectSuccess(sandbox);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task FailToAddOrRemove_ToNonOwnedStudy_AsSponsor(bool restrictedStudy)
        {
            await WithBasicSeeds();

            SetScenario(isSponsor: true);

            var sandbox = await WithSandbox(false, restrictedStudy, addDatasetsToStudy: true);

            await PerformAddExpectFailure(sandbox);
            await PerformRemoveExpectFailure(sandbox);
        }

        [Theory]
        [InlineData(false, StudyRoles.SponsorRep)]
        [InlineData(false, StudyRoles.VendorAdmin)]
        [InlineData(true, StudyRoles.SponsorRep)]
        [InlineData(true, StudyRoles.VendorAdmin)]      
        public async Task AddAndRemove_HavingCorrectStudyRoles_ShouldSucceed(bool restrictedStudy, string studyRole)
        {
            await WithBasicSeeds();

            SetScenario();

            var sandbox = await WithSandbox(false, restrictedStudy, new List<string> { studyRole }, addDatasetsToStudy: true);

            await PerformTestsExpectSuccess(sandbox);
        }

        [Theory]
        [InlineData(false, StudyRoles.VendorContributor)]
        [InlineData(false, StudyRoles.StudyViewer)] 
        [InlineData(true, StudyRoles.VendorContributor)]
        [InlineData(true, StudyRoles.StudyViewer)]
        public async Task AddAndRemove_HavingWrongStudyRoles_ShouldFail(bool restrictedStudy, string studyRole)
        {
            await WithBasicSeeds();

            SetScenario();

            var sandbox = await WithSandbox(false, restrictedStudy, new List<string> { studyRole }, addDatasetsToStudy: true);

            await PerformAddExpectFailure(sandbox);
            await PerformRemoveExpectFailure(sandbox);
        }

        async Task PerformTestsExpectSuccess(Sandbox sandbox)
        {           
            var dataset = sandbox.Study.StudyDatasets.FirstOrDefault();

            var datasetAddConversation = await SandboxDatasetOperations.AddDatasetExpectSuccess(_restHelper, sandbox.Id, dataset.DatasetId);
            SandboxDatasetAsserts.NewlyAddedExpectSuccess(datasetAddConversation);           

            //Remove dataset
            var sandboxRemoveConversation = await SandboxDatasetOperations.RemoveDatasetExpectSuccess(_restHelper, sandbox.Id, dataset.DatasetId);
            SandboxDatasetAsserts.NewlyRemovedExpectSuccess(sandboxRemoveConversation);
        }      

        async Task PerformAddExpectFailure(Sandbox sandbox)
        {          
            var dataset = sandbox.Study.StudyDatasets.FirstOrDefault();

            var datasetAddConversation = await SandboxDatasetOperations.AddDatasetExpectFailure(_restHelper, sandbox.Id, dataset.DatasetId);
            ApiResponseBasicAsserts.ExpectForbiddenWithMessage(datasetAddConversation.Response);           
        }

        async Task PerformRemoveExpectFailure(Sandbox sandbox)
        {
            var dataset = sandbox.Study.StudyDatasets.FirstOrDefault();
            var datasetRemoveConversation = await SandboxDatasetOperations.RemoveDatasetExpectFailure(_restHelper, sandbox.Id, dataset.DatasetId);
            ApiResponseBasicAsserts.ExpectForbiddenWithMessage(datasetRemoveConversation.Response);
        }
    }
}
