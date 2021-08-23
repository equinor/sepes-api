using Microsoft.AspNetCore.Mvc;
using Sepes.RestApi.IntegrationTests.TestHelpers.Dto;
using System.Threading.Tasks;

namespace Sepes.RestApi.IntegrationTests.TestHelpers.Requests
{
    public static class GenericPoster
    {
        static async Task<ApiConversation<TResponse>> Post<TResponse>(RestHelper restHelper, string url)         
        {           
            var response = await restHelper.Post<TResponse>(url);
            return new ApiConversation<TResponse>(response);
        }

        public static async Task<ApiConversation<NoContentResult>> PostAndExpectSuccess(RestHelper restHelper, string url)
        {
            return await Post<NoContentResult>(restHelper, url);
        }

        public static async Task<ApiConversation<T>> PostAndExpectSuccess<T>(RestHelper restHelper, string url)
        {
            return await Post<T>(restHelper, url);
        }      

        public static async Task<ApiConversation<Common.Response.ErrorResponse>> PostAndExpectFailure(RestHelper restHelper, string url)
        {
            return await Post<Common.Response.ErrorResponse>(restHelper, url);
        }       

        public static string SandboxNextPhase(int sandboxId) => $"api/sandboxes/{sandboxId}/nextPhase";     
    } 
}
