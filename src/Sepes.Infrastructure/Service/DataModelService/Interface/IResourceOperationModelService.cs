using Sepes.Infrastructure.Model;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService.Interface
{
    public interface IResourceOperationModelService
    {      
        Task<CloudResourceOperation> GetForOperationPromotion(int id);
        Task<CloudResourceOperation> EnsureReadyForRetry(CloudResourceOperation operationToRetry);
    }
}