
using System;
using System.Threading.Tasks;
using Sepes.RestApi.Model;

namespace Sepes.RestApi.Services
{
    // This service manage changes to Pods. Make sure they are validated and the correct azure actions are performed.
    // It do not own the Pod state and need to talk to StudyService to keep its up to date.
    public interface IPodService
    {
        Task<Pod> CreateNewPod(string name, int userID);
        Task applyNsg(string resourceGroupName, string securityGroupName, string subnetName, string networkName);
        Task removeNsg(string resourceGroupName, string subnetName, string networkName);
        Task<UInt16> deleteUnused();
        Task deleteNsg(string securityGroupName, string resourceGroupName);
        Task createNsg(string securityGroupName, string resourceGroupName);
    }

    public class PodService : IPodService
    {
        private readonly ISepesDb _database;
        private readonly IAzureService _azure;

        public PodService(ISepesDb database, IAzureService azure)
        {
            _database = database;
            _azure = azure;
        }

        public async Task<Pod> CreateNewPod(string name, int studyID)
        {

            var pod = await _database.createPod(name, studyID);
            var resourceGroupName = await _azure.CreateResourceGroup(pod.resourceGroupName);
            await _azure.CreateNetwork(pod.networkName, pod.addressSpace);
            return pod;
        }
        public async Task createNsg(string securityGroupName, string resourceGroupName)
        {
            await _azure.CreateSecurityGroup(securityGroupName, resourceGroupName);
        }
        public async Task deleteNsg(string securityGroupName, string resourceGroupName){
            await _azure.DeleteSecurityGroup(securityGroupName, resourceGroupName);
        }
        public async Task applyNsg(string resourceGroupName, string securityGroupName, string subnetName, string networkName)
        {
            //throw new NotImplementedException();
            await _azure.ApplySecurityGroup(resourceGroupName ,securityGroupName, subnetName, networkName);

        }
        public async Task removeNsg(string resourceGroupName, string subnetName, string networkName)
        {
            await _azure.RemoveSecurityGroup(resourceGroupName, subnetName, networkName);
        }
        public async Task<UInt16> deleteUnused()
        {
            throw new NotImplementedException();
            //needs to check for any policies that are not currently in use by any pods/belong to deleted pods.
        }
    }
}
