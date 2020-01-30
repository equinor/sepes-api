using System.Collections.Generic;
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
        public async Task TestSetNewPod()
        {
            var users = new List<User>();
            users.Add(new User("1", "1", "1"));
            users.Add(new User("2", "2", "2"));
            var rules = new List<Rule>();
            rules.Add(new Rule(80, "1.1.1.1"));
            rules.Add(new Rule(80, "2.2.2.2"));
            rules.Add(new Rule(3000, "3.3.3.3"));
            var newPod = new Pod(1, "test", 1, false, rules, rules, null, null);

            var azureMock = new Mock<IAzureService>();
            var podService = new PodService(azureMock.Object);


            await podService.Set(newPod, null, users, null);


            azureMock.Verify(az => az.CreateResourceGroup(newPod.resourceGroupName), Times.Once);
            azureMock.Verify(az => az.CreateNetwork(newPod.networkName, newPod.addressSpace, newPod.subnetName), Times.Once);
            azureMock.Verify(az => az.CreateSecurityGroup(newPod.networkSecurityGroupName), Times.Once);
            azureMock.Verify(az => az.NsgAllowInboundPort(newPod.networkSecurityGroupName, "in_80", 100, new string[]{"1.1.1.1", "2.2.2.2"}, 80), Times.Once);
            azureMock.Verify(az => az.NsgAllowInboundPort(newPod.networkSecurityGroupName, "in_3000", 101, new string[]{"3.3.3.3"}, 3000), Times.Once);
            azureMock.Verify(az => az.NsgAllowOutboundPort(newPod.networkSecurityGroupName, "out_80", 100, new string[]{"1.1.1.1", "2.2.2.2"}, 80), Times.Once);
            azureMock.Verify(az => az.NsgAllowOutboundPort(newPod.networkSecurityGroupName, "out_3000", 101, new string[]{"3.3.3.3"}, 3000), Times.Once);
            azureMock.Verify(az => az.ApplySecurityGroup(newPod.networkSecurityGroupName, newPod.subnetName, newPod.networkName), Times.Once);

            azureMock.Verify(az => az.AddUserToNetwork("1", newPod.networkName), Times.Once);
            azureMock.Verify(az => az.AddUserToResourceGroup("1", newPod.resourceGroupName), Times.Once);
            azureMock.Verify(az => az.AddUserToNetwork("2", newPod.networkName), Times.Once);
            azureMock.Verify(az => az.AddUserToResourceGroup("2", newPod.resourceGroupName), Times.Once);
        }

        [Fact]
        public async Task TestSetUpdatedPod()
        {
            //Given
            var azureMock = new Mock<IAzureService>();
            var podService = new PodService(azureMock.Object);

            var users = new List<User>();
            users.Add(new User("1", "1", "1"));
            users.Add(new User("2", "2", "2"));
            var rules = new List<Rule>();
            rules.Add(new Rule(1, "1.1.1.1"));
            var based = new Pod(1, "test", 1, false, rules, rules, null, null);

            var updatedUsers = new List<User>(users);
            updatedUsers.Add(new User("3", "3", "3"));
            var updatedRules = new List<Rule>();
            updatedRules.Add(new Rule(8000, "8.0.0.0"));
            updatedRules.Add(new Rule(8000, "8.0.0.1"));
            var newPod = new Pod(1, "test", 1, false, updatedRules, null, null, null);

            azureMock.Setup(az => az.GetNSGNames()).ReturnsAsync(new string[]{newPod.networkSecurityGroupName});

            //When
            await podService.Set(newPod, based, updatedUsers, users);

            //Then
            // create new nsg
            azureMock.Verify(az => az.CreateSecurityGroup(newPod.networkSecurityGroupName+"2"), Times.Once);
            // should not add old users
            azureMock.Verify(az => az.AddUserToNetwork("1", newPod.networkName), Times.Never);
            azureMock.Verify(az => az.AddUserToResourceGroup("1", newPod.resourceGroupName), Times.Never);
            azureMock.Verify(az => az.AddUserToNetwork("2", newPod.networkName), Times.Never);
            azureMock.Verify(az => az.AddUserToResourceGroup("2", newPod.resourceGroupName), Times.Never);

            // add new user
            azureMock.Verify(az => az.AddUserToNetwork("3", newPod.networkName), Times.Once);
            azureMock.Verify(az => az.AddUserToResourceGroup("3", newPod.resourceGroupName), Times.Once);

            // delete old nsg
            azureMock.Verify(az => az.DeleteSecurityGroup(newPod.networkSecurityGroupName), Times.Once);
        }

        [Fact]
        public async Task TestSetNewPodWithOpenInternet()
        {
            var users = new List<User>();
            var rules = new List<Rule>();
            rules.Add(new Rule(80, "1.1.1.1"));
            rules.Add(new Rule(80, "2.2.2.2"));
            rules.Add(new Rule(3000, "3.3.3.3"));

            bool openInternet = true;
            var newPod = new Pod(1, "test", 1, openInternet, rules, rules, null, null);

            var azureMock = new Mock<IAzureService>();
            var podService = new PodService(azureMock.Object);


            await podService.Set(newPod, null, users, null);


            azureMock.Verify(az => az.CreateResourceGroup(newPod.resourceGroupName), Times.Once);
            azureMock.Verify(az => az.CreateNetwork(newPod.networkName, newPod.addressSpace, newPod.subnetName), Times.Once);
            azureMock.Verify(az => az.CreateSecurityGroup(newPod.networkSecurityGroupName), Times.Never);
            azureMock.Verify(az => az.NsgAllowInboundPort(newPod.networkSecurityGroupName, "in_80", 100, new string[]{"1.1.1.1", "2.2.2.2"}, 80), Times.Never);
            azureMock.Verify(az => az.ApplySecurityGroup(newPod.networkSecurityGroupName, newPod.subnetName, newPod.networkName), Times.Never);
        }

        [Fact]
        public async Task TestSetUpdatedPodWithOpenInternet()
        {
            var users = new List<User>();
            var rules = new List<Rule>();
            rules.Add(new Rule(80, "1.1.1.1"));
            rules.Add(new Rule(80, "2.2.2.2"));
            rules.Add(new Rule(3000, "3.3.3.3"));

            bool openInternet = false;
            var based = new Pod(1, "test", 1, openInternet, rules, rules, null, null);

            openInternet = true;
            var newPod = new Pod(1, "test", 1, openInternet, rules, rules, null, null);

            var azureMock = new Mock<IAzureService>();
            var podService = new PodService(azureMock.Object);


            await podService.Set(newPod, based, users, null);

            azureMock.Verify(az => az.RemoveSecurityGroup(newPod.subnetName, newPod.networkName), Times.Once);

            azureMock.Verify(az => az.CreateResourceGroup(newPod.resourceGroupName), Times.Never);
            azureMock.Verify(az => az.CreateNetwork(newPod.networkName, newPod.addressSpace, newPod.subnetName), Times.Never);
            azureMock.Verify(az => az.CreateSecurityGroup(newPod.networkSecurityGroupName), Times.Never);
            azureMock.Verify(az => az.NsgAllowInboundPort(newPod.networkSecurityGroupName, "in_80", 100, new string[]{"1.1.1.1", "2.2.2.2"}, 80), Times.Never);
            azureMock.Verify(az => az.ApplySecurityGroup(newPod.networkSecurityGroupName, newPod.subnetName, newPod.networkName), Times.Never);
        }
        
    }
}
