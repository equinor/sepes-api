using Sepes.Infrastructure.Dto.Study;
using Sepes.RestApi.IntegrationTests.RequestHelpers;
using Sepes.RestApi.IntegrationTests.Setup;
using Sepes.RestApi.IntegrationTests.Setup.Scenarios;
using Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets.Study;
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
            SetScenario(new MockedAzureServiceSets(), isEmployee: true, isAdmin, isSponsor);           

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
            SetScenario(new MockedAzureServiceSets(), isEmployee: isEmployee, isDatasetAdmin: isDatasetAdmin);      

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
            SetScenario(new MockedAzureServiceSets(), isEmployee: true, isAdmin: isAdmin, isSponsor: isSponsor);

            var studySeedResponse = await StudyCreator.Create(_restHelper);
            var studyCreateRequest = studySeedResponse.Request;
            var studyResponseWrapper = studySeedResponse.Response;

            CreateStudyAsserts.ExpectSuccess(studyCreateRequest, studyResponseWrapper);
        }

        [Theory]
        [InlineData(true, false)]      
        public async Task AddStudy_ShouldCreateResourceGroupForStudySpecificDatasets(bool isAdmin, bool isSponsor)
        {
            SetScenario(new MockedAzureServiceSets(), isEmployee: true, isAdmin: isAdmin, isSponsor: isSponsor);

            var studySeedResponse = await StudyCreator.Create(_restHelper);
            var studyCreateRequest = studySeedResponse.Request;
            var studyResponseWrapper = studySeedResponse.Response;
            CreateStudyAsserts.ExpectSuccess(studyCreateRequest, studyResponseWrapper);

            //Look in database, should have a resource group defined

            //SETUP INFRASTRUCTURE BY RUNNING A METHOD ON THE API            
            var processWorkQueueResponse = await ProcessWorkQueue();

            //Look in database, resource group should appear to have been created
        }
    }
}
