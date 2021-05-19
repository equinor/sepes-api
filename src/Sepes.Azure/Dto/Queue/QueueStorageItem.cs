using System;

namespace Sepes.Azure.Dto.Queue
{
    public class QueueStorageItem
    {
        public string MessageId { get; set; }

        public string PopReceipt { get; set; }

        public string MessageText { get; set; }

        public DateTimeOffset? NextVisibleOn { get; set; }
    }
}
