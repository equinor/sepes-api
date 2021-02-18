using Sepes.RestApi.IntegrationTests.Dto;
using Xunit;

namespace Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets
{
    public static class ApiResponseBasicAsserts
    {
        public static void HappyPathAssert<T>(ApiResponseWrapper<T> responseWrapper)
        {
            Assert.Equal(System.Net.HttpStatusCode.OK, responseWrapper.StatusCode);
            Assert.NotNull(responseWrapper.Response);          
        }

        public static void HappyPathAssert(ApiResponseWrapper responseWrapper)
        {
            Assert.Equal(System.Net.HttpStatusCode.OK, responseWrapper.StatusCode);          
        }
    }
}
