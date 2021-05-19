using Sepes.Common.Dto.Study;
using Sepes.RestApi.IntegrationTests.Dto;
using Xunit;

namespace Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets
{
    public static class ReadStudyAsserts
    {
        public static void ExpectSuccess(ApiResponseWrapper<StudyDetailsDto> responseWrapper)
        {
            ApiResponseBasicAsserts.ExpectSuccess<StudyDetailsDto>(responseWrapper); 
            
            Assert.NotEqual<int>(0, responseWrapper.Content.Id);           
        }       
    }
}
