using System.Collections.Generic;
using System.Threading.Tasks; // Namespace for Task
using Azure.Storage.Queues; // Namespace for Queue storage types
using Azure.Storage.Queues.Models; // Namespace for PeekedMessage

namespace Sepes.Infrastructure.Service.Azure.Interface
{
    public interface IAzureQueueService
    {
        Task SendMessage(string message);

        // Gets first message without removing from queue, but makes it invisible for 30 seconds.
        Task<QueueMessage> RecieveMessage();

        // Gets message without removing from queue, but makes it invisible for 30 seconds.
        Task<IEnumerable<QueueMessage>> RecieveMessages(int numberOfMessages);

        // Updates the message in-place in the queue.
        // The message parameter is a message that has been fetched with RecieveMessage() or RecieveMessages()
        Task UpdateMessage(QueueMessage message, string updatedMessage, int timespan = 30);

        // Message needs to be retrieved with recieveMessage() to be able to be deleted.
        Task DeleteMessage(QueueMessage message);

        // Gets messages from queue without making them invisible.
        Task<IEnumerable<PeekedMessage>> PeekMessages(int numberOfMessages);

        // Returns approximate number of messages in queue.
        // The number is not lower than the actual number of messages in the queue, but could be higher.
        Task<int> GetNumberOfMessengesInQueue();
    }
}
