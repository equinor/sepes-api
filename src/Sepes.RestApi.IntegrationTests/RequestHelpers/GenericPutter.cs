using Microsoft.AspNetCore.Mvc;
using Sepes.RestApi.IntegrationTests.Dto;
using Sepes.RestApi.IntegrationTests.TestHelpers;
using System.Threading.Tasks;

namespace Sepes.RestApi.IntegrationTests.RequestHelpers
{
    public static class GenericPutter
    {
        static async Task<ApiConversation<TResponse>> Put<TResponse>(RestHelper restHelper, string url)         
        {           
            var response = await restHelper.Put<TResponse>(url);
            return new ApiConversation<TResponse>(response);
        }

        public static async Task<ApiConversation<NoContentResult>> PutAndExpectSuccess(RestHelper restHelper, string url)
        {
            return await Put<NoContentResult>(restHelper, url);
        }

        public static async Task<ApiConversation<T>> PutAndExpectSuccess<T>(RestHelper restHelper, string url)
        {
            return await Put<T>(restHelper, url);
        }      

        public static async Task<ApiConversation<Infrastructure.Dto.ErrorResponse>> PutAndExpectFailure(RestHelper restHelper, string url)
        {
            return await Put<Infrastructure.Dto.ErrorResponse>(restHelper, url);
        }

        public static string StudyClose(int studyId) => $"api/studies/{studyId}/close";
        public static string SandboxResourceRetry(int resourceId) => $"api/resources/{resourceId}/retry";
    
    } 
}
