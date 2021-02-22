using Sepes.Infrastructure.Dto.Study;
using Sepes.RestApi.IntegrationTests.Dto;
using Sepes.RestApi.IntegrationTests.TestHelpers;
using System.Threading.Tasks;

namespace Sepes.RestApi.IntegrationTests.RequestHelpers
{
    public static class StudyReader
    {
        static async Task<ApiConversation<TResponse>> Read<TResponse>(RestHelper restHelper, int studyId)         
        {           
            var response = await restHelper.Get<TResponse>($"api/studies/{studyId}");

            return new ApiConversation<TResponse>(response);
        }

        public static async Task<ApiConversation<StudyDetailsDto>> ReadAndExpectSuccess(RestHelper restHelper, int studyId)
        {
            return await Read<StudyDetailsDto>(restHelper, studyId);
        }

        public static async Task<ApiConversation<Infrastructure.Dto.ErrorResponse>> ReadAndExpectFailure(RestHelper restHelper, int studyId)
        {
            return await Read<Infrastructure.Dto.ErrorResponse>(restHelper, studyId);
        }
    } 
}
