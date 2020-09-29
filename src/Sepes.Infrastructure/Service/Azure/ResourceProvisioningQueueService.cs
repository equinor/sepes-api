using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sepes.Infrastructure.Dto;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class ResourceProvisioningQueueService : IResourceProvisioningQueueService
    {
        readonly ILogger _logger;
        readonly IAzureQueueService _queueService;

        public ResourceProvisioningQueueService(IConfiguration config, ILogger<ResourceProvisioningQueueService> logger, IAzureQueueService queueService)
        {
            _logger = logger;
            _queueService = queueService;
            _queueService.Init(config["ResourceProvisioningQueueConnectionString"], "sandbox-resource-operations-queue");
        }

        public async Task SendMessageAsync(ProvisioningQueueParentDto message)
        {
            await _queueService.SendMessageAsync<ProvisioningQueueParentDto>(message);
        }

        // Message needs to be retrieved with recieveMessage(s)() to be able to be deleted.
        public async Task DeleteMessageAsync(ProvisioningQueueParentDto message)
        {
            await _queueService.DeleteMessageAsync(message.MessageId, message.PopReceipt);
        }

        // Gets first message as QueueMessage without removing from queue, but makes it invisible for 30 seconds.
        public async Task<ProvisioningQueueParentDto> RecieveMessageAsync()
        {
            var messageFromQueue = await _queueService.RecieveMessageAsync();

            if (messageFromQueue != null)
            {
                var convertedMessage = JsonConvert.DeserializeObject<ProvisioningQueueParentDto>(messageFromQueue.MessageText);

                convertedMessage.MessageId = messageFromQueue.MessageId;
                convertedMessage.PopReceipt = messageFromQueue.PopReceipt;              

                return convertedMessage;
            }

            return null;
        }

        public async Task DeleteQueueAsync()
        {
            await _queueService.DeleteQueueAsync();
        }

        public async Task IncreaseInvisibilityAsync(ProvisioningQueueParentDto message, int invisibleForInSeconds)
        {
            var messageAsJson = JsonConvert.SerializeObject(message);
            message.PopReceipt = await _queueService.UpdateMessageAsync(message.MessageId, message.PopReceipt, messageAsJson, invisibleForInSeconds);           
        }
    }
}
