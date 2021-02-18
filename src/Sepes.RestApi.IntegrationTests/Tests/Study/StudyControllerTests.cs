using Sepes.Infrastructure.Dto.Study;
using Sepes.RestApi.IntegrationTests.Setup;
using Sepes.RestApi.IntegrationTests.Setup.Scenarios;
using System.Threading.Tasks;
using Xunit;

namespace Sepes.RestApi.IntegrationTests.Tests
{
    [Collection("Integration tests collection")]
    public class StudyControllerTests : ControllerTestBase
    {
        const string _endpoint = "api/studies";      
       

        public StudyControllerTests(TestHostFixture testHostFixture)
            :base(testHostFixture)
        {
         
        }      

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(true, true)]
        public async Task AddStudy_WithoutVendor_ShouldFail(bool isAdmin, bool isSponsor)
        {
            SetScenario(new E2EHappyPathServices(), isEmployee: true, isAdmin, isSponsor);           

            var studyCreateRequest = new StudyCreateDto() { Name = "studyName" };
            var responseWrapper = await _restHelper.Post<Infrastructure.Dto.ErrorResponse, StudyCreateDto>(_endpoint, studyCreateRequest);
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, responseWrapper.StatusCode);
            Assert.Contains("The Vendor field is required", responseWrapper.Response.Message);
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(true, true)]
        public async Task AddStudy_WithoutRequiredRole_ShouldFail(bool isEmployee, bool isDatasetAdmin)
        {
            SetScenario(new E2EHappyPathServices(), isEmployee: isEmployee, isDatasetAdmin: isDatasetAdmin);      

            var studyCreateRequest = new StudyCreateDto() { Name = "studyName", Vendor = "Vendor" };
            var responseWrapper = await _restHelper.Post<Infrastructure.Dto.ErrorResponse, StudyCreateDto>(_endpoint, studyCreateRequest);
            Assert.Equal(System.Net.HttpStatusCode.Forbidden, responseWrapper.StatusCode);
            Assert.Contains("does not have permission to perform operation", responseWrapper.Response.Message);
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(true, true)]
        public async Task AddStudy_WithRequiredRole_ShouldSucceed(bool isAdmin, bool isSponsor)
        {          
            SetScenario(new E2EHappyPathServices(), isEmployee: true, isAdmin: isAdmin, isSponsor: isSponsor);

            var studyCreateDto = new StudyCreateDto() { Name = "studyName", Vendor = "Vendor", WbsCode= "wbs" };
            var responseWrapper = await _restHelper.Post<StudyDto, StudyCreateDto>(_endpoint, studyCreateDto);
            Assert.Equal(System.Net.HttpStatusCode.OK, responseWrapper.StatusCode);
            Assert.NotNull(responseWrapper.Response);

            var studyDto = responseWrapper.Response;           

            Assert.NotEqual<int>(0, studyDto.Id);
            Assert.Equal(studyCreateDto.Name,  studyDto.Name);
            Assert.Equal(studyCreateDto.Vendor, studyDto.Vendor);
            Assert.Equal(studyCreateDto.WbsCode, studyDto.WbsCode);
        }       
    }
}
