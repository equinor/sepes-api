using Sepes.RestApi.Model;
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
        Task ApplySecurityGroup(string securityGroupName, string subnetName, string networkName);
        Task RemoveSecurityGroup(string resourceGroupName, string subnetName, string networkName);
        Task NsgAllowInboundPort(string securityGroupName, string resourceGroupName, string ruleName, int priority, string[] externalAddresses, int externalPort);
        Task NsgAllowOutboundPort(string securityGroupName, string resourceGroupName, string ruleName, int priority, string[] internalAddresses, int internalPort);
        Task NsgAllowPort(string securityGroupName, string resourceGroupName, string ruleName, int priority, string[] internalAddresses, int internalPort, string[] externalAddresses, int externalPort);
    }
}
