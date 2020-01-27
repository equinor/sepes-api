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
        /// <summary>
        /// Creates or updates a pod based on the differences between a base base pod serving as current state, and 
        /// a new pod object becoming the updated state. Also adds users to resources based on the diffence between 
        /// the current list of users and an updated list of users.
        /// </summary>
        /// <param name="updatedPod">The new or updated pod</param>
        /// <param name="basePod">The current pod</param>
        /// <param name="updatedUsers">The updated list of users</param>
        /// <param name="baseUsers">The current list of users</param>
        Task Set(Pod updatedPod, Pod basePod, IEnumerable<User> updatedUsers, IEnumerable<User> baseUsers);
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

        public async Task Set(Pod newPod, Pod based, IEnumerable<User> newUsers, IEnumerable<User> basedUsers)
        {
            if (based == null)
            {
                Task createRes = _azure.CreateResourceGroup(newPod.resourceGroupName);
                Task createNet = _azure.CreateNetwork(newPod.networkName, newPod.addressSpace, newPod.subnetName);
                await Task.WhenAll(createRes, createNet);
            }

            var tasks = new List<Task>();

            // Create network security group and add rules, apply to subnet
            bool newRulesAreAdded = based == null || 
                !based.incoming.SequenceEqual(newPod.incoming) ||
                !based.outgoing.SequenceEqual(newPod.outgoing);
            if (!newPod.allowAll && (newRulesAreAdded || based.allowAll)) tasks.Add(ManageNetworkSecurityGroup(newPod));
            // Removes nsg from subnet
            if (newPod.allowAll && based != null && !based.allowAll) tasks.Add(RemoveNetworkSecurityGroup(newPod));

            
            // Add users to resources
            tasks.Add(AddUsers(newPod, newUsers, basedUsers));

            await Task.WhenAll(tasks);
        }

        private async Task AddUsers(Pod newPod, IEnumerable<User> newUsers, IEnumerable<User> basedUsers)
        {
            List<Task> addUsersTasks = new List<Task>();
            foreach (var user in newUsers)
            {
                if (basedUsers == null || !basedUsers.Contains(user))
                {
                    Task addUserToResGroup = _azure.AddUserToResourceGroup(user.userEmail, newPod.resourceGroupName);
                    Task addUserToNetwork = _azure.AddUserToNetwork(user.userEmail, newPod.networkName);
                    addUsersTasks.Add(addUserToResGroup);
                    addUsersTasks.Add(addUserToNetwork);
                }
            }
            await Task.WhenAll(addUsersTasks.ToArray());
        }

        private async Task ManageNetworkSecurityGroup(Pod newPod)
        {
            string nsgNameDefault = newPod.networkSecurityGroupName;
            string nsgNameDef2 = nsgNameDefault+"2";

            // Generate network security network name
            var nsgNames = await _azure.GetNSGNames();
            string nsgName = nsgNameDefault;
            if (nsgNames.Contains(newPod.networkSecurityGroupName)) nsgName = nsgNameDef2;
            
            // Create nsg with generated nsg name
            await _azure.CreateSecurityGroup(nsgName);

            // Set inbound and outbound rules
            Dictionary<ushort, string[]> inbound = GenerateRuleDictionary(newPod.incoming);
            int inPriority = 100;
            foreach (var port in inbound.Keys) {
                await _azure.NsgAllowInboundPort(nsgName, "in_" + port, inPriority++, inbound[port], (int) port);
            }
        
            Dictionary<ushort, string[]> outbound = GenerateRuleDictionary(newPod.outgoing);
            int outPriority = 100;
            foreach (var port in outbound.Keys) {
                await _azure.NsgAllowOutboundPort(nsgName, "out_" + port, outPriority++, outbound[port], (int) port);
            }


            // Apply network security group to subnet
            await _azure.ApplySecurityGroup(nsgName, newPod.subnetName, newPod.networkName);

            // Delete old nsg
            if (nsgName == nsgNameDefault && nsgNames.Contains(nsgNameDef2)) {
                await _azure.DeleteSecurityGroup(nsgNameDef2);
            }
            else if (nsgName == nsgNameDef2 && nsgNames.Contains(nsgNameDefault)) {
                await _azure.DeleteSecurityGroup(nsgNameDefault);
            }
        }

        private async Task RemoveNetworkSecurityGroup(Pod newPod)
        {
            await _azure.RemoveSecurityGroup(newPod.subnetName, newPod.networkName);
        }

        private Dictionary<ushort, string[]> GenerateRuleDictionary(IEnumerable<Rule> array)
        {
            var g = array.GroupBy(r => r.port, r => r.ip, (port, ips) => new { Key = port, Value = ips});
            var ruleDict = g.ToDictionary(r => r.Key, r => r.Value.ToArray());
            return ruleDict;
        }
    }
}
