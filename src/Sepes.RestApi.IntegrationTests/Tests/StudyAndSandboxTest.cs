using Sepes.Infrastructure.Dto.Dataset;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Dto.Study;
using Sepes.Infrastructure.Dto.VirtualMachine;
using Sepes.Infrastructure.Response.Sandbox;
using Sepes.RestApi.IntegrationTests.Setup;
using Sepes.Tests.Common.ModelFactory.VirtualMachine;
using System;
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
        [InlineData(false, true)]
        [InlineData(true, true)]
        public async Task AddStudyAndSandboxAndVm_WithRequiredRole_ShouldSucceed(bool isAdmin, bool isSponsor)
        {
            await WithBasicSeeds();

            SetUserType(isEmployee: true, isAdmin: isAdmin, isSponsor: isSponsor);

            //CREATE STUDY
            var studyCreateDto = new StudyCreateDto() { Name = "studyName", Vendor = "Vendor", WbsCode = "wbs" };
            var studyResponseWrapper = await _restHelper.Post<StudyDto, StudyCreateDto>(_studiesEndpoint, studyCreateDto);
            Assert.Equal(System.Net.HttpStatusCode.OK, studyResponseWrapper.StatusCode);
            Assert.NotNull(studyResponseWrapper.Response);

            var studyDto = studyResponseWrapper.Response;

            Assert.NotEqual<int>(0, studyDto.Id);
            Assert.Equal(studyCreateDto.Name, studyDto.Name);
            Assert.Equal(studyCreateDto.Vendor, studyDto.Vendor);
            Assert.Equal(studyCreateDto.WbsCode, studyDto.WbsCode);

            //CREATE STUDY SPECIFIC DATASET
            var datasetCreateRequest = new DatasetCreateUpdateInputBaseDto() { Location = "norwayeast", Name = "datasetName", Classification = "open" };
            var dataseResponseWrapper = await _restHelper.Post<DatasetDto, DatasetCreateUpdateInputBaseDto>(String.Format(_studySpecificDatasetEndpoint, studyDto.Id), datasetCreateRequest);
            Assert.Equal(System.Net.HttpStatusCode.OK, dataseResponseWrapper.StatusCode);
            Assert.NotNull(dataseResponseWrapper.Response);
            var createDatasetResponse = dataseResponseWrapper.Response;
            Assert.NotEqual<int>(0, createDatasetResponse.Id);
            Assert.Equal(datasetCreateRequest.Name, createDatasetResponse.Name);
            Assert.Equal(datasetCreateRequest.Classification, createDatasetResponse.Classification);

            //CREATE SANDBOX
            var sandboxCreateDto = new SandboxCreateDto() { Name = "sandboxName", Region = "norwayeast" };
            var sandboxResponseWrapper = await _restHelper.Post<SandboxDetails, SandboxCreateDto>(String.Format(_sandboxEndpoint, studyDto.Id), sandboxCreateDto);
            Assert.Equal(System.Net.HttpStatusCode.OK, sandboxResponseWrapper.StatusCode);
            Assert.NotNull(sandboxResponseWrapper.Response);

            var sandboxDto = sandboxResponseWrapper.Response;
            Assert.NotEqual<int>(0, sandboxDto.Id);
            Assert.Equal(sandboxCreateDto.Name, sandboxDto.Name);
            Assert.Equal(sandboxCreateDto.Region, sandboxDto.Region);

            //ADD DATASET TO SANDBOX
            var sandboxDatasetResponseWrapper = await _restHelper.Put<AvailableDatasets>(String.Format(_sandboxDatasetEndpoint, sandboxDto.Id, createDatasetResponse.Id));
            Assert.Equal(System.Net.HttpStatusCode.OK, sandboxDatasetResponseWrapper.StatusCode);
            Assert.NotNull(sandboxDatasetResponseWrapper.Response);
            var addDatasetResponse = sandboxDatasetResponseWrapper.Response;
            Assert.Equal("Open", addDatasetResponse.Classification);
            Assert.NotEmpty(addDatasetResponse.Datasets);

            //CREATE VM
            var vmCreateDto = CreateVmDtoFactory.New("integrationtest");

            var vmResponseWrapper = await _restHelper.Post<VmDto, VirtualMachineCreateDto>(String.Format(_vmEndpoint, sandboxDto.Id), vmCreateDto);
            Assert.Equal(System.Net.HttpStatusCode.OK, vmResponseWrapper.StatusCode);
            Assert.NotNull(vmResponseWrapper.Response);

            var vmDto = vmResponseWrapper.Response;
            Assert.NotEqual<int>(0, vmDto.Id);
            Assert.Contains(vmCreateDto.Name, vmDto.Name);
            Assert.Equal(vmCreateDto.OperatingSystem, vmDto.OperatingSystem);
            Assert.Equal(sandboxDto.Region, vmDto.Region);//Same region as sandbox

            //OPEN INTERNET

            //MOVE TO NEXT PHASE


        } 
    }
}
