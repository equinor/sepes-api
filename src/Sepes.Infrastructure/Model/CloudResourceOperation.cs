using System;

namespace Sepes.Infrastructure.Model
{
    public class CloudResourceOperation : UpdateableBaseModel
    {
        public int CloudResourceId { get; set; }

        public string JobType { get; set; }

        public string InProgressWith { get; set; }

        public CloudResource Resource {get;set;}

        //Remember to check rowversion
    }    
}
