using Microsoft.AspNetCore.Mvc;
using Sepes.RestApi.IntegrationTests.Dto;
using Sepes.RestApi.IntegrationTests.TestHelpers;
using System.Threading.Tasks;

namespace Sepes.RestApi.IntegrationTests.RequestHelpers
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

        public static async Task<ApiConversation<Infrastructure.Dto.ErrorResponse>> DeleteAndExpectFailure(RestHelper restHelper, int studyId)
        {
            return await Delete<Infrastructure.Dto.ErrorResponse>(restHelper, studyId);
        }
    } 
}
