using Sepes.Infrastructure.Dto;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface ISandboxResourceOperationService
    {
        Task<SandboxResourceOperationDto> Add(int sandboxResourceId, SandboxResourceOperationDto operationDto);
        Task<SandboxResourceOperationDto> GetByIdAsync(int id);
        Task<SandboxResourceOperationDto> UpdateStatus(int id, string status, string updatedProvisioningState = null);

        Task<SandboxResourceOperationDto> UpdateStatusAndIncreaseTryCount(int id, string status);
        Task<SandboxResourceOperationDto> SetInProgress(int id, string requestId, string status);

        Task<bool> ExistsPreceedingUnfinishedOperations(SandboxResourceOperationDto operationDto);
    }
}