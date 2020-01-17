using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.RestApi.Services
{
    public interface IAzureService
    {
        Task<string> CreateResourceGroup(string podName);
        Task DeleteResourceGroup(string resourceGroupName);
        Task TerminateResourceGroup(string resourceGroupName);
        Task<string> CreateNetwork(string networkName, string addressSpace, string subnetName);
        Task CreateSecurityGroup(string securityGroupName);
        Task DeleteSecurityGroup(string securityGroupName);
        Task ApplySecurityGroup(string securityGroupName, string subnetName, string networkName);
        Task RemoveSecurityGroup(string subnetName, string networkName);

        Task NsgAllowOutboundPort(string securityGroupName, string ruleName, int priority, string[] internalAddresses, int internalPort);
        Task NsgAllowInboundPort(string securityGroupName, string ruleName, int priority, string[] externalAddresses, int externalPort);
        
        Task<string> AddUserToResourceGroup(string userId, string resourceGroupName);
        Task<string> AddUserToNetwork(string userId, string networkName);
        Task<IEnumerable<string>> GetNSGNames();
    }
}
