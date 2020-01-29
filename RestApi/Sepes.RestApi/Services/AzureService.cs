using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System;
using System.Threading.Tasks;
using Sepes.RestApi.Model;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Azure.Management.Graph.RBAC.Fluent;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Azure.Management.Network.Fluent;
using System.Threading;

namespace Sepes.RestApi.Services
{
    // Wraps call to azure. This service will most likely need to be split up into smaller servies.
    // This is (and future children) is the only code that is alloed to create and destoy azure resources.
    [ExcludeFromCodeCoverage]
    public class AzureService : IAzureService
    {
        private readonly IAzure _azure;
        private readonly string _commonResourceGroup;
        private readonly string _joinNetworkRoleName;

        public AzureService(AzureConfig config)
        {
            _commonResourceGroup = config.commonGroup;
            _azure = Azure.Authenticate(config.credentials).WithDefaultSubscription();
            _joinNetworkRoleName = "ExampleJoinNetwork";

            if (!_azure.ResourceGroups.Contain(_commonResourceGroup))
            {
                _azure.ResourceGroups
                    .Define(_commonResourceGroup)
                    .WithRegion(Region.EuropeNorth)
                    .Create();
            }
        }

        public async Task<string> CreateResourceGroup(string resourceGroupName)
        {
            //Create ResourceGroup
            var resourceGroup = await _azure.ResourceGroups
                    .Define(resourceGroupName)
                    .WithRegion(Region.EuropeNorth)
                    .CreateAsync();

            //return resource id from iresource objects
            return resourceGroup.Id;
        }

        ///<summary>
        ///Delete a Resource Group from Azure. WARNING this will also delete all resources contained within that Pod.
        ///</summary>
        ///<param name="resourceGroupName">Name of the Resource Group to delete.</param>
        public async Task DeleteResourceGroup(string resourceGroupName)
        {
            //Cancelation token can be saved so the azure delete can be aborted. But has not been done in this use case.
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;

            await _azure.ResourceGroups.BeginDeleteByNameAsync(resourceGroupName, token);
        }

        public async Task<string> CreateNetwork(string networkName, string addressSpace, string subnetName)
        {
            var network = await _azure.Networks.Define(networkName)
                .WithRegion(Region.EuropeNorth)
                .WithExistingResourceGroup(_commonResourceGroup)
                .WithAddressSpace(addressSpace).WithSubnet(subnetName, addressSpace)
                .CreateAsync();

            return network.Id;
        }

        ///<summary>
        ///Delete a Network resource from Azure.
        ///</summary>
        ///<param name="vNetName">Name of the Network to delete.</param>
        public async Task DeleteNetwork(string vNetName)
        {
            await _azure.Networks.DeleteByResourceGroupAsync(_commonResourceGroup, vNetName);
            return;
        }

        ///<summary>
        ///Create a new Network Security Group resource on Azure and add the Base Rules so that Azure load balancer does not interfere with the Pods
        ///</summary>
        ///<param name="securityGroupName">Unique name of the Network Security Group to create.</param>
        public async Task CreateSecurityGroup(string securityGroupName)
        {
            var nsg = await _azure.NetworkSecurityGroups
                .Define(securityGroupName)
                .WithRegion(Region.EuropeNorth)
                .WithExistingResourceGroup(_commonResourceGroup)
                /*.WithTag()*/
                .CreateAsync();

            //Add rules obligatory to every pod. This will block AzureLoadBalancer from talking to the VMs inside sandbox
            await this.NsgApplyBaseRules(nsg);
        }

        ///<summary>
        ///Delete a Network Security Group from Azure. It is required that the NSG is not tied to any subnet before performing this task.
        ///</summary>
        ///<param name="securityGroupName">Name of the Network Security Group to delete.</param>
        public async Task DeleteSecurityGroup(string securityGroupName)
        {
            await _azure.NetworkSecurityGroups.DeleteByResourceGroupAsync(_commonResourceGroup, securityGroupName);
        }

        ///<summary>
        ///Add an existing Netork Security Group to a subnet
        ///</summary>
        ///<param name="securityGroupName">Name of the Network Security Group to add.</param>
        ///<param name="subnetName">Name of the subnet to modify</param>
        ///<param name="networkName">Name of the network the subnet belongs to.</param>
        public async Task ApplySecurityGroup(string securityGroupName, string subnetName, string networkName)
        {
            //Add the security group to a subnet.
            var nsg = _azure.NetworkSecurityGroups.GetByResourceGroup(_commonResourceGroup, securityGroupName);
            var network = _azure.Networks.GetByResourceGroup(_commonResourceGroup, networkName);
            await network.Update()
                .UpdateSubnet(subnetName)
                .WithExistingNetworkSecurityGroup(nsg)
                .Parent()
                .ApplyAsync();
        }

        ///<summary>
        ///Remove a Network Security Group from a subnet
        ///</summary>
        ///<param name="subnetName">Name of the subnet to modify</param>
        ///<param name="networkName">Name of the network the subnet belongs to.</param>
        public async Task RemoveSecurityGroup(string subnetName, string networkName)
        {
            //Remove the security group from a subnet.
            await _azure.Networks.GetByResourceGroup(_commonResourceGroup, networkName)
                .Update().UpdateSubnet(subnetName).WithoutNetworkSecurityGroup().Parent().ApplyAsync();
        }

