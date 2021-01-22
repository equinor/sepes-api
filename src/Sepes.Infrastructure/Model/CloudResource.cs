using Sepes.Infrastructure.Model.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sepes.Infrastructure.Model
{
    [Table("CloudResources")]
    public class CloudResource : UpdateableBaseModel, ISupportSoftDelete
    {
        public int? SandboxId { get; set; }

        public int? StudyId { get; set; }

        [MaxLength(256)]
        public string ResourceId { get; set; }

        [MaxLength(256)]
        public string ResourceKey { get; set; }

        [MaxLength(256)]
        public string ResourceName { get; set; }

        [MaxLength(64)]
        public string ResourceType { get; set; }

        [MaxLength(64)]
        public string Purpose { get; set; }

        [MaxLength(64)]
        public string ResourceGroupName { get; set; }

        [MaxLength(64)]
        public string LastKnownProvisioningState { get; set; }

        [MaxLength(4096)]
        public string Tags { get; set; }

        [MaxLength(32)]
        public string Region { get; set; }

        [MaxLength(4096)]
        public string ConfigString { get; set; }

        public bool SandboxControlled { get; set; }

        public bool Deleted { get; set; }

        public DateTime? DeletedAt { get; set; }

        [MaxLength(64)]
        public string DeletedBy { get; set; }

        public int? ParentResourceId { get; set; }

        public Sandbox Sandbox { get; set; }

        public Study Study { get; set; }

        public List<CloudResourceOperation> Operations { get; set; }      

        public CloudResource ParentResource { get; set; }
      
        public List<CloudResource> ChildResources { get; set; }       
    }    
}
