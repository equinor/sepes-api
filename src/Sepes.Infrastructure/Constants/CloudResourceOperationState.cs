namespace Sepes.Infrastructure.Constants
{  

    public static class CloudResourceOperationType
    {
        public const string CREATE = "create";
        public const string UPDATE = "update";
        public const string DELETE = "delete";        
    }

    public static class CloudResourceOperationState
    {
        public const string NOT_STARTED = "not started";
        public const string IN_PROGRESS = "in progress";
        public const string DONE_SUCCESSFUL = "success";
        public const string FAILED = "failed";
    }   
}
