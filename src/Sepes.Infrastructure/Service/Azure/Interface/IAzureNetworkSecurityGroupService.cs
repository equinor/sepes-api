using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure.Interface
{
    public interface IAzureNetworkSecurityGroupService : IHasProvisioningState, IHasTags, IPerformResourceProvisioning
    {     
        Task<INetworkSecurityGroup> Create(Region region, string resourceGroupName, string nsgName, Dictionary<string, string> tags, CancellationToken cancellationToken = default);
        
        Task Delete(string resourceGroupName, string securityGroupName);    
     
    }
}