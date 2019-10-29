
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
        Task addNsg(string securityGroupName, string subnetName, string networkId);
        Task switchNsg(string securityGroupNameOld, string securityGroupNameNew, string subnetName, string networkId);
        Task removeNsg(string securityGroupName, string subnetName, string networkId);
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
        public async Task addNsg(string securityGroupName, string subnetName, string networkId)
        {
            //throw new NotImplementedException();
            await _azure.ApplySecurityGroup(securityGroupName, subnetName, networkId);

        }
        public async Task removeNsg(string securityGroupName, string subnetName, string networkId)
        {
            await _azure.RemoveSecurityGroup(securityGroupName, subnetName, networkId);
        }
        public async Task switchNsg(string securityGroupNameOld, string securityGroupNameNew, string subnetName, string networkId)
        {
            await _azure.ApplySecurityGroup(securityGroupNameNew, subnetName, networkId);
            await _azure.RemoveSecurityGroup(securityGroupNameOld, subnetName, networkId);
        }
    }
}