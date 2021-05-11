using Sepes.Common.Dto.Sandbox;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Response.Sandbox;
using Sepes.RestApi.IntegrationTests.Dto;
using Xunit;

namespace Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets.Sandbox
{
    public static class SandboxDetailsAsserts
    {
        public static void NewlyCreatedExpectSuccess(SandboxCreateDto createRequest, ApiResponseWrapper<SandboxDetails> responseWrapper)
        {
            ApiResponseBasicAsserts.ExpectSuccess<SandboxDetails>(responseWrapper);            
         
            Assert.NotEqual<int>(0, responseWrapper.Content.Id);
            Assert.Equal(createRequest.Name, responseWrapper.Content.Name);
            Assert.Equal(createRequest.Region, responseWrapper.Content.Region);
        }

        public static void ReadyForPhaseShiftExpectSuccess(ApiResponseWrapper<SandboxDetails> responseWrapper)
        {
            ApiResponseBasicAsserts.ExpectSuccess<SandboxDetails>(responseWrapper);
            Assert.NotEqual<int>(0, responseWrapper.Content.Id);
            Assert.Equal(SandboxPhase.Open, responseWrapper.Content.CurrentPhase);           
        }

        public static void AfterPhaseShiftExpectSuccess(ApiResponseWrapper<SandboxDetails> responseWrapper)
        {
            ApiResponseBasicAsserts.ExpectSuccess<SandboxDetails>(responseWrapper);
            Assert.NotEqual<int>(0, responseWrapper.Content.Id);
            Assert.Equal(SandboxPhase.DataAvailable, responseWrapper.Content.CurrentPhase);          
        }
    }
}
