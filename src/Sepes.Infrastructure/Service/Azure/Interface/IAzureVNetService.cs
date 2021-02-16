using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Sepes.Infrastructure.Dto;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure.Interface
{
    public interface IAzureVNetService : IHasProvisioningState, IHasTags, IPerformResourceProvisioning
    {
        Task<AzureVNetDto> CreateAsync(Region region, string resourceGroupName, string networkName, string sanboxSubnetName, Dictionary<string, string> tags, CancellationToken cancellationToken = default(CancellationToken));
      
        Task ApplySecurityGroup(string resourceGroupName, string securityGroupName, string subnetName, string networkName);
        Task EnsureSandboxSubnetHasServiceEndpointForStorage(string resourceGroupName, string networkName);
    }
}