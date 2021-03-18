using Sepes.RestApi.IntegrationTests.Dto;
using Sepes.RestApi.IntegrationTests.TestHelpers;
using Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets;
using System.Threading.Tasks;

namespace Sepes.RestApi.IntegrationTests.RequestHelpers
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

     

        public static async Task<ApiConversation<Infrastructure.Dto.ErrorResponse>> ReadExpectFailure(RestHelper restHelper, string url)
        {
            return await Read<Infrastructure.Dto.ErrorResponse>(restHelper, url);
        }

        public static async Task<ApiConversation<T>> ReadAndAssertExpectSuccess<T>(RestHelper restHelper, string url)
        {
            var conversation = await Read<T>(restHelper, url);
            ApiResponseBasicAsserts.ExpectSuccess(conversation.Response);
            return conversation;

        }

        public static async Task<ApiConversation<Infrastructure.Dto.ErrorResponse>> ReadAndAssertExpectForbidden(RestHelper restHelper, string url)
        {
            var conversation = await Read<Infrastructure.Dto.ErrorResponse>(restHelper, url);
            ApiResponseBasicAsserts.ExpectForbiddenWithMessage(conversation.Response);
            return conversation;

        }

        public static string StudiesUrl() => $"api/studies/";
        public static string StudyUrl(int studyId) => $"api/studies/{studyId}";
        public static string SandboxUrl(int sandboxId) => $"api/sandboxes/{sandboxId}";

        public static string SandboxVirtualMachines(int sandboxId) => $"api/virtualmachines/forsandbox/{sandboxId}";

        public static string VirtualMachineExtendedInfo(int vmId) => $"api/virtualmachines/{vmId}/extended";    

        public static string StudyResultsAndLearningsUrl(int studyId) => $"api/studies/{studyId}/resultsandlearnings";

        public static string SandboxResources(int sandboxId) => $"api/sandboxes/{sandboxId}/resources";
    } 
}
