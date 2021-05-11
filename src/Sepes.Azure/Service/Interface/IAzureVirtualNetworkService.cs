using Sepes.Common.Interface.Service;
using System.Threading.Tasks;

namespace Sepes.Azure.Service.Interface
{
    public interface IAzureVirtualNetworkService : IHasProvisioningState, IHasTags, IPerformResourceProvisioning
    {       
        Task EnsureSandboxSubnetHasServiceEndpointForStorage(string resourceGroupName, string networkName);
    }
}