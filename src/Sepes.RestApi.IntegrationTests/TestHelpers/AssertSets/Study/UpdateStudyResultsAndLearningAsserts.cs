using Sepes.Common.Dto.Study;
using Sepes.RestApi.IntegrationTests.TestHelpers.Dto;
using Xunit;

namespace Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets
{
    public static class UpdateStudyResultsAndLearningAsserts
    {
        public static void ExpectSuccess(StudyResultsAndLearningsDto updateRequest, ApiResponseWrapper<StudyResultsAndLearningsDto> responseWrapper)
        {
            ApiResponseBasicAsserts.ExpectSuccess<StudyResultsAndLearningsDto>(responseWrapper);
            Assert.Equal(updateRequest.ResultsAndLearnings, responseWrapper.Content.ResultsAndLearnings);      
        }     
    }
}
