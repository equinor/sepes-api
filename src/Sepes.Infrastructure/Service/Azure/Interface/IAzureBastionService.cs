using Microsoft.Azure.Management.Network.Models;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure.Interface
{
    public interface IAzureBastionService : IHasProvisioningState, IHasTags, IPerformResourceProvisioning
    {      
        Task<BastionHost> Create(Region region, string resourceGroupName, string bastionName, string subnetId, Dictionary<string, string> tags, CancellationToken cancellationToken = default(CancellationToken));
        Task Delete(string resourceGroupName, string bastionHostName);
    }
}