using Sepes.Common.Dto.Study;
using Sepes.RestApi.IntegrationTests.Dto;
using System.Net;
using Xunit;

namespace Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets
{
    public static class CreateStudyAsserts
    {
        public static void ExpectSuccess(StudyCreateDto createRequest, ApiResponseWrapper<StudyDetailsDto> responseWrapper)
        {
            ApiResponseBasicAsserts.ExpectSuccess<StudyDetailsDto>(responseWrapper); 
            
            Assert.NotEqual<int>(0, responseWrapper.Content.Id);
            Assert.Equal(createRequest.Name, responseWrapper.Content.Name);
            Assert.Equal(createRequest.Vendor, responseWrapper.Content.Vendor);
            Assert.Equal(createRequest.WbsCode, responseWrapper.Content.WbsCode); 
        }

        public static void ExpectValidationFailure(ApiResponseWrapper<Common.Dto.ErrorResponse> responseWrapper, string message)
        {
            ApiResponseBasicAsserts.ExpectFailureWithMessage(responseWrapper, HttpStatusCode.BadRequest, message);           
        }
    }
}
