namespace Sepes.RestApi.IntegrationTests.Constants
{
    public static class ApiUrls
    {
        public const string STUDIES = "api/studies";
        public const string SANDBOXES = "api/studies/{0}/sandboxes";
        public const string SANDBOX_DATASETS = "api/sandbox/{0}/datasets/{1}";    
        public const string VIRTUAL_MACHINES = "api/virtualmachines/{0}";

        public const string STUDY_SPECIFIC_DATASETS = "api/studies/{0}/datasets/studyspecific";

        public const string STUDY_SPECIFIC_DATASETS_RESOURCES = "api/studies/{studyId}/datasets/{datasetId}/resources";
    }
}
