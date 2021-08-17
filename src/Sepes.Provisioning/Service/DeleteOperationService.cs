using Microsoft.Extensions.Logging;
using Sepes.Common.Constants.CloudResource;
using Sepes.Common.Dto;
using Sepes.Common.Dto.Provisioning;

using Sepes.Infrastructure.Service.Interface;
using System;
using System.Threading;
using System.Threading.Tasks;
using Sepes.Common.Exceptions;
using Sepes.Common.Interface.Service;
using Sepes.Provisioning.Service.Interface;
using Sepes.Common.Dto.Sandbox;

namespace Sepes.Provisioning.Service.Interface
{
    public class DeleteOperationService : IDeleteOperationService
    {
        readonly IProvisioningLogService _provisioningLogService;
        readonly ICloudResourceOperationUpdateService _cloudResourceOperationUpdateService;
        
        public DeleteOperationService(IProvisioningLogService provisioningLogService, 
          
            ICloudResourceOperationUpdateService cloudResourceOperationUpdateService)
        {
            _provisioningLogService = provisioningLogService ?? throw new ArgumentNullException(nameof(provisioningLogService));
            _cloudResourceOperationUpdateService = cloudResourceOperationUpdateService ??
                                                   throw new ArgumentNullException(
                                                       nameof(cloudResourceOperationUpdateService));
        }
        
        public bool CanHandle(CloudResourceOperationDto operation)
        {
            return operation.OperationType == CloudResourceOperationType.DELETE;
        }

        public async Task<ResourceProvisioningResult> Handle(
             ProvisioningQueueParentDto queueParentItem,
           CloudResourceOperationDto operation,
           ResourceProvisioningParameters currentCrudInput,
           IPerformResourceProvisioning provisioningService
        )
        {
            try
            {
                _provisioningLogService.OperationInformation(queueParentItem, operation, $"Deleting {operation.Resource.ResourceType}");

                var deleteTask = provisioningService.EnsureDeleted(currentCrudInput);

                while (!deleteTask.IsCompleted)
                {
                    operation = await _cloudResourceOperationUpdateService.TouchAsync(operation.Id);

                    Thread.Sleep((int)TimeSpan.FromSeconds(3).TotalMilliseconds);
                }

                _provisioningLogService.OperationInformation(queueParentItem, operation, $"Delete Operation finished");

                return deleteTask.Result;
            }
            catch (Exception ex)
            {
                throw new ProvisioningException($"Provisioning (Delete) failed", innerException: ex);
            }
        }
    }
}
