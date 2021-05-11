using Microsoft.Extensions.Logging;
using Sepes.Common.Constants.CloudResource;
using Sepes.Common.Dto;
using Sepes.Common.Dto.Provisioning;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Threading;
using System.Threading.Tasks;
using Sepes.Common.Exceptions;
using Sepes.Common.Interface.Service;
using Sepes.Common.Util.Provisioning;
using Sepes.Provisioning.Service.Interface;

namespace Sepes.Provisioning.Service
{
    public class CreateAndUpdateService : ICreateAndUpdateService
    {
        readonly ILogger _logger;
        readonly ICloudResourceReadService _cloudResourceReadService;
        readonly ICloudResourceUpdateService _cloudResourceUpdateService;
        readonly ICloudResourceOperationUpdateService _cloudResourceOperationUpdateService;

        public CreateAndUpdateService(ILogger<CreateAndUpdateService> logger, ICloudResourceReadService cloudResourceReadService,
            ICloudResourceUpdateService cloudResourceUpdateService,
            ICloudResourceOperationUpdateService cloudResourceOperationUpdateService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
            if (operation.OperationType == CloudResourceOperationType.CREATE || operation.OperationType == CloudResourceOperationType.UPDATE)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<ResourceProvisioningResult> Handle(
            CloudResourceOperationDto operation,
            ResourceProvisioningParameters currentCrudInput,
            IPerformResourceProvisioning provisioningService
           )
        {
            try
            {
                var cancellation = new CancellationTokenSource();
                var currentCrudResultTask = CreateProvisioningResultTask(operation, currentCrudInput, provisioningService, cancellation, _logger);

                while (!currentCrudResultTask.IsCompleted)
                {
                    operation = await _cloudResourceOperationUpdateService.TouchAsync(operation.Id);

                    if (await _cloudResourceReadService.ResourceIsDeleted(operation.Resource.Id) || operation.Status == CloudResourceOperationState.ABORTED || operation.Status == CloudResourceOperationState.ABANDONED)
                    {
                        _logger.LogWarning(ProvisioningLogUtil.Operation(operation, $"Operation aborted, provisioning will be aborted"));
                        cancellation.Cancel();
                        break;
                    }

                    Thread.Sleep((int)TimeSpan.FromSeconds(3).TotalMilliseconds);
                }

                var provisioningResult = currentCrudResultTask.Result;

                if (operation.OperationType == CloudResourceOperationType.CREATE)
                {
                    _logger.LogInformation(ProvisioningLogUtil.Operation(operation, $"Storing resource Id and Name"));
                    await _cloudResourceUpdateService.UpdateResourceIdAndName(operation.Resource.Id, provisioningResult.IdInTargetSystem, provisioningResult.NameInTargetSystem);
                }

                return provisioningResult;

            }
            catch (Exception ex)
            {
                if(ex.InnerException != null && ex.InnerException.Message.Contains("A task was canceled"))
                {
                    throw new ProvisioningException($"Resource provisioning (Create/update) aborted.", logAsWarning: true, innerException: ex.InnerException);
                }
                else
                {
                    throw new ProvisioningException($"Resource provisioning (Create/update) failed.", CloudResourceOperationState.FAILED, postponeQueueItemFor: 10, innerException: ex);
                }
            }
        }

        static Task<ResourceProvisioningResult> CreateProvisioningResultTask(CloudResourceOperationDto operation, ResourceProvisioningParameters currentCrudInput, IPerformResourceProvisioning provisioningService, CancellationTokenSource cancellation, ILogger logger)
        {
            if (operation.OperationType == CloudResourceOperationType.CREATE)
            {              
                return provisioningService.EnsureCreated(currentCrudInput, cancellation.Token);
            }
            
            return provisioningService.Update(currentCrudInput, cancellation.Token);
        }
    }
}
