using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Provisioning;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Service;
using System;
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
           ILogger logger)
        {
            try
            {
                if (operation.Resource.ResourceType == AzureResourceType.ResourceGroup
                    || operation.Resource.ResourceType == AzureResourceType.VirtualMachine
                    )
                {
                    logger.LogInformation(ProvisioningLogUtil.Operation(operation, $"Deleting {operation.Resource.ResourceType}"));

                   var result = await provisioningService.Delete(currentCrudInput);

                    logger.LogInformation(ProvisioningLogUtil.Operation(operation, $"Operation finished"));

                    return result;
                }


                throw new Exception($"Resource type {operation.Resource.ResourceType} does not support delete");
            }
            catch (Exception ex)
            {
                throw new ProvisioningException($"Provisioning (Delete) failed", innerException: ex);
            }
        }
    }
}
