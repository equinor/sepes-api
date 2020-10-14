namespace Sepes.Infrastructure.Constants.CloudResource
{
    public static class CloudResourceConstants
    {
        public const int RESOURCE_MAX_TRY_COUNT = 3; //Tries 3 times to create a resource
        public const int INCREASE_QUEUE_INVISIBLE_WHEN_DEPENDENT_ON_NOT_FINISHED = 120; //120 seconds
    }
}
