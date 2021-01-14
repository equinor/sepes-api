using Sepes.Infrastructure.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface ICloudResourceRoleAssignmentUpdateService
    {
        Task<CloudResourceRoleAssignmentDto> SetForeignIdAsync(int roleAssignmentId, string foreignSystemId);

        Task ReviseRoleAssignments(int resourceId, string principalId, HashSet<string> rolesUserShouldHave);
    }
}