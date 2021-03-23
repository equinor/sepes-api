using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Util.Provisioning
{
    public static class EnsureRolesUtil
    {

        public static bool WillBeHandledAsEnsureRoles(CloudResourceOperationDto operation)
        {
            if (operation.OperationType == CloudResourceOperationType.ENSURE_ROLES)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static async Task EnsureRoles(
            CloudResourceOperationDto operation,
            IAzureRoleAssignmentService roleAssignmentService,
            ICloudResourceReadService resourceReadService,
            ICloudResourceOperationUpdateService operationUpdateService,
            ILogger logger)
        {
            try
            {
                var cancellation = new CancellationTokenSource();

                if (string.IsNullOrWhiteSpace(operation.DesiredState))
                {
                    throw new NullReferenceException($"Desired state empty on operation {operation.Id}: {operation.Description}");
                }

                var desiredRolesFromOperation = CloudResourceConfigStringSerializer.DesiredRoleAssignment(operation.DesiredState);

                var currentRoleAssignmentTask = roleAssignmentService.SetRoleAssignments(operation.Resource.ResourceId, operation.Resource.ResourceName, desiredRolesFromOperation, cancellation.Token);

                while (!currentRoleAssignmentTask.IsCompleted)
                {
                    operation = await operationUpdateService.TouchAsync(operation.Id);

                    if (await resourceReadService.ResourceIsDeleted(operation.Resource.Id) || operation.Status == CloudResourceOperationState.ABORTED || operation.Status == CloudResourceOperationState.ABANDONED)
                    {
                        logger.LogWarning(ProvisioningLogUtil.Operation(operation, $"Operation aborted, role assignment will be aborted"));
                        cancellation.Cancel();
                        break;
                    }

                    Thread.Sleep((int)TimeSpan.FromSeconds(3).TotalMilliseconds);
                }

                if (currentRoleAssignmentTask.IsCompletedSuccessfully)
                {

                }
                else
                {
                    if (currentRoleAssignmentTask.Exception == null)
                    {
                        throw new Exception("Role assignment task failed");
                    }
                    else
                    {
                        throw currentRoleAssignmentTask.Exception;
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("A task was canceled"))
                {
                    throw new ProvisioningException($"Resource provisioning (Ensure role assignments) aborted.", logAsWarning: true, innerException: ex.InnerException);
                }
                else
                {
                    throw new ProvisioningException($"Resource provisioning (Ensure role assignments) failed.", CloudResourceOperationState.FAILED, postponeQueueItemFor: 10, innerException: ex);
                }
            }
        }
    }
}
