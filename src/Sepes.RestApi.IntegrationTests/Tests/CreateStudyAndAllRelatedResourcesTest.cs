using Sepes.Infrastructure.Response.Sandbox;
using Sepes.RestApi.IntegrationTests.RequestHelpers;
using Sepes.RestApi.IntegrationTests.Setup;
using Sepes.RestApi.IntegrationTests.Setup.Scenarios;
using Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets.Dataset;
using Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets.Sandbox;
using Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets.Study;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Sepes.RestApi.IntegrationTests
{
    [Collection("Integration tests collection")]
    public class CreateStudyAndAllRelatedResourcesTest : ControllerTestBase
    {
        public CreateStudyAndAllRelatedResourcesTest(TestHostFixture testHostFixture)
            :base (testHostFixture)
        {
         
        }

        [Theory]       
        [InlineData(true, false)]
        //[InlineData(false, true)]
        //[InlineData(true, true)]
        public async Task AddStudyAndSandboxAndVm_WithRequiredRole_ShouldSucceed(bool isAdmin, bool isSponsor)
        {
            await WithBasicSeeds();

            SetScenario(new MockedAzureServiceSets(), isEmployee: true, isAdmin: isAdmin, isSponsor: isSponsor);

            //CREATE STUDY
            var studyCreateResult = await StudyCreator.Create(_restHelper);
            var studyCreateRequest = studyCreateResult.Request;
            var studyResponseWrapper = studyCreateResult.Response;
            CreateStudyAsserts.ExpectSuccess(studyCreateRequest, studyResponseWrapper);

            var createStudyResponse = studyResponseWrapper.Response;

            //CREATE STUDY SPECIFIC DATASET
            var datasetSeedResponse = await DatasetCreator.Create(_restHelper, createStudyResponse.Id);
            var datasetCreateRequest = datasetSeedResponse.Request;
            var datasetResponseWrapper = datasetSeedResponse.Response;        
            CreateDatasetAsserts.ExpectSuccess(datasetCreateRequest, datasetResponseWrapper);

            var createDatasetResponse = datasetResponseWrapper.Response;

            //CREATE SANDBOX
            var sandboxSeedResponse = await SandboxCreator.Create(_restHelper, createStudyResponse.Id);
            var sandboxCreateRequest = sandboxSeedResponse.Request;
            var sandboxResponseWrapper = sandboxSeedResponse.Response;

            CreateSandboxAsserts.ExpectSuccess(sandboxCreateRequest, sandboxResponseWrapper);

            var sandboxResponse = sandboxResponseWrapper.Response;

            //ADD DATASET TO SANDBOX
            var addDatasetToSandboxResponse = await SandboxOperations.AddDataset(_restHelper, sandboxResponse.Id, createDatasetResponse.Id);       
            var sandboxDatasetResponseWrapper = addDatasetToSandboxResponse.Response;           
            AddDatasetToSandboxAsserts.ExpectSuccess(createDatasetResponse.Id, createDatasetResponse.Name, createDatasetResponse.Classification, "Open", sandboxDatasetResponseWrapper);
            
            //CREATE VM
            var virtualMachineSeedResponse = await VirtualMachineCreator.Create(_restHelper, sandboxResponse.Id);
            var virtualMachineCreateRequest = virtualMachineSeedResponse.Request;
            var virtualMachineResponseWrapper = virtualMachineSeedResponse.Response;         

            CreateVirtualMachineAsserts.ExpectSuccess(virtualMachineCreateRequest, sandboxResponse.Region, virtualMachineResponseWrapper);

            //TODO: GET SANDBOX RESOURCE LIST AND ASSERT RESULT BEFORE CREATION

            //TODO: GET SANDBOX VM LIST AND ASSERT RESULT BEFORE CREATION

            //SETUP INFRASTRUCTURE BY RUNNING A METHOD ON THE API            
            _ = await ProcessWorkQueue();            
            
            //GET SANDBOX RESOURCE LIST AND ASSERT RESULT
            var sandboxResourcesResponseWrapper = await _restHelper.Get<List<SandboxResourceLight>>($"api/sandboxes/{sandboxResponse.Id}/resources");
            SandboxResourceListAsserts.ExpectSuccess(sandboxResourcesResponseWrapper);

            //TODO: GET SANDBOX VM LIST AND ASSERT RESULT



            //TODO: OPEN INTERNET

            //TODO: MOVE TO NEXT PHASE

            //TRY TO DELETE STUDY, GET ERROR

            //DELETE STUDY

            //DELETE SANDBOX


        } 
    }
}
