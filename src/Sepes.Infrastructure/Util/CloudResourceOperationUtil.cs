using Sepes.Common.Constants.CloudResource;
using Sepes.Common.Dto;
using Sepes.Infrastructure.Model;
using System;
using System.Linq;

namespace Sepes.Infrastructure.Util
{
    public static class CloudResourceOperationUtil
    {     

        public static bool IsAborted(CloudResourceOperationDto operation)
        {
            return operation.Status == CloudResourceOperationState.ABORTED;
        }

        public static bool ReadyForProcessing(CloudResourceOperationDto operation)
        {
            return !IsAborted(operation);          
        }

        public static bool HasValidStateForRetry(CloudResourceOperation operation) 
        {
            return (operation.Status == CloudResourceOperationState.FAILED || operation.Status == CloudResourceOperationState.ABORTED) && operation.TryCount >= operation.MaxTryCount;
        }

        public static CloudResourceOperation GetCreateOperation(CloudResource resource)
        {
            if(resource.Operations == null)
            {
                throw new NullReferenceException($"Operation collection is null, might be missing Include");
            }

            return resource.Operations.Where(o => o.OperationType == CloudResourceOperationType.CREATE).SingleOrDefault();
        }

        public static bool HasSuccessfulCreateOperation(CloudResource resource)
        {
            var createOperation = GetCreateOperation(resource);

            if(createOperation == null)
            {
                return false;
            }

            return createOperation.Status == CloudResourceOperationState.DONE_SUCCESSFUL;
           
        }
    }
}
