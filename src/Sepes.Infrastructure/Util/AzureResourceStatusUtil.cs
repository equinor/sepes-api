using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Model;
using System.Linq;

namespace Sepes.Infrastructure.Util
{
    public static class AzureResourceStatusUtil
    {
        public static string ResourceStatus(SandboxResource resource)
        {
            if (resource.Operations == null || resource.Operations != null && resource.Operations.Count == 0)
            {
                return "n/a";
            }

            var lastOperation = resource.Operations.OrderByDescending(o => o.Created).FirstOrDefault();

            if (lastOperation.Status == CloudResourceOperationState.IN_PROGRESS || lastOperation.Status == CloudResourceOperationState.FAILED)
            {
                return CloudResourceStatus.IN_PROGRESS;
            }
            else if (lastOperation.OperationType == CloudResourceOperationType.CREATE && string.IsNullOrWhiteSpace(resource.LastKnownProvisioningState))
            {
                return CloudResourceStatus.IN_PROGRESS;
            }
            else if (lastOperation.OperationType == CloudResourceOperationType.DELETE && lastOperation.Status == CloudResourceOperationState.DONE_SUCCESSFUL)
            {
                return CloudResourceStatus.DELETED;
            }

            if(resource.LastKnownProvisioningState == CloudResourceProvisioningStates.SUCCEEDED)
            {
                return CloudResourceStatus.OK;
            }

            return CloudResourceStatus.FAILED;           

        }
    }
}
