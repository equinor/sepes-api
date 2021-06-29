using Sepes.Common.Constants;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IOperationPermissionService
    {
        Task<bool> HasAccessToOperation(UserOperation operation);
        Task HasAccessToAnyOperationOrThrow(params UserOperation[] operations);

        Task HasAccessToOperationOrThrow(UserOperation operation);
    }
}
