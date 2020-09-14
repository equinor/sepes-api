using Azure.Storage.Queues.Models;
using System.Collections.Generic;

namespace Sepes.Infrastructure.Dto
{
    public class ProvisioningQueueParentDto
    {   

        public string CreatedBySessionId { get; set; }      

        public string CarriedOutBySessionId { get; set; }

        public List<ProvisioningQueueChildDto> Children { get; set; }

        public QueueMessage OriginalMessage { get; set; }
    }
}
