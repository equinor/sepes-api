using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface ICloudResourceOperationUpdateService
    {
        Task<CloudResourceOperationDto> ReInitiateAsync(int id);

        Task<CloudResourceOperationDto> UpdateStatusAsync(int id, string status, string updatedProvisioningState = null, string errorMessage = null);
       
        Task<CloudResourceOperationDto> SetInProgressAsync(int id, string requestId);          

        Task<List<CloudResourceOperation>> AbortAllUnfinishedCreateOrUpdateOperations(int resourceId);      
    }
}