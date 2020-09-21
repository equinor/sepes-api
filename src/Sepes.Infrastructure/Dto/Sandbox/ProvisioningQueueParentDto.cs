using Azure.Storage.Queues.Models;
using Sepes.Infrastructure.Dto.Azure;
using System.Collections.Generic;

namespace Sepes.Infrastructure.Dto
{
    public class ProvisioningQueueParentDto : QueueStorageItemDto
    {
        public int SandboxId { get; set; }

        public string Description { get; set; }

        public List<ProvisioningQueueChildDto> Children { get; set; } = new List<ProvisioningQueueChildDto>();    
    }
}
