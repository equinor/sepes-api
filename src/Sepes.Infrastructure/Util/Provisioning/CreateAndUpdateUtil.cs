using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Provisioning;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Service;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Util.Provisioning
{
    public static class CreateAndUpdateUtil
    {
        public static bool WillBeHandledAsCreateOrUpdate(CloudResourceOperationDto operation)
        {
            if (operation.OperationType == CloudResourceOperationType.CREATE || operation.OperationType == CloudResourceOperationType.UPDATE)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static async Task<ResourceProvisioningResult> HandleCreateOrUpdate(
            CloudResourceOperationDto operation,
            ResourceProvisioningParameters currentCrudInput,
            IPerformResourceProvisioning provisioningService,
            ICloudResourceReadService resourceReadService,
            ICloudResourceUpdateService resourceUpdateService,
            ICloudResourceOperationReadService operationReadService,
            ILogger logger)
        {
            try
            {
                var cancellation = new CancellationTokenSource();
                var currentCrudResultTask = CreateProvisioningResultTask(operation, currentCrudInput, provisioningService, cancellation, logger);

                while (!currentCrudResultTask.IsCompleted)
                {
                    operation = await operationReadService.GetByIdAsync(operation.Id);

                    if (await resourceReadService.ResourceIsDeleted(operation.Resource.Id) || operation.Status == CloudResourceOperationState.ABORTED)
                    {
                        logger.LogWarning(ProvisioningLogUtil.Operation(operation, $"Operation aborted, provisioning will be aborted"));
                        cancellation.Cancel();
                        break;
                    }

                    Thread.Sleep((int)TimeSpan.FromSeconds(3).TotalMilliseconds);
                }

                var provisioningResult = currentCrudResultTask.Result;

                if (operation.OperationType == CloudResourceOperationType.CREATE)
                {
                    logger.LogInformation(ProvisioningLogUtil.Operation(operation, $"Storing resource Id and Name"));
                    await resourceUpdateService.UpdateResourceIdAndName(operation.Resource.Id, provisioningResult.IdInTargetSystem, provisioningResult.NameInTargetSystem);
                }

                return provisioningResult;

            }
            catch (Exception ex)
            {
                throw new ProvisioningException($"Create/Update provisioning failed", innerException: ex);
            }
        }

        static Task<ResourceProvisioningResult> CreateProvisioningResultTask(CloudResourceOperationDto operation, ResourceProvisioningParameters currentCrudInput, IPerformResourceProvisioning provisioningService, CancellationTokenSource cancellation, ILogger logger)
        {
            if (operation.OperationType == CloudResourceOperationType.CREATE)
            {
                logger.LogInformation(ProvisioningLogUtil.Operation(operation, $"Initial checks succeeded. Proceeding with CREATE operation"));
                return provisioningService.EnsureCreated(currentCrudInput, cancellation.Token);
            }
            else
            {
                logger.LogInformation(ProvisioningLogUtil.Operation(operation, $"Initial checks succeeded. Proceeding with UPDATE operation"));
                return provisioningService.Update(currentCrudInput, cancellation.Token);
            }
        }
    }
}
