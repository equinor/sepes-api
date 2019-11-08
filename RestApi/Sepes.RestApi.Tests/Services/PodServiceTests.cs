using System.Text.Json;
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
            azureMock.Verify(azure => azure.CreateResourceGroup("42-test-ResourceGroup"), Times.Once);
            azureMock.VerifyNoOtherCalls();
        }
        [Fact]
        public async Task GetPods()
        {
            //Given
            var testPod = new Pod(1, "test", 42);
            var databaseMock = new Mock<ISepesDb>();
            databaseMock.Setup(db => db.getPods(42)).ReturnsAsync(JsonSerializer.Serialize(new Pod(42, "name", 42)));
            var azureMock = new Mock<IAzureService>();
            var service = new PodService(databaseMock.Object, azureMock.Object);

            //When
            var pod = await service.GetPods(42);

            //Then
            Assert.Equal(JsonSerializer.Serialize(new Pod(42, "name", 42)), pod);
        }
        [Fact]
        public async Task CreateNsg()
        {
            //Given
            var testPod = new Pod(1, "test", 42);
            var databaseMock = new Mock<ISepesDb>();
            var azureMock = new Mock<IAzureService>();
            var service = new PodService(databaseMock.Object, azureMock.Object);
            

            //When
            await service.createNsg(testPod.networkSecurityGroupName, testPod.resourceGroupName);
            
            //Then
            azureMock.Verify(azure => azure.CreateSecurityGroup("42-test-NetworkSecurityGroup","42-test-ResourceGroup"));
            azureMock.VerifyNoOtherCalls();
        }
        [Fact]
        public async Task RemoveNsg()
        {
            //Given
            var testPod = new Pod(1, "test", 42);
            var databaseMock = new Mock<ISepesDb>();
            var azureMock = new Mock<IAzureService>();
            var service = new PodService(databaseMock.Object, azureMock.Object);
            

            //When
            await service.deleteNsg(testPod.networkSecurityGroupName, testPod.resourceGroupName);
            
            //Then
            azureMock.Verify(azure => azure.DeleteSecurityGroup("42-test-NetworkSecurityGroup","42-test-ResourceGroup"));
            azureMock.VerifyNoOtherCalls();
        }
        [Fact]
        public async Task ApplyNsg()
        {
            //Given
            var testPod = new Pod(1, "test", 42);
            var databaseMock = new Mock<ISepesDb>();
            var azureMock = new Mock<IAzureService>();
            var service = new PodService(databaseMock.Object, azureMock.Object);
            

            //When
            await service.applyNsg( testPod.resourceGroupName, testPod.networkSecurityGroupName, testPod.subnetName, testPod.networkName);
            
            //Then
            azureMock.Verify(azure => azure.ApplySecurityGroup("42-test-ResourceGroup", "42-test-NetworkSecurityGroup","42-test-SubNet","42-test-Network"));
            azureMock.VerifyNoOtherCalls();
        }
        [Fact]
        public async Task CreateRule()
        {
            //Given
            var testPod = new Pod(1, "test", 42);
            var databaseMock = new Mock<ISepesDb>();
            var azureMock = new Mock<IAzureService>();
            var service = new PodService(databaseMock.Object, azureMock.Object);
            

            //When
            await service.removeNsg(testPod.resourceGroupName,testPod.subnetName,testPod.networkName);
            
            //Then
            azureMock.Verify(azure => azure.RemoveSecurityGroup("42-test-ResourceGroup","42-test-SubNet","42-test-Network"));
            azureMock.VerifyNoOtherCalls();
        }
    }
}