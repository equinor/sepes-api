using System.Threading.Tasks;
using Moq;
using Sepes.RestApi.Model;
using Sepes.RestApi.Services;
using Xunit;

namespace Sepes.RestApi.Tests.Services
{
    public class PodServiceTests
    {
        [Fact]
        public async Task CreateNewPod()
        {
            //Given
            var testPod = new Pod(1, "test", 42);
            var databaseMock = new Mock<ISepesDb>();
            databaseMock.Setup(db => db.createPod("test", 42)).ReturnsAsync(testPod);
            var azureMock = new Mock<IAzureService>();
            var service = new PodService(databaseMock.Object, azureMock.Object);

            //When
            var pod = await service.CreateNewPod("test", 42);

            //Then
            Assert.Equal(testPod, pod);
            azureMock.Verify(azure => azure.CreateNetwork("42-test-Network", "10.1.1.0/24"), Times.Once);
            azureMock.VerifyNoOtherCalls();
        }
    }
}