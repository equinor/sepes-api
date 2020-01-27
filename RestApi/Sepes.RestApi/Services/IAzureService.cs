using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.RestApi.Services
{
    public interface IAzureService
    {
        /// <summary>
        /// Creates a new resource group for a new pod
        /// </summary>
        Task<string> CreateResourceGroup(string podName);

        Task TerminateResourceGroup(string resourceGroupName);

        /// <summary>
        /// Creates a new network within a common resource group where all security groups and networks are stored.
        /// </summary>
        Task<string> CreateNetwork(string networkName, string addressSpace, string subnetName);

        /// <summary>
        /// Creates a new network security group within a common resource group where all security groups and networks are stored.
        /// </summary>
        Task CreateSecurityGroup(string securityGroupName);

        /// <summary>
        /// Deletes a network security group from Azure.
        /// </summary>
        Task DeleteSecurityGroup(string securityGroupName);

        /// <summary>
        /// Assigns an existing network security group to a subnet.
        /// </summary>
        Task ApplySecurityGroup(string securityGroupName, string subnetName, string networkName);

        /// <summary>
        /// Removes the association between the current network security group and a given subnet and network.
        /// </summary>
        Task RemoveSecurityGroup(string subnetName, string networkName);

        /// <summary>
        /// Allow traffic outbound, to a specified port and a list of ip adresses.
        /// </summary>
        Task NsgAllowOutboundPort(string securityGroupName, string ruleName, int priority, string[] internalAddresses, int internalPort);

        /// <summary>
        /// Allow traffic inbound, to a specified port from a list of ip adresses.
        /// </summary>
        Task NsgAllowInboundPort(string securityGroupName, string ruleName, int priority, string[] externalAddresses, int externalPort);
        
        /// <summary>
        /// Gives a user access to a resource group by giving the user a built in contributer role.
        /// </summary>
        /// <returns>The user's id</returns>
        Task<string> AddUserToResourceGroup(string userId, string resourceGroupName);

        /// <summary>
        /// Gives a user access to a network by giving the user a custom join network role.
        /// </summary>
        /// <returns>The user's id</returns>
        Task<string> AddUserToNetwork(string userId, string networkName);

        /// <summary>
        /// Get a list of names of Network Security Groups from a common resource group for NSGs and networks.
        /// </summary>
        Task<IEnumerable<string>> GetNSGNames();
    }
}
