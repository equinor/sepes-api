using System; // Namespace for Console output
using System.Configuration; // Namespace for ConfigurationManager
using System.Threading.Tasks; // Namespace for Task
using Azure.Storage.Queues; // Namespace for Queue storage types
using Azure.Storage.Queues.Models; // Namespace for PeekedMessage

namespace Sepes.Infrastructure.Service.Azure.Interface
{
    public interface IAzureQueueService
    {
        Task SendMessage(string message);
        Task RecieveMessage();
        Task RecieveMessages(int numberOfMessages);
        Task UpdateMessage(string MessageId, string updatedMessage, int timespan = 30);
        Task DeleteMessage();
        Task PeekMessage();
        Task<int> GetNumberOfMessengesInQueue();
    }
}
