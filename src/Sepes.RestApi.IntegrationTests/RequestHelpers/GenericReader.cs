using Sepes.RestApi.IntegrationTests.Dto;
using Sepes.RestApi.IntegrationTests.TestHelpers;
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

        public static async Task<ApiConversation<T>> ReadAndExpectSuccess<T>(RestHelper restHelper, string url)
        {
            return await Read<T>(restHelper, url);
        }

        public static async Task<ApiConversation<Infrastructure.Dto.ErrorResponse>> ReadAndExpectFailure<T>(RestHelper restHelper, string url)
        {
            return await Read<Infrastructure.Dto.ErrorResponse>(restHelper, url);
        }

        public static string StudiesUrl() => $"api/studies/";
        public static string StudyUrl(int studyId) => $"api/studies/{studyId}";
        public static string StudyResultsAndLearningsUrl(int studyId) => $"api/studies/{studyId}/resultsandlearnings";
    } 
}
