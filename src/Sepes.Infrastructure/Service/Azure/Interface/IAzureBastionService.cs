namespace Sepes.Infrastructure.Service.Azure.Interface
{
    public interface IAzureBastionService : IHasProvisioningState, IHasTags, IPerformResourceProvisioning
    {
      
    }
}