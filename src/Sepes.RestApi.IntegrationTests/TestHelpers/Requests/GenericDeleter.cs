﻿using Microsoft.AspNetCore.Mvc;
using Sepes.RestApi.IntegrationTests.TestHelpers.Dto;
using System.Threading.Tasks;

namespace Sepes.RestApi.IntegrationTests.TestHelpers.Requests
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

        public static async Task<ApiConversation<Common.Response.ErrorResponse>> DeleteAndExpectFailure(RestHelper restHelper, string url)
        {
            return await Delete<Common.Response.ErrorResponse>(restHelper, url);
        }       

        public static string StudyUrl(int studyId) => $"api/studies/{studyId}";     
    } 
}
