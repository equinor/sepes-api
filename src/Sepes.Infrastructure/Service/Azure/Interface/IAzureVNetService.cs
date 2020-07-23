using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Sepes.Infrastructure.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public interface IAzureVNetService : IHasProvisioningState, IHasTags
    {
        Task<AzureVNetDto> CreateAsync(Region region, string resourceGroupName, string studyName, string sandboxName, Dictionary<string, string> tags);
      
        Task Delete(string resourceGroupName, string vNetName);
        Task<bool> Exists(string resourceGroupName, string networkName);   
        Task ApplySecurityGroup(string resourceGroupName, string securityGroupName, string subnetName, string networkName);
    }
}