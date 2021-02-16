using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure.Interface
{
    public interface IAzureResourceGroupService : IHasProvisioningState, IHasTags, IPerformResourceProvisioning
    { 
        Task Delete(string resourceGroupName, CancellationToken cancellationToken = default);
       
        Task<string> GetProvisioningState(string resourceGroupName);   
    }
}
