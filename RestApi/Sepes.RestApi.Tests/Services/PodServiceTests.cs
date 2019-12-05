using System.Collections.Generic;
using System.Linq;
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
            azureMock.Verify(azure => azure.CreateNetwork("42-test-Network", "10.1.1.0/24", "subnet1"), Times.Once);
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

        [Fact]
        public async Task TestSetNewPod()
        {
            var users = new List<User>();
            users.Add(new User("1", "1", "1"));
            users.Add(new User("2", "2", "2"));
            var rules = new List<Rule>();
            rules.Add(new Rule(80, "1.1.1.1"));
            rules.Add(new Rule(80, "2.2.2.2"));
            rules.Add(new Rule(3000, "3.3.3.3"));
            var newPod = new Pod(1, "test", 1, false, rules, rules, users, null, null);

            var databaseMock = new Mock<ISepesDb>();
            var azureMock = new Mock<IAzureService>();
            var podService = new PodService(databaseMock.Object, azureMock.Object);


            await podService.Set(newPod, null);


            azureMock.Verify(az => az.CreateResourceGroup(newPod.resourceGroupName), Times.Once);
            azureMock.Verify(az => az.CreateNetwork(newPod.networkName, newPod.addressSpace, newPod.subnetName), Times.Once);
            azureMock.Verify(az => az.CreateSecurityGroup(newPod.networkSecurityGroupName, newPod.resourceGroupName), Times.Once);
            azureMock.Verify(az => az.NsgAllowInboundPort(newPod.networkSecurityGroupName, newPod.resourceGroupName, "Port_80", 100, new string[]{"1.1.1.1", "2.2.2.2"}, 80), Times.Once);
            azureMock.Verify(az => az.NsgAllowInboundPort(newPod.networkSecurityGroupName, newPod.resourceGroupName, "Port_3000", 101, new string[]{"3.3.3.3"}, 3000), Times.Once);
            azureMock.Verify(az => az.NsgAllowOutboundPort(newPod.networkSecurityGroupName, newPod.resourceGroupName, "Port_80", 100, new string[]{"1.1.1.1", "2.2.2.2"}, 80), Times.Once);
            azureMock.Verify(az => az.NsgAllowOutboundPort(newPod.networkSecurityGroupName, newPod.resourceGroupName, "Port_3000", 101, new string[]{"3.3.3.3"}, 3000), Times.Once);
            azureMock.Verify(az => az.ApplySecurityGroup(newPod.resourceGroupName, newPod.networkSecurityGroupName, newPod.subnetName, newPod.networkName), Times.Once);

            azureMock.Verify(az => az.AddUserToNetwork("1", newPod.networkName), Times.Once);
            azureMock.Verify(az => az.AddUserToResourceGroup("1", newPod.resourceGroupName), Times.Once);
            azureMock.Verify(az => az.AddUserToNetwork("2", newPod.networkName), Times.Once);
            azureMock.Verify(az => az.AddUserToResourceGroup("2", newPod.resourceGroupName), Times.Once);
        }

        [Fact]
        public async Task TestSetUpdatedPod()
        {
            //Given
            var databaseMock = new Mock<ISepesDb>();
            var azureMock = new Mock<IAzureService>();
            var podService = new PodService(databaseMock.Object, azureMock.Object);

            var users = new List<User>();
            users.Add(new User("1", "1", "1"));
            users.Add(new User("2", "2", "2"));
            var based = new Pod(1, "test", 1, false, null, null, users, null, null);

            var updatedUsers = new List<User>(users);
            updatedUsers.Add(new User("3", "3", "3"));
            var newPod = new Pod(1, "test", 1, false, null, null, updatedUsers, null, null);

            azureMock.Setup(az => az.GetNSGNames(newPod.resourceGroupName)).ReturnsAsync(new string[]{newPod.networkSecurityGroupName});

            //When
            await podService.Set(newPod, based);

            //Then
            // create new nsg
            azureMock.Verify(az => az.CreateSecurityGroup(newPod.networkSecurityGroupName+"2", newPod.resourceGroupName), Times.Once);
            // should not add old users
            azureMock.Verify(az => az.AddUserToNetwork("1", newPod.networkName), Times.Never);
            azureMock.Verify(az => az.AddUserToResourceGroup("1", newPod.resourceGroupName), Times.Never);
            azureMock.Verify(az => az.AddUserToNetwork("2", newPod.networkName), Times.Never);
            azureMock.Verify(az => az.AddUserToResourceGroup("2", newPod.resourceGroupName), Times.Never);

            // add new user
            azureMock.Verify(az => az.AddUserToNetwork("3", newPod.networkName), Times.Once);
            azureMock.Verify(az => az.AddUserToResourceGroup("3", newPod.resourceGroupName), Times.Once);

            // delete old nsg
            azureMock.Verify(az => az.DeleteSecurityGroup(newPod.networkSecurityGroupName, newPod.resourceGroupName), Times.Once);
        }
        
    }
}
