using Sepes.Common.Dto.Study;
using Sepes.RestApi.IntegrationTests.TestHelpers.Dto;
using System.Threading.Tasks;

namespace Sepes.RestApi.IntegrationTests.TestHelpers.Requests
{
    public static class StudyUpdater
    {
        static async Task<ApiConversation<StudyDto, TResponse>> Update<TResponse>(RestHelper restHelper, int studyId, StudyDto studyDto)         
        {         
            var response = await restHelper.PutAsForm<TResponse, StudyDto>($"api/studies/{studyId}/details", "study", studyDto);

            return new ApiConversation<StudyDto, TResponse>(studyDto, response);
        }

        public static async Task<ApiConversation<StudyDto, StudyDto>> UpdateAndExpectSuccess(RestHelper restHelper, int studyId, StudyDto studyDto)
        {
            return await Update<StudyDto>(restHelper, studyId, studyDto);
        }

        public static async Task<ApiConversation<StudyDto, Common.Dto.ErrorResponse>> UpdateAndExpectFailure(RestHelper restHelper, int studyId, StudyDto studyDto)
        {
            return await Update<Common.Dto.ErrorResponse>(restHelper, studyId, studyDto);
        }
    } 
}
