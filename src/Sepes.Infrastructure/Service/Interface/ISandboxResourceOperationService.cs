using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface ISandboxResourceOperationService
    {
        Task<SandboxResourceOperationDto> AddAsync(int sandboxResourceId, SandboxResourceOperationDto operationDto);
        Task<SandboxResourceOperationDto> GetByIdAsync(int id);
        Task<SandboxResourceOperationDto> UpdateStatusAsync(int id, string status, string updatedProvisioningState = null);

        Task<SandboxResourceOperationDto> UpdateStatusAndIncreaseTryCountAsync(int id, string status);
        Task<SandboxResourceOperationDto> SetInProgressAsync(int id, string requestId, string status);

        Task<bool> ExistsPreceedingUnfinishedOperationsAsync(SandboxResourceOperationDto operationDto);

        Task<List<SandboxResourceOperation>> GetUnfinishedOperations(int resourceId);

        Task<SandboxResourceOperation> GetUnfinishedDeleteOperation(int resourceId);

        Task<List<SandboxResourceOperation>> AbortAllUnfinishedCreateOrUpdateOperations(int resourceId);

        Task<bool> OperationIsFinishedAndSucceededAsync(int operationId);
     
    }
}