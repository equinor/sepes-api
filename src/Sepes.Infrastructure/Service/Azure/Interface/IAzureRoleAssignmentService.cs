using Sepes.Infrastructure.Dto.Auth;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Azure.Interface
{
    public interface IAzureRoleAssignmentService
    {
        Task<AzureRoleAssignmentResponseDto> AddResourceRoleAssignment(string resourceId, string roleAssignmentId, string roleDefinitionId, string principalId, CancellationToken cancellationToken = default);      
    }
}
