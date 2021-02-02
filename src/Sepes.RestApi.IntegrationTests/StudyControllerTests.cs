using Sepes.Infrastructure.Dto.Study;
using Sepes.RestApi.IntegrationTests.Setup;
using Sepes.RestApi.IntegrationTests.TestHelpers;
using System.Threading.Tasks;
using Xunit;

namespace Sepes.RestApi.IntegrationTests
{
    [Collection("Integration tests collection")]
    public class StudyControllerTests : IAsyncLifetime
    {
        private const string _endpoint = "api/studies";

        private readonly TestHostFixture _testHostFixture;
        private RestHelper _restHelper;

        public StudyControllerTests(TestHostFixture testHostFixture)
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
        public async Task AddStudy_WithoutVendor_ShouldFail(bool isAdmin, bool isSponsor)
        {
            SetUserType(isEmployee: true, isAdmin, isSponsor);

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
            SetUserType(isEmployee: isEmployee, isDatasetAdmin: isDatasetAdmin);

            var studyCreateRequest = new StudyCreateDto() { Name = "studyName", Vendor = "Vendor" };
            var responseWrapper = await _restHelper.Post<Infrastructure.Dto.ErrorResponse, StudyCreateDto>(_endpoint, studyCreateRequest);
            Assert.Equal(System.Net.HttpStatusCode.Forbidden, responseWrapper.StatusCode);
            Assert.Contains("does not have permission to perform operation", responseWrapper.Response.Message);
        }

        public Task InitializeAsync() => SliceFixture.ResetCheckpoint();

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
