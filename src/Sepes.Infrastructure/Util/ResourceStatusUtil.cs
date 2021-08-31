using Sepes.Common.Constants.CloudResource;
using Sepes.Infrastructure.Model;
using System;
using System.Linq;

namespace Sepes.Infrastructure.Util
{
    public static class ResourceStatusUtil
    {  

        public static CloudResourceOperation DecideWhatOperationToBaseStatusOn(CloudResource resource)
        {
            CloudResourceOperation baseStatusOnThisOperation = null;

            if(resource.Operations == null)
            {
                throw new ArgumentNullException($"AzureResourceStatusUtil - DecideWhatOperationToBaseStatusOn: Missing include on operations");
            }

            var resourceListOrdered = resource.Operations.OrderByDescending(o => o.Created);           

            foreach (var curOperation in resourceListOrdered)
            {               
                if (curOperation.Status == CloudResourceOperationState.DONE_SUCCESSFUL)
                {
                    if (baseStatusOnThisOperation == null)
                    {
                        baseStatusOnThisOperation = curOperation;
                    }

                    break;
                }
                else if (curOperation.Status == CloudResourceOperationState.FAILED)
                {
                    baseStatusOnThisOperation = curOperation;
                    break;
                }
                else if (curOperation.Status == CloudResourceOperationState.ABORTED)
                {
                    baseStatusOnThisOperation = curOperation;
                    continue;
                }
                else if (curOperation.OperationType == CloudResourceOperationType.DELETE)
                {
                    baseStatusOnThisOperation = curOperation;
                    break;
                }
                else
                {
                    baseStatusOnThisOperation = curOperation;
                }
            }

            return baseStatusOnThisOperation;
        }


        public static string ResourceStatus(CloudResource resource)
        { 
            if (resource.Operations == null || (resource.Operations != null && resource.Operations.Count == 0))
            {
                return "No operations found";
            }

            var baseStatusOnThisOperation = DecideWhatOperationToBaseStatusOn(resource);

            if (baseStatusOnThisOperation == null)
            {
                return "n/a";
            }

            if (AbleToCreateStatusForOngoingWork(baseStatusOnThisOperation, out string unfinishedWorkStatus))
            {
                return unfinishedWorkStatus;
            }

            if (baseStatusOnThisOperation.Status == CloudResourceOperationState.DONE_SUCCESSFUL && !string.IsNullOrWhiteSpace(baseStatusOnThisOperation.Status))
            {
                if (baseStatusOnThisOperation.OperationType == CloudResourceOperationType.DELETE)
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

            return "n/a";     
           
        }
        static bool AbleToCreateStatusForOngoingWork(CloudResourceOperation operation, out string status)
        {
            string resourceBaseStatus = null;

            if (operation.OperationType == CloudResourceOperationType.CREATE)
            {
                resourceBaseStatus = CloudResourceStatus.CREATING;
            }
            else if (operation.OperationType == CloudResourceOperationType.UPDATE || operation.OperationType == CloudResourceOperationType.ENSURE_ROLES)
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
            else if (operation.Status == CloudResourceOperationState.FAILED || operation.Status == CloudResourceOperationState.ABORTED)
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
    }
}
