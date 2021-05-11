using Sepes.Infrastructure.Dto.Azure;
using Sepes.Infrastructure.Dto.Azure.Queue;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public interface IAzureQueueService
    {
        void Init(string connectionString, string queueName);

        Task<QueueStorageItem> SendMessageAsync(string messageText, TimeSpan? visibilityTimeout = null, CancellationToken cancellationToken = default);
       
        // Gets first message without removing from queue, but makes it invisible for 30 seconds.
        Task<QueueStorageItem> ReceiveMessageAsync();

        // Updates the message in-place in the queue.
        // The message parameter is a message that has been fetched with ReceiveMessageAsync()
        Task<QueueUpdateReceipt> UpdateMessageAsync(string messageId, string popReceipt, string updatedMessage, int timespan = 30);

        // Message needs to be retrieved with ReceiveMessageAsync() to be able to be deleted. 
        Task DeleteMessageAsync(string messageId, string popReceipt);

        Task DeleteQueueAsync();       
    }
}
