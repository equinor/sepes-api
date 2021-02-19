using Sepes.Infrastructure.Dto.Study;
using Sepes.RestApi.IntegrationTests.Dto;
using System.Net;
using Xunit;

namespace Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets.Study
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

        public static void ExpectFailureWithMessage(ApiResponseWrapper<Infrastructure.Dto.ErrorResponse> responseWrapper, HttpStatusCode statusCode, string messageShouldContain = null)
        {
            ApiResponseBasicAsserts.ExpectFailure(responseWrapper, statusCode, messageShouldContain);
        }

        public static void ExpectForbiddenWithMessage(ApiResponseWrapper<Infrastructure.Dto.ErrorResponse> responseWrapper, string messageShouldContain = null)
        {
            ExpectFailureWithMessage(responseWrapper, HttpStatusCode.Forbidden, messageShouldContain);
        }
    }
}
