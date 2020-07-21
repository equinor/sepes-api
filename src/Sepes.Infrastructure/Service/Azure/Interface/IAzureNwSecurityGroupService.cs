using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public interface IAzureNwSecurityGroupService
    {
       
        Task<INetworkSecurityGroup> CreateSecurityGroup(Region region, string resourceGroupName, string nsgName, Dictionary<string, string> tags);
        Task<INetworkSecurityGroup> CreateSecurityGroupForSubnet(Region region, string resourceGroupName, string sandboxName, Dictionary<string, string> tags);
        Task DeleteSecurityGroup(string resourceGroupName, string securityGroupName);
        Task<bool> Exists(string resourceGroupName, string nsgName);

        Task<string> GetProvisioningState(string resourceGroupName, string resourceName);
    }
}