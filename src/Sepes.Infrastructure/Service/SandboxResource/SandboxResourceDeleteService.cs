using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using Sepes.Infrastructure.Util.Provisioning;
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
               ICloudResourceDeleteService cloudResourceDeleteService, ICloudResourceOperationCreateService cloudResourceOperationCreateService,
           IProvisioningQueueService provisioningQueueService)
              : base(config, db, mapper, logger, userService)
        {
         
            _cloudResourceDeleteService = cloudResourceDeleteService;
            _cloudResourceOperationCreateService = cloudResourceOperationCreateService;
            _provisioningQueueService = provisioningQueueService;
        }     

        public async Task HandleSandboxDeleteAsync(int sandboxId)
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

                    _logger.LogInformation(SepesEventId.SandboxDelete, "Study {0}, Sandbox {1}: Marking resource {2} for deletion", sandboxFromDb.StudyId, sandboxId, curResource.Id);

                    await _cloudResourceDeleteService.MarkAsDeletedAsync(curResource.Id);                
                }

                if (sandboxResourceGroup == null)
                {
                    throw new Exception($"Unable to find ResourceGroup record in DB for Sandbox {sandboxId}, StudyId: {sandboxFromDb.StudyId}.");
                }

                _logger.LogInformation(SepesEventId.SandboxDelete, $"Creating delete operation for resource group {sandboxResourceGroup.ResourceGroupName}");

                var deleteOperation = await _cloudResourceOperationCreateService.CreateDeleteOperationAsync(sandboxResourceGroup.Id, AzureResourceUtil.CreateDescriptionForSandboxResourceOperation(sandboxResourceGroup.ResourceType,
                     CloudResourceOperationType.DELETE,
                     sandboxResourceGroup.SandboxId.Value) + ". (Delete of Sandbox resource group and all resources within)");

                _logger.LogInformation(SepesEventId.SandboxDelete, "Study {0}, Sandbox {1}: Queuing operation", sandboxFromDb.StudyId, sandboxId);

                var queueParentItem = QueueItemFactory.CreateParent(deleteOperation);              
               
                await _provisioningQueueService.SendMessageAsync(queueParentItem, visibilityTimeout: TimeSpan.FromSeconds(10));
            }
            else
            {
                _logger.LogCritical(SepesEventId.SandboxDelete, "Study {0}, Sandbox {1}: Unable to find any resources for Sandbox", sandboxFromDb.StudyId, sandboxId);
                await _db.SaveChangesAsync();
            }
        }
    }
}
