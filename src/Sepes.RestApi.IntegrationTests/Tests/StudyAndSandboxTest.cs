using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Dto.Study;
using Sepes.Infrastructure.Dto.VirtualMachine;
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
        private const string _studiesEndpoint = "api/studies";
        private const string _sandboxEndpoint = "api/studies/{0}/sandboxes";
        private const string _vmEndpoint = "api/virtualmachines/{0}";      

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

            var studyCreateDto = new StudyCreateDto() { Name = "studyName", Vendor = "Vendor", WbsCode = "wbs" };
            var studyResponseWrapper = await _restHelper.Post<StudyDto, StudyCreateDto>(_studiesEndpoint, studyCreateDto);
            Assert.Equal(System.Net.HttpStatusCode.OK, studyResponseWrapper.StatusCode);
            Assert.NotNull(studyResponseWrapper.Response);

            var studyDto = studyResponseWrapper.Response;

            Assert.NotEqual<int>(0, studyDto.Id);
            Assert.Equal(studyCreateDto.Name, studyDto.Name);
            Assert.Equal(studyCreateDto.Vendor, studyDto.Vendor);
            Assert.Equal(studyCreateDto.WbsCode, studyDto.WbsCode);

            //CREATE SANDBOX
            var sandboxCreateDto = new SandboxCreateDto() { Name = "sandboxName", Region = "norwayeast" };
            var sandboxResponseWrapper = await _restHelper.Post<SandboxDetailsDto, SandboxCreateDto>(String.Format(_sandboxEndpoint, studyDto.Id), sandboxCreateDto);
            Assert.Equal(System.Net.HttpStatusCode.OK, sandboxResponseWrapper.StatusCode);
            Assert.NotNull(sandboxResponseWrapper.Response);

            var sandboxDto = sandboxResponseWrapper.Response;
            Assert.NotEqual<int>(0, sandboxDto.Id);
            Assert.Equal(sandboxCreateDto.Name, sandboxDto.Name);
            Assert.Equal(sandboxCreateDto.Region, sandboxDto.Region);

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
        } 
    }
}
