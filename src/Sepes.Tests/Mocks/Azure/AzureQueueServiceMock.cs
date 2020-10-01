using Newtonsoft.Json;
using Sepes.Infrastructure.Dto.Azure;
using Sepes.Infrastructure.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Tests.Mocks.Azure
{
    public class AzureQueueServiceMock : IAzureQueueService
    {
        Queue<QueueStorageItemDto> _queue = new Queue<QueueStorageItemDto>();

        Dictionary<string, QueueMessageWrapper> _invisibleItems = new Dictionary<string, QueueMessageWrapper>();

        public void Init(string connectionString, string queueName)
        {
           //Don't care about connection strings and names
        }

        public async Task SendMessageAsync(string messageText)
        {
            await SendInternal(messageText);          
        }

        public async Task SendMessageAsync<T>(T message)
        {
            var serializedMessage = JsonConvert.SerializeObject(message);
            await SendMessageAsync(serializedMessage);
        }     

        public async Task<QueueStorageItemDto> RecieveMessageAsync()
        {
            AddBackItemsThatShouldBeVisibleAgain();

            QueueStorageItemDto dequeuedMessage = null;

            if (_queue.TryDequeue(out dequeuedMessage))
            {
                dequeuedMessage.PopReceipt = Guid.NewGuid().ToString();               
                _invisibleItems.Add(dequeuedMessage.MessageId, new QueueMessageWrapper(dequeuedMessage, 30));            

                return dequeuedMessage;
            }

            return null;
        }
     
        public async Task<QueueStorageItemDto> UpdateMessageAsync(QueueStorageItemDto message, int timespan = 30)
        {
            AddBackItemsThatShouldBeVisibleAgain();

            QueueMessageWrapper itemToUpdate = null;

            if (_invisibleItems.TryGetValue(message.MessageId, out itemToUpdate))
            {
                if (message.PopReceipt == itemToUpdate.PopReceipt)
                {
                    itemToUpdate.Message.MessageText = message.MessageText;
                    itemToUpdate.VisibleAgain = DateTime.UtcNow.AddSeconds(timespan);
                    message.PopReceipt = itemToUpdate.PopReceipt = Guid.NewGuid().ToString();
                    return message;
                }
            }

            throw new ArgumentException($"No item with message id: {message.MessageId} found!");
        }

        async Task SendInternal(string messageText)
        {
            var item = new QueueStorageItemDto() { MessageId = Guid.NewGuid().ToString(), MessageText = messageText };
            _queue.Enqueue(item);
        }

        public async Task DeleteMessageAsync(QueueStorageItemDto message)
        {
            QueueMessageWrapper itemToDelete = null;

            if (_invisibleItems.TryGetValue(message.MessageId, out itemToDelete))
            {
                if(message.PopReceipt == itemToDelete.PopReceipt)
                {
                    _invisibleItems[message.MessageId] = null;
                }                            
            }
        }

        public async Task DeleteQueueAsync()
        {
            AddBackItemsThatShouldBeVisibleAgain();
            _queue = new Queue<QueueStorageItemDto>();
        }

        void AddBackItemsThatShouldBeVisibleAgain()
        {
            List<QueueMessageWrapper> addBackIn = new List<QueueMessageWrapper>();

            var shouldBeVisibleAgain = _invisibleItems.Values.Where(i => i.VisibleAgain <= DateTime.UtcNow).ToList();

           if(shouldBeVisibleAgain.Count > 0)
            {
               var newQueue = new Queue<QueueStorageItemDto>();

                foreach(var curAddBackIn in shouldBeVisibleAgain)
                {
                    newQueue.Enqueue(curAddBackIn.Message);
                }

                while(_queue.Count > 0)
                {
                    var curItemFromOldQueue = _queue.Dequeue();
                    newQueue.Enqueue(curItemFromOldQueue);
                }

                _queue = newQueue;
            }
        }

        public Task<string> UpdateMessageAsync(string messageId, string popReceipt, string updatedMessage, int timespan = 30)
        {
            throw new NotImplementedException();
        }

        public Task DeleteMessageAsync(string messageId, string popReceipt)
        {
            throw new NotImplementedException();
        }
    }
    public class QueueMessageWrapper
    {
        public QueueStorageItemDto Message { get; set; }

        public DateTime VisibleAgain { get; set; }

        public string PopReceipt { get; set; }

        public QueueMessageWrapper(QueueStorageItemDto message, int invisibleFor)
        {
            Message = message;
            VisibleAgain = DateTime.UtcNow.AddSeconds(invisibleFor);
        }

    }
}
