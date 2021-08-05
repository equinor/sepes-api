using Sepes.Common.Dto.Study;
using Sepes.RestApi.IntegrationTests.TestHelpers.Dto;
using System.Threading.Tasks;

namespace Sepes.RestApi.IntegrationTests.TestHelpers.Requests
{
    public static class StudyResultsAndLearningsUpdater
    {
        static async Task<ApiConversation<StudyResultsAndLearningsDto, TResponse>> Update<TResponse>(RestHelper restHelper, int studyId, StudyResultsAndLearningsDto studyResultsAndLearningsDto)         
        {         
            var response = await restHelper.Put<TResponse, StudyResultsAndLearningsDto>($"api/studies/{studyId}/resultsandlearnings", studyResultsAndLearningsDto);

            return new ApiConversation<StudyResultsAndLearningsDto, TResponse>(studyResultsAndLearningsDto, response);
        }

        public static async Task<ApiConversation<StudyResultsAndLearningsDto, StudyResultsAndLearningsDto>> UpdateAndExpectSuccess(RestHelper restHelper, int studyId, StudyResultsAndLearningsDto studyResultsAndLearningsDto)
        {
            return await Update<StudyResultsAndLearningsDto>(restHelper, studyId, studyResultsAndLearningsDto);
        }

        public static async Task<ApiConversation<StudyResultsAndLearningsDto, Common.Dto.ErrorResponse>> UpdateAndExpectFailure(RestHelper restHelper, int studyId, StudyResultsAndLearningsDto studyResultsAndLearningsDto)
        {
            return await Update<Common.Dto.ErrorResponse>(restHelper, studyId, studyResultsAndLearningsDto);
        }
    } 
}
