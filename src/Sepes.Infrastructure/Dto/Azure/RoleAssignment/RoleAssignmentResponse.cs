using System.Collections.Generic;

namespace Sepes.Infrastructure.Dto.Azure.RoleAssignment
{
    public class RoleAssignmentResponse
    {
        public List<AzureRoleAssignment> value { get; set; }
    }
}
