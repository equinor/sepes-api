using Microsoft.AspNetCore.Mvc;
using Sepes.RestApi.IntegrationTests.Dto;
using Sepes.RestApi.IntegrationTests.TestHelpers;
using Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets;
using System;
using System.Threading.Tasks;

namespace Sepes.RestApi.IntegrationTests.RequestHelpers
{
    public static class GenericDeleter
    {
        static async Task<ApiConversation<TResponse>> Delete<TResponse>(RestHelper restHelper, string url)         
        {           
            var response = await restHelper.Delete<TResponse>(url);

            return new ApiConversation<TResponse>(response);
        }

        public static async Task<ApiConversation<NoContentResult>> DeleteAndExpectSuccess(RestHelper restHelper, string url)
        {
            return await Delete<NoContentResult>(restHelper, url);
        }

        public static async Task<ApiConversation<T>> DeleteAndExpectSuccess<T>(RestHelper restHelper, string url)
        {
            return await Delete<T>(restHelper, url);
        }      

        public static async Task<ApiConversation<Infrastructure.Dto.ErrorResponse>> DeleteAndExpectFailure(RestHelper restHelper, string url)
        {
            return await Delete<Infrastructure.Dto.ErrorResponse>(restHelper, url);
        }

        public static void ExpectForbiddenWithMessage(Task<Func<ApiResponseWrapper<Infrastructure.Dto.ErrorResponse>>> apiCaller)
        {
            var result = apiCaller.Result();
            ApiResponseBasicAsserts.ExpectForbiddenWithMessage(result, "does not have permission to perform operation");
        }

        public static string StudyUrl(int studyId) => $"api/studies/{studyId}";     
    } 
}
