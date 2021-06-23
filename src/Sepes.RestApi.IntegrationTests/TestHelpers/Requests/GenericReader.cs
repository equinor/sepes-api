using Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets;
using Sepes.RestApi.IntegrationTests.TestHelpers.Dto;
using System.Threading.Tasks;

namespace Sepes.RestApi.IntegrationTests.TestHelpers.Requests
{
    public static class GenericReader
    {
        static async Task<ApiConversation<TResponse>> Read<TResponse>(RestHelper restHelper, string url)         
        {           
            var response = await restHelper.Get<TResponse>(url);

            return new ApiConversation<TResponse>(response);
        }

        public static async Task<ApiConversation<T>> ReadExpectSuccess<T>(RestHelper restHelper, string url)
        {
            return await Read<T>(restHelper, url);
        }     

        public static async Task<ApiConversation<Common.Dto.ErrorResponse>> ReadExpectFailure(RestHelper restHelper, string url)
        {
            return await Read<Common.Dto.ErrorResponse>(restHelper, url);
        }

        public static async Task<ApiConversation<T>> ReadAndAssertExpectSuccess<T>(RestHelper restHelper, string url)
        {
            var conversation = await Read<T>(restHelper, url);
            ApiResponseBasicAsserts.ExpectSuccess(conversation.Response);
            return conversation;

        }

        public static async Task<ApiConversation<Common.Dto.ErrorResponse>> ReadAndAssertExpectForbidden(RestHelper restHelper, string url)
        {
            var conversation = await Read<Common.Dto.ErrorResponse>(restHelper, url);
            ApiResponseBasicAsserts.ExpectForbiddenWithMessage(conversation.Response);
            return conversation;

        }

        public static string StudyListUrl() => $"api/studies/";
        public static string StudyDetailsUrl(int studyId) => $"api/studies/{studyId}";
        public static string StudyResultsAndLearningsUrl(int studyId) => $"api/studies/{studyId}/resultsandlearnings";

        //STUDY DATASET
        public static string StudyDatasetsUrl(int studyId) => $"api/studies/{studyId}/datasets";
        public static string StudyDatasetSpecificUrl(int studyId, int datasetId) => $"api/studies/{studyId}/datasets/{datasetId}";
        public static string StudyDatasetResourcesUrl(int studyId, int datasetId) => $"api/studies/{studyId}/datasets/{datasetId}/resources ";

        //SANDBOX
        public static string SandboxDetailsUrl(int sandboxId) => $"api/sandboxes/{sandboxId}";

        public static string SandboxResourcesUrl(int sandboxId) => $"api/sandboxes/{sandboxId}/resources";
        public static string SandboxCostAnalysisUrl(int sandboxId) => $"api/sandboxes/{sandboxId}/costanalysis";

        public static string SandboxAvailableDatasetsUrl(int sandboxId) => $"api/sandbox/{sandboxId}/availabledatasets";

       
        //VIRTUAL MACHINE


        public static string SandboxVirtualMachinesUrl(int sandboxId) => $"api/virtualmachines/forsandbox/{sandboxId}";


    
        public static string VirtualMachineExtendedInfoUrl(int vmId) => $"api/virtualmachines/{vmId}/extended";
        public static string VirtualMachineExternalLinkUrl(int vmId) => $"api/virtualmachines/{vmId}/externalLink";



        public static string VirtualMachineRulesUrl(int vmId) => $"api/virtualmachines/{vmId}/rules";



       
    }
}
