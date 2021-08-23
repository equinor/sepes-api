using Sepes.Common.Dto.Study;
using Sepes.RestApi.IntegrationTests.TestHelpers.Dto;
using System.Threading.Tasks;

namespace Sepes.RestApi.IntegrationTests.TestHelpers.Requests
{
    public static class StudyUpdater
    {
        static async Task<ApiConversation<StudyUpdateDto, TResponse>> Update<TResponse>(RestHelper restHelper, int studyId, StudyUpdateDto studyUpdateDto)         
        {         
            var response = await restHelper.PutAsForm<StudyUpdateDto, TResponse>($"api/studies/{studyId}/details", "study", studyUpdateDto);

            return new ApiConversation<StudyUpdateDto, TResponse>(studyUpdateDto, response);
        }

        public static async Task<ApiConversation<StudyUpdateDto, StudyDetailsDto>> UpdateAndExpectSuccess(RestHelper restHelper, int studyId, StudyUpdateDto studyUpdateDto)
        {
            return await Update<StudyDetailsDto>(restHelper, studyId, studyUpdateDto);
        }

        public static async Task<ApiConversation<StudyUpdateDto, Common.Response.ErrorResponse>> UpdateAndExpectFailure(RestHelper restHelper, int studyId, StudyUpdateDto studyUpdateDto)
        {
            return await Update<Common.Response.ErrorResponse>(restHelper, studyId, studyUpdateDto);
        }
    } 
}
