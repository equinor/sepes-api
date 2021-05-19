using System;

namespace Sepes.Azure.Dto.Queue
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
