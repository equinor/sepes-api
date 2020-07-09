using Microsoft.Azure.Management.Network.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Sepes.Infrastructure.Dto;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public interface IAzureVNetService
    {
        Task<AzureVNetDto> Create(Region region, string resourceGroupName, string studyName, string sandboxName);

        //Task<string> Status(string resourceGroup, string vNetName);
        Task ApplySecurityGroup(string resourceGroupName, string securityGroupName, string subnetName, string networkName);
        Task Delete(string resourceGroup, string vNetName);
    }
}