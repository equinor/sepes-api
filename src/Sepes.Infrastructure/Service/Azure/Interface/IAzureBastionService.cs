using Microsoft.Azure.Management.Network.Models;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure.Interface
{
    public interface IAzureBastionService : IHasProvisioningState, IHasTags, IPerformCloudResourceCRUD
    {      
        Task<BastionHost> Create(Region region, string resourceGroupName, string studyName, string sandboxName, string subnetId, Dictionary<string, string> tags);
        Task Delete(string resourceGroupName, string bastionHostName);
    }
}