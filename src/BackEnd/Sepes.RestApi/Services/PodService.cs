using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sepes.Infrastructure.Dto;

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
        Task Set(PodDto updatedPod, PodDto basePod, IEnumerable<UserDto> updatedUsers, IEnumerable<UserDto> baseUsers);
    }

    public class PodService : IPodService
    {
        private readonly ISepesDb _database;
        private readonly IAzureService _azure;


        public PodService(IAzureService azure)
        {
            _azure = azure;
        }
        
        public async Task Set(PodDto newPod, PodDto based, IEnumerable<UserDto> newUsers, IEnumerable<UserDto> basedUsers)
        {
            if (newPod == null)
            {
                await this.Delete(based);
                return;
            }
            else if (based == null)
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

        public async Task Delete(PodDto pod)
        {
            //Remove nsg assignment so deletion can be done without error
            await _azure.DeleteNetwork(pod.networkName);
            await _azure.DeleteResourceGroup(pod.resourceGroupName);
            var (nsg1, nsg2) = GetNSGNameVariants(pod.networkSecurityGroupName);

            await _azure.DeleteSecurityGroup(nsg1);
            await _azure.DeleteSecurityGroup(nsg2);
        }

        private async Task AddUsers(PodDto newPod, IEnumerable<UserDto> newUsers, IEnumerable<UserDto> basedUsers)
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

        private async Task ManageNetworkSecurityGroup(PodDto newPod)
        {
            var (nsgNameDefault, nsgNameDef2) = GetNSGNameVariants(newPod.networkSecurityGroupName);

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

        private async Task RemoveNetworkSecurityGroup(PodDto newPod)
        {
            await _azure.RemoveSecurityGroup(newPod.subnetName, newPod.networkName);
        }

        private Dictionary<ushort, string[]> GenerateRuleDictionary(IEnumerable<RuleDto> array)
        {
            var g = array.GroupBy(r => r.port, r => r.ip, (port, ips) => new { Key = port, Value = ips });
            var ruleDict = g.ToDictionary(r => r.Key, r => r.Value.ToArray());
            return ruleDict;
        }

        private (string default1, string default2) GetNSGNameVariants(string networkSecurityGroupName)
        {
            return (networkSecurityGroupName, networkSecurityGroupName+"2");
        }
    }
}
