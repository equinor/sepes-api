using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model.Config;
using Sepes.Infrastructure.Service.Interface;
using System.Threading.Tasks;

namespace Sepes.CloudResourceWorker
{
    public class ResourceOperationsQueueFunction
    {    
        readonly ISandboxResourceProvisioningService _provisioningService;

        public ResourceOperationsQueueFunction(ISandboxResourceProvisioningService provisioningService)
        {
            _provisioningService = provisioningService;
        }

        [FunctionName("ResourceOperationsQueue")]
        [StorageAccount(ConfigConstants.RESOURCE_PROVISIONING_QUEUE_CONSTRING)]      
        public async Task Run([QueueTrigger(queueName: "sandbox-resource-operations-queue")] CloudQueueMessage myQueueItem, ILogger log)
        {           
            log.LogInformation($"Processing message: {myQueueItem.Id}, pop count: {myQueueItem.DequeueCount}, exp: {myQueueItem.ExpirationTime}, next visible: { myQueueItem.NextVisibleTime}");

            var transformedQueueItem = JsonConvert.DeserializeObject<ProvisioningQueueParentDto>(myQueueItem.AsString);
            transformedQueueItem.MessageId = myQueueItem.Id;
            transformedQueueItem.PopReceipt = myQueueItem.PopReceipt;

          await  _provisioningService.HandleQueueItem(transformedQueueItem);          
        }
    }
}
