namespace Sepes.Common.Constants.CloudResource
{
    public static class CloudResourceConstants
    {
        public const string MANAGED_BY_TAG_NAME = "ManagedBy";
        public const int RESOURCE_MAX_TRY_COUNT = 5; //Tries X times to create a resource
        public const int INCREASE_QUEUE_INVISIBLE_WHEN_DEPENDENT_ON_NOT_FINISHED = 15; //30 seconds
    }
}
