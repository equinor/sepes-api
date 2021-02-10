using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Dto.Study;
using Sepes.Infrastructure.Dto.VirtualMachine;
using Sepes.Infrastructure.Response.Sandbox;
using Sepes.RestApi.IntegrationTests.Setup;
using Sepes.RestApi.IntegrationTests.TestHelpers;
using Sepes.Tests.Common.ModelFactory.VirtualMachine;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Sepes.RestApi.IntegrationTests
{
    [Collection("Integration tests collection")]
    public class StudyAndSandboxTest : IAsyncLifetime
    {
        private const string _studiesEndpoint = "api/studies";
        private const string _sandboxEndpoint = "api/studies/{0}/sandboxes";
        private const string _vmEndpoint = "api/virtualmachines/{0}";

        private readonly TestHostFixture _testHostFixture;
        private RestHelper _restHelper;

        public StudyAndSandboxTest(TestHostFixture testHostFixture)
        {
            _testHostFixture = testHostFixture;
            _restHelper = new RestHelper(testHostFixture.Client);
        }

        void SetUserType(bool isEmployee = false, bool isAdmin = false, bool isSponsor = false, bool isDatasetAdmin = false)
        {
            _testHostFixture.SetUserType(isEmployee, isAdmin, isSponsor, isDatasetAdmin);
            _restHelper = new RestHelper(_testHostFixture.Client);
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(true, true)]
        public async Task AddStudyAndSandboxAndVm_WithRequiredRole_ShouldSucceed(bool isAdmin, bool isSponsor)
        {
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
            var sandboxResponseWrapper = await _restHelper.Post<SandboxDetails, SandboxCreateDto>(String.Format(_sandboxEndpoint, studyDto.Id), sandboxCreateDto);
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
            Assert.Equal(vmCreateDto.Name, vmDto.Name);
            Assert.Equal(vmCreateDto.OperatingSystem, vmDto.OperatingSystem);
            Assert.Equal(sandboxDto.Region, vmDto.Region);//Same region as sandbox
        }

        public Task InitializeAsync() => SliceFixture.ResetCheckpoint();

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
