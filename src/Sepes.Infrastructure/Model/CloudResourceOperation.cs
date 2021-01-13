using System;
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

        [MaxLength(32)]
        public string Status { get; set; }

        public int TryCount { get; set; }

        public int MaxTryCount { get; set; }

        [MaxLength(64)]
        public string BatchId { get; set; }

        [MaxLength(64)]
        public string CreatedBySessionId { get; set; }

        [MaxLength(64)]
        public string CarriedOutBySessionId { get; set; }

        [MaxLength(256)]
        public string Description { get; set; }

        [MaxLength(4096)]
        public string LatestError { get; set; }

        [MaxLength(64)]
        public string QueueMessageId { get; set; }

        [MaxLength(64)]
        public string QueueMessagePopReceipt { get; set; }

        public DateTime? QueueMessageVisibleAgainAt { get; set; }

        public CloudResource Resource { get; set; }
        
        public CloudResourceOperation DependsOnOperation { get; set; }

        public ICollection<CloudResourceOperation> DependantOnThisOperation { get; set; }
    }
}
