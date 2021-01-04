using System;
using System.Text;

namespace Sepes.Infrastructure.Exceptions
{
    public class ProvisioningException : Exception
    {
        public ProvisioningException(string message,
            string newOperationStatus = null,
            bool proceedWithOtherOperations = false,
            bool deleteFromQueue = false,
            int? postponeQueueItemFor = default,
            bool storeQueueInfoOnOperation = false,
             bool logAsWarning = false,
            Exception innerException = null)
            : base(message, innerException)
        {
            NewOperationStatus = newOperationStatus;
            ProceedWithOtherOperations = proceedWithOtherOperations;
            DeleteFromQueue = deleteFromQueue;
            PostponeQueueItemFor = postponeQueueItemFor;
            StoreQueueInfoOnOperation = storeQueueInfoOnOperation;
            LogAsWarning = logAsWarning;
        }

        public bool DeleteFromQueue { get; set; }

        public int? PostponeQueueItemFor { get; set; }

        public string NewOperationStatus { get; set; }

        public bool ProceedWithOtherOperations { get; set; }

        public bool StoreQueueInfoOnOperation { get; set; }

        public bool LogAsWarning { get; set; }

        public string GetMessageSummary()
        {
            var sbMessages = new StringBuilder();

            Exception curException = this;

            while (curException != null)
            {
                sbMessages.AppendLine(curException.Message);
                curException = curException.InnerException;
            }

            return sbMessages.ToString();
        }
    }
}
