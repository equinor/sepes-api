using System;

namespace Sepes.Azure.Dto.RoleAssignment
{
    public class AzureRoleAssignmentRequestDto
    {
        public RoleAssignmentRequestProperties properties { get; set; }

        public AzureRoleAssignmentRequestDto(string resourceId, string principalId)
        {
            properties = new RoleAssignmentRequestProperties(resourceId, principalId);
        }
    }

    public class RoleAssignmentRequestProperties
    {
        public RoleAssignmentRequestProperties()
        {
        }

        public RoleAssignmentRequestProperties(string roleDefinitionId, string principalId)
        {
            this.roleDefinitionId = roleDefinitionId;
            this.principalId = principalId;
        }

        public string roleDefinitionId { get; set; }

        public string principalId { get; set; }

    }

    public class AzureRoleAssignment
    {
        public AzureRoleAssignment()
        {
        }

        public string id { get; set; }

        public string type { get; set; }

        public string name { get; set; }

        public RoleAssignmentResponseProperties properties { get; set; }

    }



    public class RoleAssignmentResponseProperties : RoleAssignmentRequestProperties
    {
        public RoleAssignmentResponseProperties()
        {
        }       

        public string scope { get; set; }

        public string createdBy { get; set; }

        public string updatedBy { get; set; }

        public DateTime createdOn { get; set; }

        public DateTime updatedOn { get; set; }

    }
}
