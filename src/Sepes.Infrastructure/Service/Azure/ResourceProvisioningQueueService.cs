using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sepes.Infrastructure.Dto;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class ResourceProvisioningQueueService : AzureQueueServiceBase, IResourceProvisioningQueueService
    {
        protected readonly IConfiguration _config;      

        public ResourceProvisioningQueueService(IConfiguration config, ILogger<ResourceProvisioningQueueService> logger)
            
        {
            _config = config; 
            _logger = logger;         
            _connectionString = config["ResourceProvisioningQueueConnectionString"];
            _queueName = config["ResourceProvisioningQueueName"];
        }

        public async Task SendMessageAsync(ProvisioningQueueParentDto message)
        {
           await base.SendMessageAsync<ProvisioningQueueParentDto>(message);  
        }

        // Message needs to be retrieved with recieveMessage(s)() to be able to be deleted.
        public async Task DeleteMessageAsync(ProvisioningQueueParentDto message)
        {      
            _ = await base.DeleteMessageAsync(message.OriginalMessage);
        }       

        // Gets first message as QueueMessage without removing from queue, but makes it invisible for 30 seconds.
        public new async Task<ProvisioningQueueParentDto> RecieveMessageAsync()
        {
            var message = await base.PopNextMessageAsync();

            if(message != null)
            {
                var result = JsonConvert.DeserializeObject<ProvisioningQueueParentDto>(message.MessageText);

                result.OriginalMessage = message;

                return result;
            }

            return null; 
        } 
        
        public new async Task DeleteQueueAsync()
        {
            await base.DeleteQueueAsync();
        }
    }
}
