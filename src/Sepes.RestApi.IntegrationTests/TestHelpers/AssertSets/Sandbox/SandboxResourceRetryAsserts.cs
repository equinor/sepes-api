using Sepes.Common.Response.Sandbox;
using Sepes.RestApi.IntegrationTests.TestHelpers.Dto;
using Xunit;

namespace Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets.Sandbox
{
    public static class SandboxResourceRetryAsserts
    {
        public static void ExpectSuccess(ApiResponseWrapper<SandboxResourceLight> responseWrapper, string statusShouldContain)
        {
            ApiResponseBasicAsserts.ExpectSuccess<SandboxResourceLight>(responseWrapper);          

            var sandboxResourceResponse = responseWrapper.Content;

            Assert.NotNull(sandboxResourceResponse.Name);
            Assert.Null(sandboxResourceResponse.RetryLink);
            Assert.Contains(statusShouldContain, sandboxResourceResponse.Status);          
        }
    }
}
