using Sepes.Common.Constants;
using Sepes.RestApi.IntegrationTests.RequestHelpers;
using Sepes.RestApi.IntegrationTests.Setup;
using Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets;
using Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets.Dataset;
using Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets.Sandbox;
using Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets.StudyParticipant;
using System.Threading.Tasks;
using Xunit;

namespace Sepes.RestApi.IntegrationTests.Tests
{
    [Collection("Integration tests collection")]
    public class StudyParticipantControllerConcurrencyTests : ControllerTestBase
    {
        public StudyParticipantControllerConcurrencyTests(TestHostFixture testHostFixture)
            : base(testHostFixture)
        {

        }

        [Theory]
        [InlineData(true)]        
        public async Task AddParticipantWhileSandboxIsCreating_ShouldSucceed(bool isAdmin)
        {
            await WithBasicSeeds();

            SetScenario(isEmployee: true, isAdmin: isAdmin);

            //CREATE STUDY
            var studyCreateConversation = await StudyCreator.CreateAndExpectSuccess(_restHelper);
            CreateStudyAsserts.ExpectSuccess(studyCreateConversation.Request, studyCreateConversation.Response);

            //CREATE STUDY SPECIFIC DATASET
            var datasetSeedResponse = await DatasetCreator.Create(_restHelper, studyCreateConversation.Response.Content.Id);
            var datasetCreateRequest = datasetSeedResponse.Request;
            var datasetResponseWrapper = datasetSeedResponse.Response;
            CreateDatasetAsserts.ExpectSuccess(datasetCreateRequest, datasetResponseWrapper);

            var createDatasetResponse = datasetResponseWrapper.Content;

            //CREATE SANDBOX
            var sandboxSeedResponse = await SandboxCreator.CreateAndExpectSuccess(_restHelper, studyCreateConversation.Response.Content.Id);
            var sandboxCreateRequest = sandboxSeedResponse.Request;
            var sandboxResponseWrapper = sandboxSeedResponse.Response;

            SandboxDetailsAsserts.NewlyCreatedExpectSuccess(sandboxCreateRequest, sandboxResponseWrapper);

            var sandboxResponse = sandboxResponseWrapper.Content;

            //ADD DATASET TO SANDBOX
            var addDatasetToSandboxResponse = await SandboxOperations.AddDataset(_restHelper, sandboxResponse.Id, createDatasetResponse.Id);
            var sandboxDatasetResponseWrapper = addDatasetToSandboxResponse.Response;
            AddDatasetToSandboxAsserts.ExpectSuccess(createDatasetResponse.Id, createDatasetResponse.Name, createDatasetResponse.Classification, "Open", sandboxDatasetResponseWrapper);

            //TODO: Add participant
            var roleToAdd = StudyRoles.SponsorRep;
            var studyId = studyCreateConversation.Response.Content.Id;          
            var responseDto = StudyParticipantAdderAndRemover.CreateParticipantLookupDto();

            var studyParticipantAddConversation = await StudyParticipantAdderAndRemover.AddAndExpectSuccess(_restHelper, studyId, roleToAdd, responseDto);
            AddStudyParticipantsAsserts.ExpectSuccess(roleToAdd, studyParticipantAddConversation.Request, studyParticipantAddConversation.Response);

            var processWorkQueueResponse = await ProcessWorkQueue(1);


            ////CREATE VM
            //var virtualMachineSeedResponse = await VirtualMachineCreator.Create(_restHelper, sandboxResponse.Id);
            //var virtualMachineCreateRequest = virtualMachineSeedResponse.Request;
            //var virtualMachineResponseWrapper = virtualMachineSeedResponse.Response;

            //CreateVirtualMachineAsserts.ExpectSuccess(virtualMachineCreateRequest, sandboxResponse.Region, virtualMachineResponseWrapper);

            //TODO: GET SANDBOX RESOURCE LIST AND ASSERT RESULT BEFORE CREATION

            //TODO: GET SANDBOX VM LIST AND ASSERT RESULT BEFORE CREATION

            //SETUP INFRASTRUCTURE BY RUNNING A METHOD ON THE API            
            //processWorkQueueResponse = await ProcessWorkQueue(5);

            ////GET SANDBOX RESOURCE LIST AND ASSERT RESULT
            //var sandboxResourcesResponseWrapper = await _restHelper.Get<List<SandboxResourceLight>>($"api/sandboxes/{sandboxResponse.Id}/resources");
            //SandboxResourceListAsserts.ExpectSuccess(sandboxResourcesResponseWrapper);

            //TODO: GET SANDBOX VM LIST AND ASSERT RESULT

            //TODO: Add some participants

            //TODO: OPEN INTERNET

            //TODO: MOVE TO NEXT PHASE

            //TRY TO DELETE STUDY, GET ERROR

            //DELETE STUDY

            //DELETE SANDBOX


        }
    }
}
