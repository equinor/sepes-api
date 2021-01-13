using Sepes.Infrastructure.Model.Interface;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sepes.Infrastructure.Model
{
    [Table("CloudResourceRoleAssignments")]
    public class CloudResourceRoleAssignment : UpdateableBaseModel, ISupportSoftDelete
    {
        public int CloudResourceId { get; set; }

        [MaxLength(64)]
        public string UserOjectId { get; set; }

        [MaxLength(512)]
        public string RoleId { get; set; }

        [MaxLength(512)]
        public string ForeignSystemId { get; set; }

        public bool? Deleted { get; set; }

        [MaxLength(64)]
        public string DeletedBy { get; set; }

        public DateTime? DeletedAt { get; set; }

        public CloudResource Resource { get; set; }
    }
}
