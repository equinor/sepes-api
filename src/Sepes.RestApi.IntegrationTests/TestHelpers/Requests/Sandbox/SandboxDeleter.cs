using Microsoft.AspNetCore.Mvc;
using Sepes.RestApi.IntegrationTests.TestHelpers.Constants;
using Sepes.RestApi.IntegrationTests.TestHelpers.Dto;
using System;
using System.Threading.Tasks;

namespace Sepes.RestApi.IntegrationTests.TestHelpers.Requests
{
    public static class SandboxDeleter
    {
        static async Task<ApiConversation<TResponse>> Delete<TResponse>(RestHelper restHelper, int sandboxId)         
        {          
            var response = await restHelper.Delete<TResponse>(String.Format(ApiUrls.SANDBOX, sandboxId));

            return new ApiConversation<TResponse>(response);
        }

        public static async Task<ApiConversation<NoContentResult>> DeleteAndExpectSuccess(RestHelper restHelper, int sandboxId)
        {
            return await Delete<NoContentResult>(restHelper, sandboxId);
        }

        public static async Task<ApiConversation<Common.Response.ErrorResponse>> DeleteAndExpectFailure(RestHelper restHelper, int sandboxId)
        {
            return await Delete<Common.Response.ErrorResponse>(restHelper, sandboxId);
        }
    } 
}
