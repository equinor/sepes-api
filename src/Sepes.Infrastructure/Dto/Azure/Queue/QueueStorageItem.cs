using System;

namespace Sepes.Infrastructure.Dto.Azure
{
    public class QueueStorageItem
    {
        public string MessageId { get; set; }

        public string PopReceipt { get; set; }

        public string MessageText { get; set; }

        public DateTimeOffset? NextVisibleOn { get; set; }
    }
}
