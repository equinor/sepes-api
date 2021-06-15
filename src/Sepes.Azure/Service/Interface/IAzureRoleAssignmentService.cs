using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Sepes.Azure.Dto.RoleAssignment;

namespace Sepes.Azure.Service.Interface
{
    public interface IAzureRoleAssignmentService
    {
        Task<bool> RoleAssignmentExists(string resourceId, string roleAssignmentId, CancellationToken cancellationToken = default);

        Task<AzureRoleAssignment> AddRoleAssignment(string resourceId, string roleDefinitionId, string principalId, string roleAssignmentId = null, CancellationToken cancellationToken = default);
        
        Task<AzureRoleAssignment> DeleteRoleAssignment(string roleAssignmentId, CancellationToken cancellationToken = default);
        Task<List<AzureRoleAssignment>> GetResourceGroupRoleAssignments(string resourceGroupId, string resourceGroupName, CancellationToken cancellation = default);
        Task<List<AzureRoleAssignment>> GetStorageAccountRoleAssignments(string resourceGroupId, string resourceGroupName, string storageAccountId, string storageAccountName, CancellationToken cancellation = default);
    }
}
