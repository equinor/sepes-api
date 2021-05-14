using Sepes.Common.Interface.Service;

namespace Sepes.Azure.Service.Interface
{
    public interface IAzureBastionService : IHasProvisioningState, IHasTags, IPerformResourceProvisioning
    {
      
    }
}