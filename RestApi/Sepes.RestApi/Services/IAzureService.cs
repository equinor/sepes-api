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
        Task ApplySecurityGroup(string securityGroupName, string subnetName, string networkId);
        Task RemoveSecurityGroup(string securityGroupName, string subnetName, string networkId);
    }
}
