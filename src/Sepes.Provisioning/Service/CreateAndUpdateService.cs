using Sepes.Common.Constants.CloudResource;
using Sepes.Common.Dto;
using Sepes.Common.Dto.Provisioning;
using Sepes.Common.Dto.Sandbox;
using Sepes.Common.Exceptions;
using Sepes.Common.Interface.Service;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Provisioning.Service.Interface;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Provisioning.Service
{
    public class CreateAndUpdateService : ICreateAndUpdateService
    {
        readonly IProvisioningLogService _provisioningLogService;
        readonly ICloudResourceReadService _cloudResourceReadService;
        readonly ICloudResourceUpdateService _cloudResourceUpdateService;
        readonly ICloudResourceOperationUpdateService _cloudResourceOperationUpdateService;

        public CreateAndUpdateService(IProvisioningLogService provisioningLogService, ICloudResourceReadService cloudResourceReadService,
            ICloudResourceUpdateService cloudResourceUpdateService,
            ICloudResourceOperationUpdateService cloudResourceOperationUpdateService)
        {
            _provisioningLogService = provisioningLogService ?? throw new ArgumentNullException(nameof(provisioningLogService));
            _cloudResourceReadService = cloudResourceReadService ??
                                        throw new ArgumentNullException(nameof(cloudResourceReadService));
            _cloudResourceUpdateService = cloudResourceUpdateService ??
                                          throw new ArgumentNullException(nameof(cloudResourceUpdateService));
            _cloudResourceOperationUpdateService = cloudResourceOperationUpdateService ??
                                                   throw new ArgumentNullException(
                                                       nameof(cloudResourceOperationUpdateService));
        }

        public bool CanHandle(CloudResourceOperationDto operation)
        {
            if (operation == null)
            {
                throw new ArgumentException("Cloud-Resource-Operation was null");
            }

            return (operation.OperationType == CloudResourceOperationType.CREATE || operation.OperationType == CloudResourceOperationType.UPDATE) ? true : false;          
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
                using (var cancellation = new CancellationTokenSource())
                {
                    var currentCrudResultTask = CreateProvisioningResultTask(operation, currentCrudInput, provisioningService, cancellation);

                    while (!currentCrudResultTask.IsCompleted)
                    {
                        operation = await _cloudResourceOperationUpdateService.TouchAsync(operation.Id);

                        if (await _cloudResourceReadService.ResourceIsDeleted(operation.Resource.Id) || operation.Status == CloudResourceOperationState.ABORTED || operation.Status == CloudResourceOperationState.ABANDONED)
                        {
                            _provisioningLogService.OperationWarning(queueParentItem, operation, "Operation aborted, provisioning will be aborted");
                            cancellation.Cancel();
                            break;
                        }

                        Thread.Sleep((int)TimeSpan.FromSeconds(3).TotalMilliseconds);
                    }

                    var provisioningResult = currentCrudResultTask.Result;

                    if (operation.OperationType == CloudResourceOperationType.CREATE)
                    {
                        _provisioningLogService.OperationInformation(queueParentItem, operation, $"Storing resource Id and Name");
                        await _cloudResourceUpdateService.UpdateResourceIdAndName(operation.Resource.Id, provisioningResult.IdInTargetSystem, provisioningResult.NameInTargetSystem);
                    }

                    return provisioningResult;
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("A task was canceled"))
                {
                    throw new ProvisioningException($"Resource provisioning (Create/update) aborted.", logAsWarning: true, innerException: ex.InnerException);
                }
                else
                {
                    throw new ProvisioningException($"Resource provisioning (Create/update) failed.", CloudResourceOperationState.FAILED, postponeQueueItemFor: 10, innerException: ex);
                }
            }
        }

        Task<ResourceProvisioningResult> CreateProvisioningResultTask(CloudResourceOperationDto operation, ResourceProvisioningParameters currentCrudInput, IPerformResourceProvisioning provisioningService, CancellationTokenSource cancellation)
        {
            if (operation.OperationType == CloudResourceOperationType.CREATE)
            {
                return provisioningService.EnsureCreated(currentCrudInput, cancellation.Token);
            }

            return provisioningService.Update(currentCrudInput, cancellation.Token);
        }
    }
}
