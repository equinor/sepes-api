
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
        Task applyNsg(string securityGroupName, string subnetName, string networkId);
        Task switchNsg(string securityGroupNameOld, string securityGroupNameNew, string subnetName, string networkId);
        Task removeNsg(string subnetName, string networkId);
        Task<UInt16> deleteUnused();
        Task deleteNsg(string securityGroupName);
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
        public async Task deleteNsg(string securityGroupName){
            await _azure.DeleteSecurityGroup(securityGroupName);
        }
        public async Task applyNsg(string securityGroupName, string subnetName, string networkId)
        {
            //throw new NotImplementedException();
            await _azure.ApplySecurityGroup(securityGroupName, subnetName, networkId);

        }
        public async Task removeNsg(string subnetName, string networkId)
        {
            await _azure.RemoveSecurityGroup(subnetName, networkId);
        }
        public async Task switchNsg(string securityGroupNameOld, string securityGroupNameNew, string subnetName, string networkId)
        {
            await _azure.RemoveSecurityGroup(subnetName, networkId); //Might have a time where its open. Check for way to specify which NSG to remove
            await _azure.ApplySecurityGroup(securityGroupNameNew, subnetName, networkId);
        }
        public async Task<UInt16> deleteUnused()
        {
            throw new NotImplementedException();
            //needs to check for any policies that are not currently in use by any pods/belong to deleted pods.
        }
    }
}
