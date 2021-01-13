using System;

namespace Sepes.Infrastructure.Dto
{
    public class CloudResourceRoleAssignmentDto : UpdateableBaseDto
    {
        public int CloudResourceId { get; set; }
        
        public string UserOjectId { get; set; }
      
        public string RoleId { get; set; }

        public string ForeignSystemId { get; set; }

        public bool? Deleted { get; set; }
      
        public string DeletedBy { get; set; }

        public DateTime? DeletedAt { get; set; } 
    }
}
