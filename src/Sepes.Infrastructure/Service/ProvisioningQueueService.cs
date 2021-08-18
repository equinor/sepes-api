using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Azure.Service.Interface;
using Sepes.Azure.Util;
using Sepes.Common.Constants;
using Sepes.Common.Dto;
using Sepes.Common.Dto.Sandbox;
using Sepes.Common.Util;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Threading;
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

        public async Task<ProvisioningQueueParentDto> SendMessageAsync(ProvisioningQueueParentDto message, TimeSpan? visibilityTimeout = null, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Sending message: {message.Description}, having {message.Children.Count} children");
            var serializedMessage = JsonSerializerUtil.Serialize(message);
            var sendtMessage = await _queueService.SendMessageAsync(serializedMessage, visibilityTimeout, cancellationToken);

            message.MessageId = sendtMessage.MessageId;
            message.PopReceipt = sendtMessage.PopReceipt;
            message.NextVisibleOn = sendtMessage.NextVisibleOn;

            _logger.LogInformation($"{MessagePrefix(message)}: Message sendt");

            return message;
        }

        // Gets first message as QueueMessage without removing from queue, but makes it invisible for 30 seconds.
        public async Task<ProvisioningQueueParentDto> ReceiveMessageAsync()
        {
            _logger.LogInformation($"Receive message");

            var messageFromQueue = await _queueService.ReceiveMessageAsync();

            if (messageFromQueue != null)
            {
                var convertedMessage = JsonSerializerUtil.Deserialize<ProvisioningQueueParentDto>(messageFromQueue.MessageText);

                convertedMessage.MessageId = messageFromQueue.MessageId;
                convertedMessage.PopReceipt = messageFromQueue.PopReceipt;
                convertedMessage.NextVisibleOn = messageFromQueue.NextVisibleOn;
                _logger.LogInformation($"Returning {MessagePrefix(convertedMessage)}, description: {convertedMessage.Description}");
                return convertedMessage;
            }

            _logger.LogInformation($"Receive message: No message found");
            return null;
        }

        // Message needs to be retrieved with ReceiveMessageAsync() to be able to be deleted.
        public async Task DeleteMessageAsync(ProvisioningQueueParentDto message)
        {
            _logger.LogInformation($"{MessagePrefix(message)}: Deleting, children: {message.Children.Count}");
            await _queueService.DeleteMessageAsync(message.MessageId, message.PopReceipt);
        }

        // Message needs to be retrieved with ReceiveMessageAsync() to be able to be deleted.
        public async Task DeleteMessageAsync(string messageId, string popReceipt)
        {
            _logger.LogInformation($"{MessagePrefix(messageId)}: Deleting message");
            await _queueService.DeleteMessageAsync(messageId, popReceipt);
        }      

        public async Task DeleteQueueAsync()
        {
            await _queueService.DeleteQueueAsync();
        }

        public async Task IncreaseInvisibilityAsync(ProvisioningQueueParentDto message, int invisibleForInSeconds)
        {
            _logger.LogInformation($"{MessagePrefix(message)}: Increasing message invisibility by {invisibleForInSeconds} seconds.");
            var messageAsJson = JsonSerializerUtil.Serialize(message);
            var updateReceipt = await _queueService.UpdateMessageAsync(message.MessageId, message.PopReceipt, messageAsJson, invisibleForInSeconds);
            message.PopReceipt = updateReceipt.PopReceipt;
            message.NextVisibleOn = updateReceipt.NextVisibleOn;
            _logger.LogInformation($"{MessagePrefix(message)}: Will be visible again at {updateReceipt.NextVisibleOn} (UTC)");

        }

        string MessagePrefix(ProvisioningQueueParentDto message)
        {
            return $"{message.MessageId}";
        }

        string MessagePrefix(string messageId)
        {
            return $"{messageId}";
        }

        public async Task ReQueueMessageAsync(ProvisioningQueueParentDto message, int? invisibleForInSeconds = default, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"{MessagePrefix(message)}: Re-queuing message with description: {message.Description}. Children: {message.Children.Count}.");

            await DeleteMessageAsync(message);
            message.DequeueCount = 0;
            message.PopReceipt = null;
            message.MessageId = null;

            TimeSpan invisibleForTimespan = invisibleForInSeconds.HasValue ? new TimeSpan(0, 0, invisibleForInSeconds.Value) : new TimeSpan(0, 0, 10);
            await SendMessageAsync(message, visibilityTimeout: invisibleForTimespan, cancellationToken: cancellationToken);
        }

        public async Task IncreaseInvisibleBasedOnResource(CloudResourceOperationDto currentOperation, ProvisioningQueueParentDto queueParentItem)
        {
            var increaseBy = ResourceProivisoningTimeoutResolver.GetTimeoutForOperationInSeconds(currentOperation.Resource.ResourceType, currentOperation.OperationType);
            await IncreaseInvisibilityAsync(queueParentItem, increaseBy);
        }

        public async Task CreateItemAndEnqueue(int operationId, string operationDescription)
        {
            var queueParentItem = new ProvisioningQueueParentDto
            {
                Description = operationDescription
            };

            queueParentItem.Children.Add(new ProvisioningQueueChildDto() { ResourceOperationId = operationId });

            await SendMessageAsync(queueParentItem);
        }

        public async Task CreateItemAndEnqueue(CloudResourceOperation operation)
        {
            await CreateItemAndEnqueue(operation.Id, operation.Description);
        }

        public async Task CreateItemAndEnqueue(CloudResourceOperationDto operation)
        {
            await CreateItemAndEnqueue(operation.Id, operation.Description);
        }       

        public async Task AddNewQueueMessageForOperation(CloudResourceOperation operation)
        {
            var queueParentItem = new ProvisioningQueueParentDto
            {
                Description = operation.Description
            };

            queueParentItem.Children.Add(new ProvisioningQueueChildDto() { ResourceOperationId = operation.Id });

            await SendMessageAsync(queueParentItem, visibilityTimeout: TimeSpan.FromSeconds(5));
        }
    }
}
