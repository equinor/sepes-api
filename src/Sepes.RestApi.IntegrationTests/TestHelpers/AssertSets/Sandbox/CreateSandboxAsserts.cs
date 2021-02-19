using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Response.Sandbox;
using Sepes.RestApi.IntegrationTests.Dto;
using Xunit;

namespace Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets.Sandbox
{
    public static class CreateSandboxAsserts
    {
        public static void ExpectSuccess(SandboxCreateDto createRequest, ApiResponseWrapper<SandboxDetails> responseWrapper)
        {
            ApiResponseBasicAsserts.ExpectSuccess<SandboxDetails>(responseWrapper);            
         
            Assert.NotEqual<int>(0, responseWrapper.Response.Id);
            Assert.Equal(createRequest.Name, responseWrapper.Response.Name);
            Assert.Equal(createRequest.Region, responseWrapper.Response.Region);
        }
    }
}
