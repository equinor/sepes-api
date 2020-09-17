using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sepes.Infrastructure.Dto.Azure;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class AzureQueueService : IAzureQueueService
    {
        readonly ILogger _logger;
        string _connectionString;
        string _queueName;

        public AzureQueueService(ILogger<AzureQueueService> logger)
        {
            _logger = logger;                  
        }

        public void Init(string connectionString, string queueName)
        {
            _connectionString = connectionString;
            _queueName = queueName;
        }

        public async Task SendMessageAsync(string message)
        {
            var client = await CreateQueueClient();
           await client.SendMessageAsync(message);    
        }

        public async Task SendMessageAsync<T>(T message)
        {
            var serializedMessage = JsonConvert.SerializeObject(message);
            await SendMessageAsync(serializedMessage);
        }

        // Message needs to be retrieved with recieveMessage(s)() to be able to be deleted.
        public async Task DeleteMessageAsync(QueueStorageItemDto message)
        {
            var client = await CreateQueueClient();
            await client.DeleteMessageAsync(message.MessageId, message.PopReceipt);
        }

        // Returns approximate number of messages in queue.
        // The number is not lower than the actual number of messages in the queue, but could be higher.
        //public async Task<int> GetApproximateNumberOfMessengesInQueueAsync()
        //{
        //    var client = await CreateQueueClient();
        //    QueueProperties props = await client.GetPropertiesAsync();
        //    return props.ApproximateMessagesCount;
        //}

        // Gets messages from queue without making them invisible.
        // Parameter numberOfMessages has a max of 32.
        //public async Task<IEnumerable<PeekedMessage>> PeekMessagesAsync(int numberOfMessages)
        //{
        //    var client = await CreateQueueClient();
        //    PeekedMessage[] peekedMessages = await client.PeekMessagesAsync(numberOfMessages);
        //    return peekedMessages;
        //}

        // Pop first message as QueueMessage
        //public async Task<QueueMessage> PopNextMessageAsync()
        //{
        //    var singleMessage = await RecieveMessageAsync();

        //    if(singleMessage != null)
        //    {
        //        var deleteResult = await DeleteMessageAsync(singleMessage);

        //        if (deleteResult.Status == (int)HttpStatusCode.NoContent)
        //        {
        //            return singleMessage;
        //        }
        //    }           

        //    return null;
        //}

        // Gets first message as QueueMessage without removing from queue, but makes it invisible for 30 seconds.
        public async Task<QueueStorageItemDto> RecieveMessageAsync()
        {
            var client = await CreateQueueClient();
            QueueMessage[] messages = await client.ReceiveMessagesAsync();
            var firstMessage = messages.FirstOrDefault();

            if(firstMessage != null)
            {
                return new QueueStorageItemDto() { MessageId = firstMessage.MessageId, MessageText = firstMessage.MessageText, PopReceipt = firstMessage.PopReceipt };
            }

            return null;          
        }

        // Gets message without removing from queue, but makes it invisible for 30 seconds.
        // Parameter numberOfMessages has a max of 32.
        //public async Task<IEnumerable<QueueMessage>> RecieveMessagesAsync(int numberOfMessages)
        //{
        //    var client = await CreateQueueClient();
        //    QueueMessage[] queueMessages = await client.ReceiveMessagesAsync(numberOfMessages);
        //    return queueMessages;
        //}

        // Updates the message in-place in the queue.
        // The message parameter is a message that has been fetched with RecieveMessageRaw() or RecieveMessages()
        public async Task<QueueStorageItemDto> UpdateMessageAsync(QueueStorageItemDto message, int timespan = 30)
        {
            var client = await CreateQueueClient();
            var updateReceipt = await client.UpdateMessageAsync (message.MessageId, message.PopReceipt, message.MessageText, TimeSpan.FromSeconds(timespan));
            message.PopReceipt = updateReceipt.Value.PopReceipt;
            return message;
        }      

        public async Task DeleteQueueAsync()
        {
            var client = await CreateQueueClient();
            _ = await client.DeleteIfExistsAsync();
        }        


        // Helper method for creating queueClient
        async Task<QueueClient> CreateQueueClient()
        {
            if (String.IsNullOrWhiteSpace(_connectionString) || String.IsNullOrWhiteSpace(_queueName))
            {
                throw new NullReferenceException("ConnectionString or Queue name is null. Remember to call Init() method, providing a connection string and queue name");
            }

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

     
    }
}
