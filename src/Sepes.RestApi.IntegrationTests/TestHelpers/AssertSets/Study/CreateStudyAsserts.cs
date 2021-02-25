using Sepes.Infrastructure.Dto.Study;
using Sepes.RestApi.IntegrationTests.Dto;
using System.Net;
using Xunit;

namespace Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets
{
    public static class CreateStudyAsserts
    {
        public static void ExpectSuccess(StudyCreateDto createRequest, ApiResponseWrapper<StudyDto> responseWrapper)
        {
            ApiResponseBasicAsserts.ExpectSuccess<StudyDto>(responseWrapper); 
            
            Assert.NotEqual<int>(0, responseWrapper.Content.Id);
            Assert.Equal(createRequest.Name, responseWrapper.Content.Name);
            Assert.Equal(createRequest.Vendor, responseWrapper.Content.Vendor);
            Assert.Equal(createRequest.WbsCode, responseWrapper.Content.WbsCode); 
        }
   
    }
}
