using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Model;
using System.Linq;

namespace Sepes.Infrastructure.Util
{
    public static class AzureResourceStatusUtil
    {
        public static string ResourceStatus(SandboxResource resource)
        {
            if (resource.Operations == null || (resource.Operations != null && resource.Operations.Count == 0))
            {
                return "No operations found";
            }

            var lastOperation = resource.Operations.OrderByDescending(o => o.Created).FirstOrDefault();

            if (lastOperation.Status == CloudResourceOperationState.IN_PROGRESS)
            {
                return CloudResourceStatus.IN_PROGRESS;
            }
            else if (lastOperation.OperationType == CloudResourceOperationType.CREATE && string.IsNullOrWhiteSpace(lastOperation.Status))
            {
                return CloudResourceStatus.IN_QUEUE;
            }
            else if (lastOperation.Status == CloudResourceOperationState.FAILED)
            {
                if(lastOperation.TryCount < CloudResourceConstants.RESOURCE_MAX_TRY_COUNT)
                {
                    return $"{CloudResourceStatus.RETRYING} ({lastOperation.TryCount}/{CloudResourceConstants.RESOURCE_MAX_TRY_COUNT})";
                }
                else
                {
                    return $"{CloudResourceStatus.FAILED} ({lastOperation.TryCount}/{CloudResourceConstants.RESOURCE_MAX_TRY_COUNT})";
                }             
            }          
            else if (lastOperation.OperationType == CloudResourceOperationType.DELETE && lastOperation.Status == CloudResourceOperationState.DONE_SUCCESSFUL)
            {
                return CloudResourceStatus.DELETED;
            }

            if(resource.LastKnownProvisioningState == CloudResourceProvisioningStates.SUCCEEDED)
            {
                return CloudResourceStatus.OK;
            }

            return "n/a";

        }
    }
}
