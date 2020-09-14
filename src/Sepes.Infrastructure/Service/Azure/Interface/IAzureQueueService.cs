using Azure;
using Azure.Storage.Queues.Models; // Namespace for PeekedMessage
using System.Collections.Generic;
using System.Threading.Tasks; // Namespace for Task

namespace Sepes.Infrastructure.Service
{
    public interface IAzureQueueService
    {
        Task<Response<SendReceipt>> SendMessageAsync(string message);

        Task<Response<SendReceipt>> SendMessageAsync<T>(T message);

        // Gets first message without removing from queue, but makes it invisible for 30 seconds.
        Task<QueueMessage> RecieveMessageAsync();

        Task<QueueMessage> PopNextMessageAsync();

        // Gets message without removing from queue, but makes it invisible for 30 seconds.
        //Task<IEnumerable<QueueMessage>> RecieveMessagesAsync(int numberOfMessages);

        // Updates the message in-place in the queue.
        // The message parameter is a message that has been fetched with RecieveMessage() or RecieveMessages()
        Task UpdateMessageAsync(QueueMessage message, string updatedMessage, int timespan = 30);

        // Message needs to be retrieved with recieveMessage(s)() to be able to be deleted.
        Task<Response> DeleteMessageAsync(QueueMessage message);

        // Gets messages from queue without making them invisible.
        Task<IEnumerable<PeekedMessage>> PeekMessagesAsync(int numberOfMessages);

        // Returns approximate number of messages in queue.
        // The number is not lower than the actual number of messages in the queue, but could be higher.
        Task<int> GetApproximateNumberOfMessengesInQueueAsync();       

        Task DeleteQueueAsync();       
    }
}
