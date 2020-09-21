using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Sepes.Infrastructure.Service.Azure.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public interface IAzureNetworkSecurityGroupService : IHasProvisioningState, IHasTags, IPerformCloudResourceCRUD
    {
       
        Task<INetworkSecurityGroup> CreateSecurityGroup(Region region, string resourceGroupName, string nsgName, Dictionary<string, string> tags);
        //Task<INetworkSecurityGroup> CreateSecurityGroupForSubnet(Region region, string resourceGroupName, string sandboxName, Dictionary<string, string> tags);
        Task DeleteSecurityGroup(string resourceGroupName, string securityGroupName);       
    }
}