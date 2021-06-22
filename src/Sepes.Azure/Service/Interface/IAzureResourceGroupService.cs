using Sepes.Common.Interface.Service;

namespace Sepes.Azure.Service.Interface
{
    public interface IAzureResourceGroupService : IHasProvisioningState, IServiceForTaggedResource, IPerformResourceProvisioning
    { 
    
    }
}
