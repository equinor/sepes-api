using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.RestApi.Services
{
    public interface IAzureService
    {
        Task<string> CreateResourceGroup(string podName);
        Task TerminateResourceGroup(string resourceGroupName);
        Task<string> CreateNetwork(string networkName, string addressSpace);
        Task CreateSecurityGroup(string securityGroupName, string resourceGroupName);
        Task DeleteSecurityGroup(string securityGroupName, string resourceGroupName);
        Task ApplySecurityGroup(string resourceGroupName, string securityGroupName, string subnetName, string networkName);
        Task RemoveSecurityGroup(string resourceGroupName, string subnetName, string networkName);

        Task NsgAllowOutboundPort(string securityGroupName, string resourceGroupName, string ruleName, int priority, string[] internalAddresses, int internalPort);
        Task NsgAllowInboundPort(string securityGroupName, string resourceGroupName, string ruleName, int priority, string[] externalAddresses, int externalPort);
        
        Task<string> AddUserToResourceGroup(string userId, string resourceGroupName);
        Task<string> AddUserToNetwork(string userId, string networkName);
        IEnumerable<string> GetNSGNames(string resourceGroupName);
    }
}
