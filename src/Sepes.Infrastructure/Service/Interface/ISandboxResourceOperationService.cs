using Sepes.Infrastructure.Dto;
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

        Task<bool> OperationIsFinishedAndSucceededAsync(int operationId);
     
    }
}