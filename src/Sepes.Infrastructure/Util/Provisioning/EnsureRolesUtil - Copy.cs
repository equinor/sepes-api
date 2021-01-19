//using Microsoft.Extensions.Logging;
//using Sepes.Infrastructure.Constants.Auth;
//using Sepes.Infrastructure.Constants.CloudResource;
//using Sepes.Infrastructure.Dto;
//using Sepes.Infrastructure.Dto.Auth;
//using Sepes.Infrastructure.Exceptions;
//using Sepes.Infrastructure.Service.Azure.Interface;
//using Sepes.Infrastructure.Service.Interface;
//using System;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Sepes.Infrastructure.Util.Provisioning
//{
//    public static class EnsureRolesUtil
//    {

//        public static bool WillBeHandledAsEnsureRoles(CloudResourceOperationDto operation)
//        {
//            if (operation.OperationType == CloudResourceOperationType.ENSURE_ROLES)
//            {
//                return true;
//            }
//            else
//            {
//                return false;
//            }
//        }

//        public static async Task EnsureRoles(
//            CloudResourceOperationDto operation,
//            IAzureRoleAssignmentService roleAssignmentService,
//            ICloudResourceReadService resourceReadService,
//            ICloudResourceRoleAssignmentUpdateService roleAssignmentUpdateService,
//            ICloudResourceOperationUpdateService operationUpdateService,
//            ILogger logger)
//        {
//            try
//            {
//                var cancellation = new CancellationTokenSource();

//                foreach (var curRole in operation.Resource.RoleAssignments)
//                {
//                    Task<AzureRoleAssignmentResponseDto> currentRoleAssignmentTask = null;

//                    var shouldExist = !(curRole.Deleted.HasValue && curRole.Deleted.Value);

//                    var createdNewOne = false;

//                    if (shouldExist)
//                    {
//                        var roleDefinitionId = AzureRoleIds.CreateUrl(operation.Resource.ResourceId, curRole.RoleId);

//                        if (String.IsNullOrWhiteSpace(curRole.ForeignSystemId))
//                        {
//                            //Probably new role assignment that has not been created before                          
//                            currentRoleAssignmentTask = roleAssignmentService.AddRoleAssignment(operation.Resource.ResourceId, roleDefinitionId, curRole.UserOjectId, cancellationToken: cancellation.Token);
//                            createdNewOne = true;
//                        }
//                        else if (await roleAssignmentService.RoleAssignmentExists(operation.Resource.ResourceId, curRole.ForeignSystemId) == false)
//                        {
//                            //role assignment should have existed, but does not                           
//                            logger.LogWarning($"Role assignment {curRole.ForeignSystemId} for resource {operation.Resource.ResourceId} should have existed");
//                            currentRoleAssignmentTask = roleAssignmentService.AddRoleAssignment(operation.Resource.ResourceId, roleDefinitionId, curRole.UserOjectId, curRole.ForeignSystemId, cancellation.Token);
//                        }
//                    }
//                    else
//                    {
//                        if (String.IsNullOrWhiteSpace(curRole.ForeignSystemId))
//                        {
//                            logger.LogWarning($"Deleted role assignment for resource {operation.Resource.ResourceId} did not have foreign system id. Cannot verify deletion");
//                        }
//                        else if (await roleAssignmentService.RoleAssignmentExists(operation.Resource.ResourceId, curRole.ForeignSystemId))
//                        {
//                            currentRoleAssignmentTask = roleAssignmentService.DeleteRoleAssignment(curRole.ForeignSystemId); //Delete should not be cancellable
//                        }
//                    }

//                    if(currentRoleAssignmentTask != null)
//                    {
//                        while (!currentRoleAssignmentTask.IsCompleted)
//                        {
//                            operation = await operationUpdateService.TouchAsync(operation.Id);

//                            if (shouldExist && await resourceReadService.ResourceIsDeleted(operation.Resource.Id) || operation.Status == CloudResourceOperationState.ABORTED)
//                            {
//                                logger.LogWarning(ProvisioningLogUtil.Operation(operation, $"Operation aborted, role assignment will be aborted"));
//                                cancellation.Cancel();
//                                break;
//                            }

//                            Thread.Sleep((int)TimeSpan.FromSeconds(3).TotalMilliseconds);
//                        }

//                        if (createdNewOne)
//                        {
//                            if (currentRoleAssignmentTask.IsCompletedSuccessfully)
//                            {
//                                if (String.IsNullOrWhiteSpace(currentRoleAssignmentTask.Result.id) == false)
//                                {
//                                    await roleAssignmentUpdateService.SetForeignIdAsync(curRole.Id, currentRoleAssignmentTask.Result.id);
//                                }
//                                else
//                                {
//                                    throw new Exception("Role assignment id was null", currentRoleAssignmentTask.Exception);
//                                }
//                            }
//                            else
//                            {
//                                if(currentRoleAssignmentTask.Exception == null)
//                                {
//                                    throw new Exception("Role assignment task failed");
//                                }
//                                else
//                                {
//                                    throw currentRoleAssignmentTask.Exception;
//                                }
                               
//                            }                         
                                                     
//                        }
//                    }  
//                }
//            }
//            catch (Exception ex)
//            {
//                if (ex.InnerException != null && ex.InnerException.Message.Contains("A task was canceled"))
//                {
//                    throw new ProvisioningException($"Resource provisioning (Ensure role assignments) aborted.", logAsWarning: true, innerException: ex.InnerException);
//                }
//                else
//                {
//                    throw new ProvisioningException($"Resource provisioning (Ensure role assignments) failed.", CloudResourceOperationState.FAILED, postponeQueueItemFor: 10, innerException: ex);
//                }
//            }
//        }
//    }
//}
