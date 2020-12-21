using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sepes.Infrastructure.Model
{
    [Table("CloudResourceOperations")]
    public class CloudResourceOperation : UpdateableBaseModel
    {        
        public int CloudResourceId { get; set; }

        public int? DependsOnOperationId { get; set; }

        [MaxLength(32)]
        public string OperationType { get; set; }

        public string Status { get; set; }

        public int TryCount { get; set; }

        public int MaxTryCount { get; set; }

        public string BatchId { get; set; }

        public string CreatedBySessionId { get; set; }

        public string CarriedOutBySessionId { get; set; }

        public string Description { get; set; }

        public string LatestError { get; set; }    

        public CloudResource Resource { get; set; }
        
        public CloudResourceOperation DependsOnOperation { get; set; }

        public ICollection<CloudResourceOperation> DependantOnThisOperation { get; set; }
    }
}
