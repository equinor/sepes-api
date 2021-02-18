using Sepes.Infrastructure.Dto.Dataset;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Dto.Study;
using Sepes.Infrastructure.Dto.VirtualMachine;
using Sepes.Infrastructure.Response.Sandbox;
using Sepes.RestApi.IntegrationTests.Setup;
using Sepes.RestApi.IntegrationTests.Setup.Scenarios;
using Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets.Dataset;
using Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets.Sandbox;
using Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets.Study;
using Sepes.Tests.Common.ModelFactory.VirtualMachine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Sepes.RestApi.IntegrationTests
{
    [Collection("Integration tests collection")]
    public class StudyAndSandboxTest : ControllerTestBase
    {
        const string _studiesEndpoint = "api/studies";
        const string _studySpecificDatasetEndpoint = "api/studies/{0}/datasets/studyspecific";
        const string _sandboxEndpoint = "api/studies/{0}/sandboxes";
        const string _sandboxDatasetEndpoint = "api/sandbox/{0}/datasets/{1}";
        const string _vmEndpoint = "api/virtualmachines/{0}";     

        public StudyAndSandboxTest(TestHostFixture testHostFixture)
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

            SetScenario(new E2EHappyPathServices(), isEmployee: true, isAdmin: isAdmin, isSponsor: isSponsor);

            //CREATE STUDY
            var studyCreateDto = new StudyCreateDto() { Name = "studyName", Vendor = "Vendor", WbsCode = "wbs" };
            var studyResponseWrapper = await _restHelper.Post<StudyDto, StudyCreateDto>(_studiesEndpoint, studyCreateDto);
            CreateStudyAsserts.ExpectSuccess(studyCreateDto, studyResponseWrapper);

            var createStudyResponse = studyResponseWrapper.Response;

            //CREATE STUDY SPECIFIC DATASET
            var datasetCreateRequest = new DatasetCreateUpdateInputBaseDto() { Location = "norwayeast", Name = "datasetName", Classification = "open" };
            var dataseResponseWrapper = await _restHelper.Post<DatasetDto, DatasetCreateUpdateInputBaseDto>(String.Format(_studySpecificDatasetEndpoint, createStudyResponse.Id), datasetCreateRequest);
            CreateDatasetAsserts.ExpectSuccess(datasetCreateRequest, dataseResponseWrapper);

            var createDatasetResponse = dataseResponseWrapper.Response;          

            //CREATE SANDBOX
            var sandboxCreateDto = new SandboxCreateDto() { Name = "sandboxName", Region = "norwayeast" };
            var sandboxResponseWrapper = await _restHelper.Post<SandboxDetails, SandboxCreateDto>(String.Format(_sandboxEndpoint, createStudyResponse.Id), sandboxCreateDto);

            CreateSandboxAsserts.ExpectSuccess(sandboxCreateDto, sandboxResponseWrapper);

            var sandboxDto = sandboxResponseWrapper.Response;

            //ADD DATASET TO SANDBOX
            var sandboxDatasetResponseWrapper = await _restHelper.Put<AvailableDatasets>(String.Format(_sandboxDatasetEndpoint, sandboxDto.Id, createDatasetResponse.Id));
            AddDatasetToSandboxAsserts.ExpectSuccess(createDatasetResponse.Id, createDatasetResponse.Name, createDatasetResponse.Classification, "Open", sandboxDatasetResponseWrapper);
            
            var addDatasetToSandboxResponse = sandboxDatasetResponseWrapper.Response;

            //CREATE VM
            var vmCreateDto = CreateVmDtoFactory.New("integrationtest");

            var vmResponseWrapper = await _restHelper.Post<VmDto, VirtualMachineCreateDto>(String.Format(_vmEndpoint, sandboxDto.Id), vmCreateDto);

            CreateVirtualMachineAsserts.ExpectSuccess(vmCreateDto, sandboxDto.Region, vmResponseWrapper);

            var createVmResponse = vmResponseWrapper.Response;

            //TODO: GET SANDBOX RESOURCE LIST AND ASSERT RESULT BEFORE CREATION

            //SETUP INFRASTRUCTURE BY RUNNING A METHOD ON THE API
            //SetUserType(isAdmin: true); //If this test will be ran as non-admins, must find a way to set admin before running this
            var doWorkResponseWrapper = await _restHelper.Get("api/provisioningqueue/lookforwork");
            Assert.Equal(System.Net.HttpStatusCode.OK, doWorkResponseWrapper.StatusCode);
            
            
            //GET SANDBOX RESOURCE LIST AND ASSERT RESULT
            var sandboxResourcesResponseWrapper = await _restHelper.Get<List<SandboxResourceLight>>($"api/sandboxes/{sandboxDto.Id}/resources");

            SandboxResourceListAsserts.HappyPathAssert(sandboxResourcesResponseWrapper);

            //OPEN INTERNET

            //MOVE TO NEXT PHASE


        } 
    }
}
