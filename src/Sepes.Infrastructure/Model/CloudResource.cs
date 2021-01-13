using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sepes.Infrastructure.Model
{
    [Table("CloudResources")]
    public class CloudResource : UpdateableBaseModel
    {
        public int SandboxId { get; set; }

        public string ResourceId { get; set; }

        public string ResourceKey { get; set; }

        public string ResourceName { get; set; }

        public string ResourceType { get; set; }      

        public string ResourceGroupName { get; set; }


        public string LastKnownProvisioningState { get; set; }

        public string Tags { get; set; }

        public string Region { get; set; }

        public string ConfigString { get; set; }

        public bool SandboxControlled { get; set; }

        public DateTime? Deleted { get; set; }      

        public string DeletedBy { get; set; }

        public int? ParentResourceId { get; set; }

        public Sandbox Sandbox { get; set; }

        public List<CloudResourceOperation> Operations { get; set; }

        public List<CloudResourceRoleAssignment> RoleAssignments { get; set; }

        public CloudResource ParentResource { get; set; }
      
        public List<CloudResource> ChildResources { get; set; }       
    }    
}
