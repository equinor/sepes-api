using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Dto;
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
            return IsAborted(operation) == false;          
        }

        public static CloudResourceOperation GetCreateOperation(CloudResource resource)
        {
            if(resource.Operations == null)
            {
                throw new NullReferenceException($"Operation collection is null, might be missing Include");
            }

            return resource.Operations.Where(o => o.OperationType == CloudResourceOperationType.CREATE).SingleOrDefault();
        }
    }
}
