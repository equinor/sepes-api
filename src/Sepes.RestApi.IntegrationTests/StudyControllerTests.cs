using Sepes.RestApi.IntegrationTests.Setup;
using Sepes.RestApi.IntegrationTests.TestHelpers;
using Sepes.Infrastructure.Dto.VirtualMachine;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xunit;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Dto.Study;

namespace Sepes.RestApi.IntegrationTests
{
    [Collection("Integration tests collection")]
    public class StudyControllerTests : IAsyncLifetime
    {
        private const string _endpoint = "api/studies";
        private readonly RestHelper _restHelper;

        public StudyControllerTests(TestHostFixture testHostFixture)
        {
            _restHelper = new RestHelper(testHostFixture.Client);
        }

        [Fact]
        public async Task AddStudy_WithoutVendor_ShouldFail()
        {
            var studyCreateRequest = new StudyCreateDto() { Name = "studyName" };
            var result = await _restHelper.Post<StudyDetailsDto, StudyCreateDto>(_endpoint, studyCreateRequest);
        }

        public Task InitializeAsync() => SliceFixture.ResetCheckpoint();

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
