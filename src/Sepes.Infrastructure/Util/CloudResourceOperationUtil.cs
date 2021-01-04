using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Dto;

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
    }
}
