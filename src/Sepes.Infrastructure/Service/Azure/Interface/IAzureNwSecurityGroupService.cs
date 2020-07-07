using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public interface IAzureNwSecurityGroupService
    {
       
        Task<INetworkSecurityGroup> CreateSecurityGroup(Region region, string resourceGroupName, string nsgName);
        Task<INetworkSecurityGroup> CreateSecurityGroupForSubnet(Region region, string resourceGroupName, string sandboxName);
        Task DeleteSecurityGroup(string resourceGroupName, string securityGroupName);
    }
}