namespace Sepes.Common.Dto
{
    public class CloudResourceDesiredRoleAssignmentDto
    {
        public CloudResourceDesiredRoleAssignmentDto(string principalId, string roleId)
        {
            PrincipalId = principalId;
            RoleId = roleId;           
        }

        public string PrincipalId { get; set; }
      
        public string RoleId { get; set; }

        //public string RoleDefinitionId { get; set; }       
    }
}
