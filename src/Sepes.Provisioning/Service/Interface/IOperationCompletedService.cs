using System.Threading.Tasks;
using Sepes.Common.Dto;

namespace Sepes.Provisioning.Service.Interface
{
    public interface IOperationCompletedService
    {
        Task<bool> HandledAsAllreadyCompletedAsync(CloudResourceOperationDto operation);
        Task ThrowIfUnexpectedProvisioningState(CloudResourceOperationDto operation);
    }
}