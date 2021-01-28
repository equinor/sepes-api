using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
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
    public static class DeleteOperationUtil
    {
        public static bool WillBeHandledAsDelete(CloudResourceOperationDto operation)
        {
            return operation.OperationType == CloudResourceOperationType.DELETE;
        }

        public static async Task<ResourceProvisioningResult> HandleDelete(
           CloudResourceOperationDto operation,
           ResourceProvisioningParameters currentCrudInput,
           IPerformResourceProvisioning provisioningService,
           ICloudResourceOperationUpdateService operationUpdateService,
           ILogger logger)
        {
            try
            {
                logger.LogInformation(ProvisioningLogUtil.Operation(operation, $"Deleting {operation.Resource.ResourceType}"));

                var deleteTask = provisioningService.Delete(currentCrudInput);

                while (!deleteTask.IsCompleted)
                {
                    operation = await operationUpdateService.TouchAsync(operation.Id);

                    Thread.Sleep((int)TimeSpan.FromSeconds(3).TotalMilliseconds);
                }

                logger.LogInformation(ProvisioningLogUtil.Operation(operation, $"Delete Operation finished"));

                return deleteTask.Result;
            }
            catch (Exception ex)
            {
                throw new ProvisioningException($"Provisioning (Delete) failed", innerException: ex);
            }
        }
    }
}
