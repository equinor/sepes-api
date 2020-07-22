using System;
using System.Collections.Generic;

namespace Sepes.Infrastructure.Model
{
    public class SandboxResource : UpdateableBaseModel
    {
        public int SandboxId { get; set; }

        public string ResourceId { get; set; }

        public string ResourceKey { get; set; }

        public string ResourceName { get; set; }

        public string ResourceType { get; set; }

        public string ResourceGroupId { get; set; }

        public string ResourceGroupName { get; set; }

        public string Status { get; set; }

        public DateTime Deleted { get; set; }

        public string DeletedBy { get; set; }

        public Sandbox Sandbox { get; set; }

        public List<SandboxResourceOperation> Operations{ get; set; }
    }    
}
