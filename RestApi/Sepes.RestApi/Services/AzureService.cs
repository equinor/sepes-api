using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System;
using System.Threading.Tasks;
using Sepes.RestApi.Model;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Azure.Management.Graph.RBAC.Fluent;
using System.Linq;
using System.Collections.Immutable;
using System.Collections.Generic;

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

        public async Task<string> CreateResourceGroup(string networkName)
        {
            //Create ResourceGroup
            var resourceGroup = await _azure.ResourceGroups
                    .Define(networkName)
                    .WithRegion(Region.EuropeNorth)
                    .CreateAsync();
            return resourceGroup.Id;//return resource id from iresource objects

        }
        
        public Task TerminateResourceGroup(string commonResourceGroup)
        {
            //Wrap in try...catch? Or was that done in controller?
            return _azure.ResourceGroups.DeleteByNameAsync(commonResourceGroup); //Delete might not be what we want.
            //Might instead want to get list of all users then remove them?
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

        public async Task RemoveNetwork(string vNetName)
        {
            await _azure.Networks.DeleteByResourceGroupAsync(_commonResourceGroup, vNetName);
            return;
        }

        public async Task CreateSecurityGroup(string securityGroupName, string resourceGroupName)
        {
            await _azure.NetworkSecurityGroups
                .Define(securityGroupName)
                .WithRegion(Region.EuropeNorth)
                .WithExistingResourceGroup(resourceGroupName)
                /*.WithTag()*/
                .CreateAsync();
        }
        public async Task DeleteSecurityGroup(string securityGroupName, string resourceGroupName)
        {
            await _azure.NetworkSecurityGroups.DeleteByResourceGroupAsync(resourceGroupName, securityGroupName);
        }
        
        public async Task ApplySecurityGroup(string resourceGroupName, string securityGroupName, string subnetName, string networkName)
        {
            //Add the security group to a subnet.
            var nsg = _azure.NetworkSecurityGroups.GetByResourceGroup(resourceGroupName, securityGroupName);
            var network = _azure.Networks.GetByResourceGroup(_commonResourceGroup, networkName);
            await network.Update()
                .UpdateSubnet(subnetName)
                .WithExistingNetworkSecurityGroup(nsg)
                .Parent()
                .ApplyAsync();
        }
        
        public async Task RemoveSecurityGroup(string resourceGroupName, string subnetName, string networkName)
        {
            //Remove the security group from a subnet.
            await _azure.Networks.GetByResourceGroup(resourceGroupName, networkName)
                .Update().UpdateSubnet(subnetName).WithoutNetworkSecurityGroup().Parent().ApplyAsync();
        }
        public async Task NsgAllowOutboundPort(string securityGroupName, string resourceGroupName, string ruleName, int priority, string[] internalAddresses, int internalPort)
        {
            await _azure.NetworkSecurityGroups
                .GetByResourceGroup(resourceGroupName, securityGroupName) //can be changed to get by ID
                .Update()
                .DefineRule(ruleName)//Maybe "AllowOutgoing" + portvariable
                .AllowOutbound()
                .FromAddresses(internalAddresses)
                .FromPort(internalPort)
                .ToAnyAddress()
                .ToAnyPort()
                .WithAnyProtocol()
                .WithPriority(priority)
                .Attach()
                .ApplyAsync();
        }
        public async Task NsgAllowInboundPort(string securityGroupName, string resourceGroupName, string ruleName, int priority, string[] externalAddresses, int externalPort)
        {
            await _azure.NetworkSecurityGroups
                .GetByResourceGroup(resourceGroupName, securityGroupName) //can be changed to get by ID
                .Update()
                .DefineRule(ruleName)
                .AllowInbound()
                .FromAnyAddress()
                .FromAnyPort()
                .ToAddresses(externalAddresses)
                .ToPort(externalPort)
                .WithAnyProtocol()
                .WithPriority(priority)
                .Attach()
                .ApplyAsync();
        }

        public async Task<IEnumerable<string>> GetNSGNames(string resourceGroupName)
        {
            var nsgs = await _azure.NetworkSecurityGroups.ListByResourceGroupAsync(resourceGroupName);
            return nsgs.Select(nsg => nsg.Name);
        }

        public async Task NsgAllowPort(string securityGroupName,
                                       string resourceGroupName,
                                       string ruleName,
                                       int priority,
                                       string[] internalAddresses,
                                       int internalPort,
                                       string[] externalAddresses,
                                       int externalPort)
        {
            await _azure.NetworkSecurityGroups
                .GetByResourceGroup(resourceGroupName, securityGroupName) //can be changed to get by ID
                .Update()
                .DefineRule(ruleName)
                .AllowInbound()
                .FromAddresses(internalAddresses)
                .FromPort(internalPort)
                .ToAddresses(externalAddresses)
                .ToPort(externalPort)
                .WithAnyProtocol()
                .WithPriority(priority)
                .Attach()
                .ApplyAsync();
        }

        // ApplyDataset(...)
        // Don't need a remove dataset as that happes when resource group gets terminated.


        //// Pod user/role management
        // Gives a user contributor to a resource group and network join on a network
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
