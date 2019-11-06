using Microsoft.Azure.Management.Graph.RBAC.Fluent;
using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
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
        Task ApplySecurityGroup(string resourceGroupName, string securityGroupName, string subnetName, string networkName);
        Task RemoveSecurityGroup(string resourceGroupName, string subnetName, string networkName);

        Task<IRoleAssignment> AddUserToResourceGroup(string userId, string resourceGroupName);
        Task<IRoleAssignment> AddUserToNetwork(string userId, string joinNetworkRoleId, string networkId);
        Task RemoveUserFromResource(string userId, string resScope);
    }
}
