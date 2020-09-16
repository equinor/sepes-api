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
            _queueService.Init(config["ResourceProvisioningQueueConnectionString"], config["ResourceProvisioningQueueName"]);
        }

        public async Task SendMessageAsync(ProvisioningQueueParentDto message)
        {
           await _queueService.SendMessageAsync<ProvisioningQueueParentDto>(message);  
        }

        // Message needs to be retrieved with recieveMessage(s)() to be able to be deleted.
        public async Task DeleteMessageAsync(ProvisioningQueueParentDto message)
        {      
            await _queueService.DeleteMessageAsync(message);
        }       

        // Gets first message as QueueMessage without removing from queue, but makes it invisible for 30 seconds.
        public async Task<ProvisioningQueueParentDto> RecieveMessageAsync()
        {
            var message = await _queueService.RecieveMessageAsync();

            if(message != null)
            {
                var result = JsonConvert.DeserializeObject<ProvisioningQueueParentDto>(message.MessageText);

                result.MessageId = message.MessageId;
                result.PopReceipt = message.PopReceipt;
                result.MessageText = message.MessageText;

                return result;
            }

            return null; 
        } 
        
        public async Task DeleteQueueAsync()
        {
            await _queueService.DeleteQueueAsync();
        }
    }
}
