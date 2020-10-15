﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model.Config;
using Sepes.Infrastructure.Service.Interface;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class ProvisioningQueueService : IProvisioningQueueService
    {
        readonly ILogger _logger;
        readonly IAzureQueueService _queueService;

        public ProvisioningQueueService(IConfiguration config, ILogger<ProvisioningQueueService> logger, IAzureQueueService queueService)
        {
            _logger = logger;
            _queueService = queueService;
            _queueService.Init(config[ConfigConstants.RESOURCE_PROVISIONING_QUEUE_CONSTRING], "sandbox-resource-operations-queue");
        }

        public async Task SendMessageAsync(ProvisioningQueueParentDto message)
        {
            _logger.LogInformation($"Queue: Adding message: {message.Description}, having {message.Children.Count} children");
            await _queueService.SendMessageAsync<ProvisioningQueueParentDto>(message);
        }

        // Message needs to be retrieved with recieveMessage(s)() to be able to be deleted.
        public async Task DeleteMessageAsync(ProvisioningQueueParentDto message)
        {
            _logger.LogInformation($"Queue: Deleting message: {message.MessageId} with description \"{message.Description}\", having {message.Children.Count} children");
            await _queueService.DeleteMessageAsync(message.MessageId, message.PopReceipt);
        }

        // Gets first message as QueueMessage without removing from queue, but makes it invisible for 30 seconds.
        public async Task<ProvisioningQueueParentDto> RecieveMessageAsync()
        {
            _logger.LogInformation($"Queue: Receive message");
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
            _logger.LogInformation($"Queue: Increasing message invisibility message for message {message.MessageId} with description \"{message.Description}\" by {invisibleForInSeconds} seconds.");
            var messageAsJson = JsonConvert.SerializeObject(message);
            var updateReceipt = await _queueService.UpdateMessageAsync(message.MessageId, message.PopReceipt, messageAsJson, invisibleForInSeconds);
            message.PopReceipt = updateReceipt.PopReceipt;
            _logger.LogInformation($"Queue: Message {message.MessageId} will be visible again at {updateReceipt.NextVisibleOn} (UTC)");

        }
    }
}