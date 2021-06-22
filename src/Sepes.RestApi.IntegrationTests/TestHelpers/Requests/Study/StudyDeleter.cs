using Microsoft.AspNetCore.Mvc;
using Sepes.RestApi.IntegrationTests.TestHelpers.Dto;
using System.Threading.Tasks;

namespace Sepes.RestApi.IntegrationTests.TestHelpers.Requests
{
    public static class StudyDeleter
    {
        static async Task<ApiConversation<TResponse>> Delete<TResponse>(RestHelper restHelper, int studyId)         
        {          
            var response = await restHelper.Delete<TResponse>($"api/studies/{studyId}");

            return new ApiConversation<TResponse>(response);
        }

        public static async Task<ApiConversation<NoContentResult>> DeleteAndExpectSuccess(RestHelper restHelper, int studyId)
        {
            return await Delete<NoContentResult>(restHelper, studyId);
        }

        public static async Task<ApiConversation<Common.Dto.ErrorResponse>> DeleteAndExpectFailure(RestHelper restHelper, int studyId)
        {
            return await Delete<Common.Dto.ErrorResponse>(restHelper, studyId);
        }
    } 
}