        ///<summary>
        ///Adds rules to an Nsg. This lets an array of internal addresses and a specified external port talk to any external address and internal port.
        ///</summary>
        ///<param name="securityGroupname">Name of the Network Security Group to modify</param>
        ///<param name="ruleName">Gives the rules a name. Does not have to be unique</param>
        ///<param name="priority">Specifies the priority of the rule. Each rule must have a unique priority. Check azure for detailed explanation.</param>
        ///<param name="internalAddresses">An array of internal addresses</param>
        ///<param name="toPort">The External port to open</param>
        public async Task NsgAllowInboundPort(string securityGroupName,
                                              string ruleName,
                                              int priority,
                                              string[] internalAddresses,
                                              int toPort)
        {
            await _azure.NetworkSecurityGroups
                .GetByResourceGroup(_commonResourceGroup, securityGroupName) //can be changed to get by ID
                .Update()
                .DefineRule(ruleName)//Maybe "AllowOutgoing" + portvariable
                .AllowInbound()
                .FromAddresses(internalAddresses)
                .FromAnyPort()
                .ToAnyAddress()
                .ToPort(toPort)
                .WithAnyProtocol()
                .WithPriority(priority)
                .Attach()
                .ApplyAsync();
        }

        ///<summary>
        ///Adds rules to an Nsg. This lets an array of external addresses and a specified port talk to any internal address and port combo.
        ///</summary>
        ///<param name="securityGroupname">Name of the Network Security Group to modify</param>
        ///<param name="ruleName">Gives the rules a name. Does not have to be unique</param>
        ///<param name="priority">Specifies the priority of the rule. Each rule must have a unique priority. Check azure for detailed explanation.</param>
        ///<param name="externalAddresses">An array of external addresses</param>
        ///<param name="toPort">The External port to open</param>
        public async Task NsgAllowOutboundPort(string securityGroupName,
                                               string ruleName,
                                               int priority,
                                               string[] externalAddresses,
                                               int toPort)
        {
            await _azure.NetworkSecurityGroups
                .GetByResourceGroup(_commonResourceGroup, securityGroupName) //can be changed to get by ID
                .Update()
                .DefineRule(ruleName)
                .AllowOutbound()
                .FromAnyAddress()
                .FromAnyPort()
                .ToAddresses(externalAddresses)
                .ToPort(toPort)
                .WithAnyProtocol()
                .WithPriority(priority)
                .Attach()
                .ApplyAsync();
        }

        ///<summary>
        ///NsgApplyBaseRules readd the default rules added by Azure but without the tunnel for Azures load balancer.
        ///</summary>
        public async Task NsgApplyBaseRules(INetworkSecurityGroup nsg)
        {
            await nsg.Update()
            .DefineRule("DenyInbound")
            .DenyInbound()
            .FromAnyAddress()
            .FromAnyPort()
            .ToAnyAddress()
            .ToAnyPort()
            .WithAnyProtocol()
            .WithPriority(4050)
            .Attach()

            .DefineRule("AllowVnetInBound2")
            .AllowInbound()
            .FromAddress("VirtualNetwork")
            .FromAnyPort()
            .ToAddress("VirtualNetwork")
            .ToAnyPort()
            .WithAnyProtocol()
            .WithPriority(4000)
            .Attach()

            .DefineRule("DenyOutbound")
            .DenyOutbound()
            .FromAnyAddress()
            .FromAnyPort()
            .ToAnyAddress()
            .ToAnyPort()
            .WithAnyProtocol()
            .WithPriority(4050)
            .Attach()

            .DefineRule("AllowVnetoutBound2")
            .AllowOutbound()
            .FromAddress("VirtualNetwork")
            .FromAnyPort()
            .ToAddress("VirtualNetwork")
            .ToAnyPort()
            .WithAnyProtocol()
            .WithPriority(4000)
            .Attach()
            .ApplyAsync();
        }

        ///<summary>
        ///Returns a string with the existing Network Security Groups
        ///</summary>
        public async Task<IEnumerable<string>> GetNSGNames()
        {
            var nsgs = await _azure.NetworkSecurityGroups.ListByResourceGroupAsync(_commonResourceGroup);
            return nsgs.Select(nsg => nsg.Name);
        }

        ///<summary>
        ///Gives a user contributor to a resource group
        ///</summary>
        ///<param name="userId">A GUID string unique to the user</param>
        ///<param name="resourceGroupName">The name of the resource group to add the user to</param>
        public async Task<string> AddUserToResourceGroup(string userId, string resourceGroupName)
        {
            var resourceGroup = await _azure.ResourceGroups.GetByNameAsync(resourceGroupName);

            return _azure.AccessManagement.RoleAssignments
                .Define(Guid.NewGuid().ToString())
                .ForObjectId(userId)
                .WithBuiltInRole(BuiltInRole.Contributor)
                .WithResourceScope(resourceGroup)
                .CreateAsync().Result.Id;
        }

        ///<summary>
        ///Gives a user network join on a network
        ///</summary>
        ///<param name="userId">A GUID string unique to the user</param>
        ///<param name="resourceGroupName">The name of the network to add the user to</param>
        public async Task<string> AddUserToNetwork(string userId, string networkName)
        {
            var network = await _azure.Networks.GetByResourceGroupAsync(_commonResourceGroup, networkName);
            string joinNetworkRoleId = _azure.AccessManagement.RoleDefinitions
                .GetByScopeAndRoleNameAsync(network.Id, _joinNetworkRoleName).Result.Id;

            return _azure.AccessManagement.RoleAssignments
                .Define(Guid.NewGuid().ToString())
                .ForObjectId(userId)
                .WithRoleDefinition(joinNetworkRoleId)
                .WithResourceScope(network)
                .CreateAsync().Result.Id;
        }

    }
}
