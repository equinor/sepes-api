using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Service.Azure;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Util.Provisioning
{
    public static class EnsureFirewallRulesUtil
    {

        public static bool CanHandle(CloudResourceOperationDto operation)
        {
            if (operation.OperationType == CloudResourceOperationType.ENSURE_FIREWALL_RULES)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static async Task Handle(
            CloudResourceOperationDto operation,
            IHasFirewallRules networkRuleService,
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

                var rulesFromOperationState = CloudResourceConfigStringSerializer.DesiredFirewallRules(operation.DesiredState);

                var setRulesTask = networkRuleService.SetFirewallRules(operation.Resource.ResourceGroupName, operation.Resource.ResourceName, rulesFromOperationState, cancellation.Token);

                while (!setRulesTask.IsCompleted)
                {
                    operation = await operationUpdateService.TouchAsync(operation.Id);

                    if (await resourceReadService.ResourceIsDeleted(operation.Resource.Id) || operation.Status == CloudResourceOperationState.ABORTED)
                    {
                        logger.LogWarning(ProvisioningLogUtil.Operation(operation, $"Operation aborted, firewall rule assignment will be aborted"));
                        cancellation.Cancel();
                        break;
                    }

                    Thread.Sleep((int)TimeSpan.FromSeconds(3).TotalMilliseconds);
                }

                if (setRulesTask.IsCompletedSuccessfully)
                {

                }
                else
                {
                    if (setRulesTask.Exception == null)
                    {
                        throw new Exception("Firewall rule assignment task failed");
                    }
                    else
                    {
                        throw setRulesTask.Exception;
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("A task was canceled"))
                {
                    throw new ProvisioningException($"Resource provisioning (Ensure firewall assignments) aborted.", logAsWarning: true, innerException: ex.InnerException);
                }
                else
                {
                    throw new ProvisioningException($"Resource provisioning (Ensure firewall assignments) failed.", CloudResourceOperationState.FAILED, postponeQueueItemFor: 10, innerException: ex);
                }
            }
        }
    }
}
