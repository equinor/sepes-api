using Sepes.Infrastructure.Dto.Study;
using Sepes.RestApi.IntegrationTests.Dto;
using Xunit;

namespace Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets.Study
{
    public static class CreateStudyAsserts
    {
        public static void ExpectSuccess(StudyCreateDto createRequest, ApiResponseWrapper<StudyDto> responseWrapper)
        {
            ApiResponseBasicAsserts.ExpectSuccess<StudyDto>(responseWrapper); 
            
            Assert.NotEqual<int>(0, responseWrapper.Response.Id);
            Assert.Equal(createRequest.Name, responseWrapper.Response.Name);
            Assert.Equal(createRequest.Vendor, responseWrapper.Response.Vendor);
            Assert.Equal(createRequest.WbsCode, responseWrapper.Response.WbsCode); 
        }
    }
}
