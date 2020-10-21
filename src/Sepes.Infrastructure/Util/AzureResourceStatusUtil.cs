using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Model;
using System;
using System.Linq;

namespace Sepes.Infrastructure.Util
{
    public static class AzureResourceStatusUtil
    {     

        static bool AbleToCreateStatusForOngoingWork(SandboxResourceOperation operation, out string status)
        {
            string resourceBaseStatus = null;

            if (operation.OperationType == CloudResourceOperationType.CREATE)
            {
                resourceBaseStatus = CloudResourceStatus.CREATING;
            }
            else if (operation.OperationType == CloudResourceOperationType.UPDATE)
            {
                resourceBaseStatus = CloudResourceStatus.UPDATING;
            }
            else if (operation.OperationType == CloudResourceOperationType.DELETE)
            {
                resourceBaseStatus = CloudResourceStatus.DELETING;
            }

            if (string.IsNullOrWhiteSpace(operation.Status) || operation.Status == CloudResourceOperationState.NEW)
            {                
                status = $"{resourceBaseStatus} (queued)";     
                return true;
            }
            else if (operation.Status == CloudResourceOperationState.IN_PROGRESS)
            {
                if (operation.TryCount <= 1)
                {
                    status = resourceBaseStatus;
                    return true;
                }
                else
                {
                    status = $"{resourceBaseStatus} ({operation.TryCount}/{operation.MaxTryCount})";
                    return true;
                }
            }
            else if (operation.Status == CloudResourceOperationState.FAILED)
            {
                if (operation.OperationType == CloudResourceOperationType.CREATE)
                {
                    resourceBaseStatus = CloudResourceStatus.CREATE;
                }
                else if (operation.OperationType == CloudResourceOperationType.UPDATE)
                {
                    resourceBaseStatus = CloudResourceStatus.UPDATE;
                }
                else if (operation.OperationType == CloudResourceOperationType.DELETE)
                {
                    resourceBaseStatus = CloudResourceStatus.DELETE;
                }

                status = $"{resourceBaseStatus} {CloudResourceStatus.FAILED} ({operation.TryCount}/{operation.MaxTryCount})";
                return true;
            }

            status = null;
            return false;
        }

        public static string ResourceStatus(SandboxResource resource)
        {
            if (resource.Operations == null || (resource.Operations != null && resource.Operations.Count == 0))
            {
                return "No operations found";
            }

            var operation = resource.Operations.OrderByDescending(o => o.Created).FirstOrDefault();

            string unfinishedWorkStatus = null;
           
            if (AbleToCreateStatusForOngoingWork(operation, out unfinishedWorkStatus))
            {
                return unfinishedWorkStatus;
            }            

            if (!string.IsNullOrWhiteSpace(operation.Status))
            {
                if (operation.Status == CloudResourceOperationState.DONE_SUCCESSFUL)
                {
                    if (operation.OperationType == CloudResourceOperationType.DELETE)
                    {
                        return CloudResourceStatus.DELETED;
                    }
                    else
                    {
                        if (resource.LastKnownProvisioningState == CloudResourceProvisioningStates.SUCCEEDED)
                        {
                            return CloudResourceStatus.OK;
                        }
                    }
                }
            }

            return "n/a";     
           
        }
    }
}
