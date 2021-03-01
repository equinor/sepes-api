using Microsoft.AspNetCore.Mvc;
using Sepes.RestApi.IntegrationTests.Constants;
using Sepes.RestApi.IntegrationTests.Dto;
using Sepes.RestApi.IntegrationTests.TestHelpers;
using System;
using System.Threading.Tasks;

namespace Sepes.RestApi.IntegrationTests.RequestHelpers
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

        public static async Task<ApiConversation<Infrastructure.Dto.ErrorResponse>> DeleteAndExpectFailure(RestHelper restHelper, int sandboxId)
        {
            return await Delete<Infrastructure.Dto.ErrorResponse>(restHelper, sandboxId);
        }
    } 
}
