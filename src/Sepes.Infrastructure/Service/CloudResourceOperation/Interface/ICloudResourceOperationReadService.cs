using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface ICloudResourceOperationReadService
    {      
        Task<CloudResourceOperationDto> GetByIdAsync(int id);  
        
        Task<bool> ExistsPreceedingUnfinishedOperationsAsync(CloudResourceOperationDto operationDto);

        Task<List<CloudResourceOperation>> GetUnfinishedOperations(int resourceId);

        Task<CloudResourceOperation> GetUnfinishedDeleteOperation(int resourceId);      

        Task<bool> OperationIsFinishedAndSucceededAsync(int operationId);
        Task<bool> HasUnstartedCreateOrUpdateOperation(int resourceId);
       
    }
}