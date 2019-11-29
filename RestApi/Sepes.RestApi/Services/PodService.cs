
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sepes.RestApi.Model;

namespace Sepes.RestApi.Services
{
    // This service manage changes to Pods. Make sure they are validated and the correct azure actions are performed.
    // It do not own the Pod state and need to talk to StudyService to keep its up to date.
    public interface IPodService
    {
        Task<Pod> CreateNewPod(string name, int userID);
        Task<string> GetPods(int studyID);
        Task Set(Pod newPod, Pod based);
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

// old
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
        public Task<UInt16> deleteUnused()
        {
            throw new NotImplementedException();
            //needs to check for any policies that are not currently in use by any pods/belong to deleted pods.
        }
        public async Task<string> GetPods(int studyID)
        {
            return await _database.getPods(studyID);
        }

// new
        public async Task Set(Pod newPod, Pod based)
        {
            if (based == null)
            {
                await _azure.CreateResourceGroup(newPod.resourceGroupName);
                await _azure.CreateNetwork(newPod.networkName, newPod.addressSpace);
                await _azure.CreateSecurityGroup(newPod.networkSecurityGroupName, newPod.resourceGroupName);
            }
            // pod.allowAll != basePod.allowAll // Apply allow all

            if (!based.incoming.SequenceEqual(newPod.incoming))
            {
                var inbound = new Dictionary<ushort, string[]>();

                foreach (var rule in newPod.incoming) {
                    inbound[rule.port].Append(rule.ip);
                }

                foreach (var port in inbound.Keys) {
                    // TASK manage securitygroup, rulename and prority
                    Task addInbound = _azure.NsgAllowInboundPort(newPod.networkSecurityGroupName, newPod.resourceGroupName, "ruleName", 100, inbound[port], (int) port);
                    addInbound.Start();
                }
            }
            if (!based.outgoing.SequenceEqual(newPod.outgoing))
            {
                // Apply rules
            }

            List<Task> addUsersTasks = new List<Task>();
            foreach (var user in newPod.users)
            {
                if (!based.users.Contains(user))
                {
                    Task addUserToResGroup = _azure.AddUserToResourceGroup(user.userEmail, newPod.resourceGroupName);
                    Task addUserToNetwork = _azure.AddUserToNetwork(user.userEmail, newPod.networkName);
                    addUsersTasks.Add(addUserToResGroup);
                    addUsersTasks.Add(addUserToNetwork);
                    addUserToResGroup.Start();
                    addUserToNetwork.Start();
                }
            }
            Task.WaitAll(addUsersTasks.ToArray());
        }

    }
}
