using System; // Namespace for Console output
using System.Collections;
using System.Collections.Generic;
using System.Configuration; // Namespace for ConfigurationManager
using System.Threading.Tasks; // Namespace for Task
using Azure.Storage.Queues; // Namespace for Queue storage types
using Azure.Storage.Queues.Models; // Namespace for PeekedMessage
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Service.Azure.Interface;

namespace Sepes.Infrastructure.Service.Azure
{
    public class AzureQueueService : AzureServiceBase, IAzureQueueService
    {
        private const string _queueName = "sandbox-resource-operations";
        private readonly string _connectionString;

        public AzureQueueService(IConfiguration config, ILogger logger) : base(config, logger)
        {
            this._connectionString = config["AzureQueueConnectionString"];
        }

        // Helper method for creating queueClient
        private async Task<QueueClient> CreateQueueClient()
        {
            // Instantiate a QueueClient which will be used to create and manipulate the queue
            QueueClient queueClient = new QueueClient(_connectionString, _queueName);

            // Create the queue if it doesn't already exist
            await queueClient.CreateIfNotExistsAsync();

            // Log creation
            if (await queueClient.ExistsAsync())
            {
                _logger.LogInformation($"Queue '{queueClient.Name}' created");
            }
            else
            {
                _logger.LogInformation($"Queue '{queueClient.Name}' exists");
            }

            return queueClient;
        }

        // Message needs to be retrieved with recieveMessage() to be able to be deleted.
        public async Task DeleteMessage(QueueMessage message)
        {
            var client = await CreateQueueClient();
            _ = await client.DeleteMessageAsync(message.MessageId, message.PopReceipt);
        }

        // Returns approximate number of messages in queue.
        // The number is not lower than the actual number of messages in the queue, but could be higher.
        public async Task<int> GetNumberOfMessengesInQueue()
        {
            var client = await CreateQueueClient();
            QueueProperties props = await client.GetPropertiesAsync();
            return props.ApproximateMessagesCount;
        }

        // Gets messages from queue without making them invisible.
        public async Task<IEnumerable<PeekedMessage>> PeekMessages(int numberOfMessages)
        {
            var client = await CreateQueueClient();
            PeekedMessage[] peekedMessages = await client.PeekMessagesAsync(numberOfMessages);
            return peekedMessages;
        }

        public async Task SendMessage(string message)
        {
            var client = await CreateQueueClient();
            _ = await client.SendMessageAsync(message);
        }

        // Gets first message without removing from queue, but makes it invisible for 30 seconds.
        public async Task<QueueMessage> RecieveMessage()
        {
            var client = await CreateQueueClient();
            QueueMessage[] messages = await client.ReceiveMessagesAsync();
            return messages[0];           
        }

        // Gets message without removing from queue, but makes it invisible for 30 seconds.
        public async Task<IEnumerable<QueueMessage>> RecieveMessages(int numberOfMessages)
        {
            var client = await CreateQueueClient();
            QueueMessage[] queueMessages = await client.ReceiveMessagesAsync(numberOfMessages);
            return queueMessages;
        }

        // Updates the message in-place in the queue.
        // The message parameter is a message that has been fetched with RecieveMessage() or RecieveMessages()
        public async Task UpdateMessage(QueueMessage message, string updatedMessage, int timespan = 30)
        {
            var client = await CreateQueueClient();
            _ = await client.UpdateMessageAsync(message.MessageId, message.PopReceipt, updatedMessage, TimeSpan.FromSeconds(timespan));
        }
    }
}
