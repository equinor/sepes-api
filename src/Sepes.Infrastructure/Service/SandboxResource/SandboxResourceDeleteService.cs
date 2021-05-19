using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Common.Constants;
using Sepes.Common.Constants.CloudResource;
using Sepes.Common.Util.Provisioning;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class SandboxResourceDeleteService : SandboxServiceBase, ISandboxResourceDeleteService
    {       
        readonly ICloudResourceDeleteService _cloudResourceDeleteService;        
        readonly ICloudResourceOperationCreateService _cloudResourceOperationCreateService;
        readonly IProvisioningQueueService _provisioningQueueService;

        public SandboxResourceDeleteService(IConfiguration config, SepesDbContext db, IMapper mapper, ILogger<SandboxResourceDeleteService> logger, IUserService userService,
               ISandboxModelService sandboxModelService,
               ICloudResourceDeleteService cloudResourceDeleteService, ICloudResourceOperationCreateService cloudResourceOperationCreateService,
           IProvisioningQueueService provisioningQueueService)
              : base(config, db, mapper, logger, userService, sandboxModelService)
        {
         
            _cloudResourceDeleteService = cloudResourceDeleteService;
            _cloudResourceOperationCreateService = cloudResourceOperationCreateService;
            _provisioningQueueService = provisioningQueueService;
        }     

        public async Task HandleSandboxDeleteAsync(int sandboxId, EventId eventId)
        {
            var sandboxFromDb = await GetWithoutChecks(sandboxId);

            CloudResource sandboxResourceGroup = null;

            if (sandboxFromDb.Resources.Count > 0)
            {
                //Mark all resources as deleted
                foreach (var curResource in sandboxFromDb.Resources)
                {
                    if (curResource.ResourceType == AzureResourceType.ResourceGroup)
                    {
                        sandboxResourceGroup = curResource;
                    }

                    _logger.LogInformation(eventId, "Study {0}, Sandbox {1}: Marking resource {2} for deletion", sandboxFromDb.StudyId, sandboxId, curResource.Id);

                    await _cloudResourceDeleteService.MarkAsDeletedAsync(curResource.Id);                
                }

                if (sandboxResourceGroup == null)
                {
                    throw new Exception($"Unable to find ResourceGroup record in DB for Sandbox {sandboxId}, StudyId: {sandboxFromDb.StudyId}.");
                }

                _logger.LogInformation(eventId, $"Creating delete operation for resource group {sandboxResourceGroup.ResourceGroupName}");

                var deleteOperation = await _cloudResourceOperationCreateService.CreateDeleteOperationAsync(sandboxResourceGroup.Id, ResourceOperationDescriptionUtils.CreateDescriptionForResourceOperation(sandboxResourceGroup.ResourceType,
                     CloudResourceOperationType.DELETE,
                    sandboxId: sandboxResourceGroup.SandboxId.Value) + ". (Delete of Sandbox resource group and all resources within)");

                _logger.LogInformation(eventId, "Study {0}, Sandbox {1}: Queuing operation", sandboxFromDb.StudyId, sandboxId);

                var queueParentItem = QueueItemFactory.CreateParent(deleteOperation);              
               
                await _provisioningQueueService.SendMessageAsync(queueParentItem, visibilityTimeout: TimeSpan.FromSeconds(10));
            }
            else
            {
                _logger.LogCritical(eventId, "Study {0}, Sandbox {1}: Unable to find any resources for Sandbox", sandboxFromDb.StudyId, sandboxId);
                await _db.SaveChangesAsync();
            }
        }
    }
}
