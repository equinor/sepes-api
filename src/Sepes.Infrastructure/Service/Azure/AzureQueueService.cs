using System; // Namespace for Console output
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

        public Task DeleteMessage()
        {
            throw new NotImplementedException();
        }

        public Task<int> GetNumberOfMessengesInQueue()
        {
            throw new NotImplementedException();
        }

        public Task PeekMessage()
        {
            throw new NotImplementedException();
        }

        public async Task SendMessage(string message)
        {
            var client = await CreateQueueClient();
            _ = await client.SendMessageAsync(message);
        }

        public Task RecieveMessage()
        {
            throw new NotImplementedException();
        }

        public Task RecieveMessages(int numberOfMessages)
        {
            throw new NotImplementedException();
        }

        public Task UpdateMessage(string MessageId, string updatedMessage, int timespan = 30)
        {
            throw new NotImplementedException();
        }
    }
}
