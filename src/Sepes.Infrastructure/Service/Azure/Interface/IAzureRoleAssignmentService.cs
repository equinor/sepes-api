using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sepes.Infrastructure.Dto.Azure.RoleAssignment;

namespace Sepes.Infrastructure.Service.Azure.Interface
{
    public interface IAzureRoleAssignmentService
    {
        Task<bool> RoleAssignmentExists(string resourceId, string roleAssignmentId, CancellationToken cancellationToken = default);

        Task<AzureRoleAssignment> AddRoleAssignment(string resourceId, string roleDefinitionId, string principalId, string roleAssignmentId = null, CancellationToken cancellationToken = default);
        
        Task<AzureRoleAssignment> DeleteRoleAssignment(string roleAssignmentId, CancellationToken cancellationToken = default);
        Task<List<AzureRoleAssignment>> GetResourceGroupRoleAssignments(string resourceGroupId, string resourceGroupName, CancellationToken cancellation = default);
    }
}
