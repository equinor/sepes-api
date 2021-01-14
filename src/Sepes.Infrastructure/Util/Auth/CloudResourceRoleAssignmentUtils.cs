using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model;

namespace Sepes.Infrastructure.Util.Auth
{
    public static class CloudResourceRoleAssignmentUtils
    {
       public static void MarkAllForSandboxAsDeleted(Sandbox sandbox, UserDto currentUser)
        {
            foreach(var curResource in sandbox.Resources)
            {
                foreach(var curRoleAssignment in curResource.RoleAssignments)
                {
                    SoftDeleteUtil.MarkAsDeleted(curRoleAssignment, currentUser);
                }
            }
        }
    }
}
