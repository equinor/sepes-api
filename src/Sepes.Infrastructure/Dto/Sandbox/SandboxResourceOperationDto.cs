using System;
using System.Collections.Generic;

namespace Sepes.Infrastructure.Dto.Sandbox
{
    public class SandboxResourceOperationDto : UpdateableBaseDto
    {
        public string Description { get; set; }
        public string OperationType { get; set; }

        public string Status { get; set; }

        public int TryCount { get; set; }

        public int MaxTryCount { get; set; }

        public string BatchId { get; set; }

        public string CreatedBySessionId { get; set; }

        public string CarriedOutBySessionId { get; set; }

        public int? DependsOnOperationId { get; set; }    

        public SandboxResourceOperationDto DependsOnOperation { get; set; }

        public List<SandboxResourceOperationDto> DependantOnThisOperation { get; set; }

        public SandboxResourceDto Resource { get; set; }

        public string QueueMessageId { get; set; }
      
        public string QueueMessagePopReceipt { get; set; }

        public DateTime? QueueMessageVisibleAgainAt { get; set; }
    }
}
