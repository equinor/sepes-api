//using Microsoft.Extensions.Logging;
//using Sepes.Infrastructure.Constants.CloudResource;
//using Sepes.Infrastructure.Dto;
//using Sepes.Infrastructure.Dto.Provisioning;
//using Sepes.Infrastructure.Exceptions;
//using Sepes.Infrastructure.Service;
//using Sepes.Infrastructure.Service.Azure;
//using Sepes.Infrastructure.Service.Azure.Interface;
//using Sepes.Infrastructure.Service.Interface;
//using System;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Sepes.Infrastructure.Util.Provisioning
//{
//    public static class ResourceUserRolesUtil
//    {       

//        public static async Task<ResourceProvisioningResult> HandleCreateOrUpdate(
//            CloudResourceOperationDto operation,
//            ResourceProvisioningParameters currentCrudInput,
//            IAzureRoleAssignmentService roleAssignmentService,
//            ICloudResourceReadService resourceReadService,
//            ICloudResourceUpdateService resourceUpdateService,
//            ICloudResourceOperationUpdateService operationUpdateService,
//            ILogger logger)
//        {
//            try
//            {
//                var cancellation = new CancellationTokenSource();
//                var currentRoleAssignmentTask = roleAssignmentService.AddResourceRoleAssignment(currentCrudInput.re, currentCrudInput, provisioningService, cancellation, logger);

//                while (!currentRoleAssignmentTask.IsCompleted)
//                {
//                    operation = await operationUpdateService.TouchAsync(operation.Id);

//                    if (await resourceReadService.ResourceIsDeleted(operation.Resource.Id) || operation.Status == CloudResourceOperationState.ABORTED)
//                    {
//                        logger.LogWarning(ProvisioningLogUtil.Operation(operation, $"Operation aborted, role assignment will be aborted"));
//                        cancellation.Cancel();
//                        break;
//                    }

//                    Thread.Sleep((int)TimeSpan.FromSeconds(3).TotalMilliseconds);
//                }

//                var provisioningResult = currentCrudResultTask.Result;

//                if (operation.OperationType == CloudResourceOperationType.CREATE)
//                {
//                    logger.LogInformation(ProvisioningLogUtil.Operation(operation, $"Storing resource Id and Name"));
//                    await resourceUpdateService.UpdateResourceIdAndName(operation.Resource.Id, provisioningResult.IdInTargetSystem, provisioningResult.NameInTargetSystem);
//                }

//                return provisioningResult;

//            }
//            catch (Exception ex)
//            {
//                if(ex.InnerException != null && ex.InnerException.Message.Contains("A task was canceled"))
//                {
//                    throw new ProvisioningException($"Resource provisioning (Create/update) aborted.", logAsWarning: true, innerException: ex.InnerException);
//                }
//                else
//                {
//                    throw new ProvisioningException($"Resource provisioning (Create/update) failed.", CloudResourceOperationState.FAILED, postponeQueueItemFor: 10, innerException: ex);
//                }
//            }
//        }

     
//    }
//}
