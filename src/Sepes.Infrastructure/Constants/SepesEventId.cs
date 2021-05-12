namespace Sepes.Infrastructure.Constants
{
    public class SepesEventId
    {
        //Monitoring events
        public const string MONITORING_GENERAL = "Monitoring-General";
        public const string MONITORING_CRITICAL = "Monitoring-Critical";
        public const string MONITORING_NO_TAG_SERVICE = "Monitoring-TagService-NotFound";
        public const string MONITORING_NO_PROVISIONING_STATE_SERVICE = "Monitoring-ProvisioningService-NotFound";
        public const string MONITORING_NO_OPERATIONS = "Monitoring-NoOperations";
        public const string MONITORING_OPERATION_FROZEN = "Monitoring-FrozenOperations";
        public const string MONITORING_NO_TAGS = "Monitoring-NoTags";
        public const string MONITORING_INCORRECT_TAGS = "Monitoring-IncorrectTags";
        public const string MONITORING_MANUALLY_ADDED_TAGS = "Monitoring-ManuallyAddedTags";
        public const string MONITORING_DELETED_RESOURCE_STILL_PRESENT_IN_CLOUD = "Monitoring-NotDeletedFromCloud";
        public const string MONITORING_NEW_RESOURCE_ID_NOT_SET = "Monitoring-ResourceIdNotSetLocally";
        public const string MONITORING_NEW_RESOURCE_NAME_NOT_SET = "Monitoring-ResourceNameNotSetLocally";    
    }
}
