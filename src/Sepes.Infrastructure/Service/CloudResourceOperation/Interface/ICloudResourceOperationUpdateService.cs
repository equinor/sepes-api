using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface ICloudResourceOperationUpdateService
    {
        Task<CloudResourceOperationDto> ReInitiateAsync(int id);

        Task<CloudResourceOperationDto> UpdateStatusAsync(int id, string status, string updatedProvisioningState = null, string errorMessage = null);
       
        Task<CloudResourceOperationDto> SetInProgressAsync(int id, string requestId);          

        Task<List<CloudResourceOperation>> AbortAllUnfinishedCreateOrUpdateOperationsAsync(int resourceId);
        Task<CloudResourceOperationDto> SetQueueInformationAsync(int id, string messageId, string popReceipt, DateTimeOffset nextVisibleOn);

        Task<CloudResourceOperationDto> ClearQueueInformationAsync(int id);
    }
}