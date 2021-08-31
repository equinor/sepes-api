using Sepes.Azure.Dto.Queue;
using Sepes.Azure.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Tests.Common.Mocks.Azure
{
    public class AzureQueueServiceMock : IAzureQueueService
    {
        Queue<QueueStorageItem> _queue = new Queue<QueueStorageItem>();

        readonly Dictionary<string, QueueMessageWrapper> _invisibleItems = new Dictionary<string, QueueMessageWrapper>();

        public void Init(string connectionString, string queueName)
        {
           //Don't care about connection strings and names
        }       

        public async Task<QueueStorageItem> SendMessageAsync(string messageText, TimeSpan? visibilityTimeout, CancellationToken cancellationToken)
        {
            var item = new QueueStorageItem() { MessageId = Guid.NewGuid().ToString(), MessageText = messageText };
            _queue.Enqueue(item);
            return item;
        }

        public async Task<QueueStorageItem> ReceiveMessageAsync()
        {
            AddBackItemsThatShouldBeVisibleAgain();

            if (_queue.TryDequeue(out QueueStorageItem dequeuedMessage))
            {              
                dequeuedMessage.NextVisibleOn = DateTime.UtcNow.AddSeconds(30);
                dequeuedMessage.PopReceipt = Guid.NewGuid().ToString();             

                _invisibleItems.Add(dequeuedMessage.MessageId, new QueueMessageWrapper(dequeuedMessage));

                return dequeuedMessage;
            }

            return null;
        }         

        public async Task<QueueUpdateReceipt> UpdateMessageAsync(string messageId, string popReceipt, string updatedMessage, int timespan = 30)
        {
            AddBackItemsThatShouldBeVisibleAgain();

            var messageToUpdate = GetMessageInternal(messageId, popReceipt);

            if (messageToUpdate != null)
            {
                messageToUpdate.Message.MessageText = updatedMessage;
                messageToUpdate.VisibleAgain = DateTime.UtcNow.AddSeconds(timespan);
                messageToUpdate.Message.NextVisibleOn = DateTime.UtcNow.AddSeconds(timespan);
                messageToUpdate.Message.PopReceipt = Guid.NewGuid().ToString();

                return await Task.FromResult(new QueueUpdateReceipt(messageToUpdate.Message.PopReceipt, messageToUpdate.VisibleAgain));                
            }

            throw new ArgumentException($"No item with message id: {messageId} found!");
        }

        QueueMessageWrapper GetMessageInternal(string messageId, string popReceipt)
        {
            if (_invisibleItems.TryGetValue(messageId, out QueueMessageWrapper itemToUpdate))
            {
                if (popReceipt == itemToUpdate.Message.PopReceipt)
                {
                    return itemToUpdate;
                }
            }           

            throw new ArgumentException($"No item with message id: {messageId} found!");
        }

        public Task DeleteMessageAsync(string messageId, string popReceipt)
        {
            if (_invisibleItems.TryGetValue(messageId, out QueueMessageWrapper itemToDelete) && popReceipt == itemToDelete.Message.PopReceipt)
            {
                _invisibleItems.Remove(messageId);
                return Task.CompletedTask;
            }

            throw new Exception($"Message {messageId} not found!");
        }

        public async Task DeleteQueueAsync()
        {
            AddBackItemsThatShouldBeVisibleAgain();
            _queue = new Queue<QueueStorageItem>();
        }

        void AddBackItemsThatShouldBeVisibleAgain()
        {
            var shouldBeVisibleAgain = _invisibleItems.Values.Where(i => i.VisibleAgain <= DateTime.UtcNow).ToList();

            if (shouldBeVisibleAgain.Count > 0)
            {
                var newQueue = new Queue<QueueStorageItem>();

                foreach (var curAddBackIn in shouldBeVisibleAgain)
                {
                    newQueue.Enqueue(curAddBackIn.Message);
                }

                while (_queue.Count > 0)
                {
                    var curItemFromOldQueue = _queue.Dequeue();
                    newQueue.Enqueue(curItemFromOldQueue);
                }

                _queue = newQueue;
            }
        }

    }
    public class QueueMessageWrapper
    {
        public QueueStorageItem Message { get; set; }

        public DateTime VisibleAgain { get; set; }

        public string PopReceipt { get; set; }

        public QueueMessageWrapper(QueueStorageItem message)
        {
            Message = message;
            VisibleAgain = message.NextVisibleOn.Value.UtcDateTime;
        }

    }    
}
