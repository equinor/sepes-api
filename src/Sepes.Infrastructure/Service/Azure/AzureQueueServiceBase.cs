using Azure;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sepes.Infrastructure.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public abstract class AzureQueueServiceBase : IAzureQueueService
    {
        protected string _connectionString;
        protected string _queueName;
        protected ILogger _logger;       

        public async Task<Response<SendReceipt>> SendMessageAsync(string message)
        {
            var client = await CreateQueueClient();
            return await client.SendMessageAsync(message);
        }

        public async Task<Response<SendReceipt>> SendMessageAsync<T>(T message)
        {
            var serializedMessage = JsonConvert.SerializeObject(message);

            return await SendMessageAsync(serializedMessage);
        }


        // Message needs to be retrieved with recieveMessage(s)() to be able to be deleted.
        public async Task<Response> DeleteMessageAsync(QueueMessage message)
        {
            var client = await CreateQueueClient();
            return await client.DeleteMessageAsync(message.MessageId, message.PopReceipt);
        }

        // Returns approximate number of messages in queue.
        // The number is not lower than the actual number of messages in the queue, but could be higher.
        public async Task<int> GetApproximateNumberOfMessengesInQueueAsync()
        {
            var client = await CreateQueueClient();
            QueueProperties props = await client.GetPropertiesAsync();
            return props.ApproximateMessagesCount;
        }

        // Gets messages from queue without making them invisible.
        // Parameter numberOfMessages has a max of 32.
        public async Task<IEnumerable<PeekedMessage>> PeekMessagesAsync(int numberOfMessages)
        {
            var client = await CreateQueueClient();
            PeekedMessage[] peekedMessages = await client.PeekMessagesAsync(numberOfMessages);
            return peekedMessages;
        }

        // Pop first message as QueueMessage
        public async Task<QueueMessage> PopNextMessageAsync()
        {
            var singleMessage = await RecieveMessageAsync();

            if(singleMessage != null)
            {
                var deleteResult = await DeleteMessageAsync(singleMessage);

                if (deleteResult.Status == (int)HttpStatusCode.NoContent)
                {
                    return singleMessage;
                }
            }           

            return null;
        }

        // Gets first message as QueueMessage without removing from queue, but makes it invisible for 30 seconds.
        public async Task<QueueMessage> RecieveMessageAsync()
        {
            var client = await CreateQueueClient();
            QueueMessage[] messages = await client.ReceiveMessagesAsync();
            return messages.FirstOrDefault();
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
        public async Task UpdateMessageAsync(QueueMessage message, string updatedMessage, int timespan = 30)
        {
            var client = await CreateQueueClient();
            _ = await client.UpdateMessageAsync(message.MessageId, message.PopReceipt, updatedMessage, TimeSpan.FromSeconds(timespan));
        }

        public string SandboxResourceOperationToMessageString(ProvisioningQueueParentDto operation)
        {
            return JsonConvert.SerializeObject(operation);
        }      

        public async Task DeleteQueueAsync()
        {
            var client = await CreateQueueClient();
            _ = await client.DeleteIfExistsAsync();

        }        


        // Helper method for creating queueClient
        async Task<QueueClient> CreateQueueClient()
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
    }
}
