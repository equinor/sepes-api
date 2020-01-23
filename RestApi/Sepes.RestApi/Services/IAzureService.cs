using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.RestApi.Services
{
    public interface IAzureService
    {
        Task<string> CreateResourceGroup(string podName);
        Task TerminateResourceGroup(string resourceGroupName);
        Task<string> CreateNetwork(string networkName, string addressSpace, string subnetName);
        Task CreateSecurityGroup(string securityGroupName);
        Task DeleteSecurityGroup(string securityGroupName);
        Task ApplySecurityGroup(string securityGroupName, string subnetName, string networkName);
        Task RemoveSecurityGroup(string subnetName, string networkName);

        Task NsgAllowOutboundPort(string securityGroupName, string ruleName, int priority, string[] internalAddresses, int internalPort);
        Task NsgAllowInboundPort(string securityGroupName, string ruleName, int priority, string[] externalAddresses, int externalPort);
        
        /// <summary>
        /// Gives a user access to a resource group by giving the user a built in contributer role
        /// </summary>
        /// <returns>The user's id</returns>
        Task<string> AddUserToResourceGroup(string userId, string resourceGroupName);

        /// <summary>
        /// Gives a user access to a network by giving the user a custom join network role
        /// </summary>
        /// <returns>The user's id</returns>
        Task<string> AddUserToNetwork(string userId, string networkName);

        /// <summary>
        /// Get a list of names of Network Security Groups from a common resource group for NSGs and networks
        /// </summary>
        Task<IEnumerable<string>> GetNSGNames();
    }
}
