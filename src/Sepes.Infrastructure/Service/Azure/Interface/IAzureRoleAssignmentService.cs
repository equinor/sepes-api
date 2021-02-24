using Sepes.Infrastructure.Dto;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure.Interface
{
    public interface IAzureRoleAssignmentService
    {
        //Task<bool> RoleAssignmentExists(string resourceId, string roleAssignmentId, CancellationToken cancellationToken = default);

        //Task<AzureRoleAssignment> DeleteRoleAssignment(string roleAssignmentId, CancellationToken cancellationToken = default);
       
        //Task<AzureRoleAssignment> AddRoleAssignment(string resourceId, string roleDefinitionId, string principalId, string roleAssignmentId = null, CancellationToken cancellationToken = default);

        Task SetRoleAssignments(string resourceGroupId, string resourceGroupName, List<CloudResourceDesiredRoleAssignmentDto> desiredRoleAssignments, CancellationToken cancellationToken = default);
       
    }
}
