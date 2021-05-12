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

        public static async Task<ApiConversation<Common.Dto.ErrorResponse>> DeleteAndExpectFailure(RestHelper restHelper, string url)
        {
            return await Delete<Common.Dto.ErrorResponse>(restHelper, url);
        }       

        public static string StudyUrl(int studyId) => $"api/studies/{studyId}";     
    } 
}
