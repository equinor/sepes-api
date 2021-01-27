using Sepes.RestApi.IntegrationTests.Setup;
using Sepes.RestApi.IntegrationTests.TestHelpers;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xunit;

namespace Sepes.RestApi.IntegrationTests
{
    [Collection("Integration tests collection")]
    public class PerformanceStandardTests : IAsyncLifetime
    {
        private const string _endpoint = "/performance-standards";
        private readonly RestHelper _restHelper;

        public PerformanceStandardTests(TestHostFixture testHostFixture)
        {
            _restHelper = new RestHelper(testHostFixture.Client);
        }
        /*
        [Fact]
        public async Task GetAllPerformanceStandards()
        {
            //Arrange
            await SliceFixture.InsertAsync(new PerformanceStandard  { StandardId = "First PerformanceStandard Id" } );
            await SliceFixture.InsertAsync(new PerformanceStandard { StandardId = "Second PerformanceStandard Id" });

            //Act
            var result = await _restHelper.Get<ReadOnlyCollection<PerformanceStandard>>(_endpoint);

            //Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, x => x.StandardId.Equals("Second PerformanceStandard Id"));
        }
        */
        public Task InitializeAsync() => SliceFixture.ResetCheckpoint();

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
