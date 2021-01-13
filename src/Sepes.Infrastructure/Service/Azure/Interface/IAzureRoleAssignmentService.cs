using Sepes.Infrastructure.Dto.Auth;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure.Interface
{
    public interface IAzureRoleAssignmentService
    {
        Task<bool> RoleAssignmentExists(string resourceId, string roleAssignmentId, CancellationToken cancellationToken = default);

        Task<AzureRoleAssignmentResponseDto> DeleteRoleAssignment(string resourceId, string roleAssignmentId, CancellationToken cancellationToken = default);
       
        Task<AzureRoleAssignmentResponseDto> AddRoleAssignment(string resourceId, string roleDefinitionId, string principalId, string roleAssignmentId = null, CancellationToken cancellationToken = default);
    }
}
