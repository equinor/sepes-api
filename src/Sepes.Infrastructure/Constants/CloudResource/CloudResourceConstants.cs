namespace Sepes.Infrastructure.Constants.CloudResource
{
    public static class CloudResourceConstants
    {
        public const int RESOURCE_MAX_TRY_COUNT = 5; //Tries 3 times to create a resource
        public const int INCREASE_QUEUE_INVISIBLE_WHEN_DEPENDENT_ON_NOT_FINISHED = 30; //30 seconds
    }
}
