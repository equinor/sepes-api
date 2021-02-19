using Sepes.RestApi.IntegrationTests.Dto;
using System.Net;
using Xunit;

namespace Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets
{
    public static class ApiResponseBasicAsserts
    {
        public static void ExpectSuccess(ApiResponseWrapper responseWrapper)
        {
            Assert.Equal(HttpStatusCode.OK, responseWrapper.StatusCode);
        }

        public static void ExpectSuccess<T>(ApiResponseWrapper<T> responseWrapper)
        {        
            Assert.Equal(HttpStatusCode.OK, responseWrapper.StatusCode);
            Assert.NotNull(responseWrapper.Content);          
        }

        public static void ExpectFailure(ApiResponseWrapper responseWrapper, HttpStatusCode expectedStatusCode)
        {
            Assert.Equal(expectedStatusCode, responseWrapper.StatusCode);
        }

        public static void ExpectFailure(ApiResponseWrapper<Infrastructure.Dto.ErrorResponse> responseWrapper, HttpStatusCode expectedStatusCode, string messageShouldContain = null)
        {
            Assert.Equal(expectedStatusCode, responseWrapper.StatusCode);            

            if (!string.IsNullOrWhiteSpace(messageShouldContain))
            {
                Assert.Contains(messageShouldContain, responseWrapper.Content.Message);
            }
        }

        public static void ExpectForbidden(ApiResponseWrapper responseWrapper)
        {
            ExpectFailure(responseWrapper, HttpStatusCode.Forbidden);

        }

        public static void ExpectForbidden(ApiResponseWrapper<Infrastructure.Dto.ErrorResponse> responseWrapper, string messageShouldContain = null)
        {
            ExpectFailure(responseWrapper, HttpStatusCode.Forbidden, messageShouldContain);

        }
    }
}
