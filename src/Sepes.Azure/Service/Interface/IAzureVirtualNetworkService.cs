using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure.Interface
{
    public interface IAzureVirtualNetworkService : IHasProvisioningState, IHasTags, IPerformResourceProvisioning
    {       
        Task EnsureSandboxSubnetHasServiceEndpointForStorage(string resourceGroupName, string networkName);
    }
}