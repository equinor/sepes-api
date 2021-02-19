using System;

namespace Sepes.Infrastructure.Dto.Azure.Queue
{
    public class QueueUpdateReceipt
    {
        public string PopReceipt { get; }
        public DateTimeOffset NextVisibleOn { get; }

        public QueueUpdateReceipt(string popReceipt, DateTimeOffset nextVisibleOn)
        {
            PopReceipt = popReceipt;
            NextVisibleOn = nextVisibleOn;
        }    
    }
}
