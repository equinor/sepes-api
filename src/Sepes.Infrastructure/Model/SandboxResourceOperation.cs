using System;

namespace Sepes.Infrastructure.Model
{
    public class SandboxResourceOperation : UpdateableBaseModel
    {
        public int CloudResourceId { get; set; }

        public string JobType { get; set; }

        public string InProgressWith { get; set; }

        public SandboxResource Resource {get;set;}

        //Remember to check rowversion
    }    
}
