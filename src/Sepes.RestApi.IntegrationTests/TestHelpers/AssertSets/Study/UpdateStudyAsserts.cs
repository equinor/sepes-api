using Sepes.Common.Dto.Study;
using Sepes.RestApi.IntegrationTests.TestHelpers.Dto;
using Xunit;

namespace Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets
{
    public static class UpdateStudyAsserts
    {
        public static void ExpectSuccess(StudyUpdateDto updateRequest, ApiResponseWrapper<StudyDetailsDto> responseWrapper)
        {
            ApiResponseBasicAsserts.ExpectSuccess<StudyDetailsDto>(responseWrapper); 
            
            Assert.NotEqual<int>(0, responseWrapper.Content.Id);
            Assert.Equal(updateRequest.Name, responseWrapper.Content.Name);
            Assert.Equal(updateRequest.Vendor, responseWrapper.Content.Vendor);
            Assert.Equal(updateRequest.WbsCode, responseWrapper.Content.WbsCode); 
        }     
    }
}
