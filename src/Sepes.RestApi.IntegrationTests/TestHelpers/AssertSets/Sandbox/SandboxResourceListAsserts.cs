using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Response.Sandbox;
using Sepes.RestApi.IntegrationTests.Dto;
using System.Collections.Generic;
using Xunit;

namespace Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets.Sandbox
{
    public static class SandboxResourceListAsserts
    {
        public static void HappyPathAssert(ApiResponseWrapper<List<SandboxResourceLight>> responseWrapper)
        {
            ApiResponseBasicAsserts.HappyPathAssert<List<SandboxResourceLight>>(responseWrapper);          

            var sandboxResourceResponse = responseWrapper.Response;

            foreach (var curResource in sandboxResourceResponse)
            {
                Assert.NotNull(curResource.Name);
                Assert.Equal(CloudResourceProvisioningStates.SUCCEEDED, curResource.LastKnownProvisioningState);
                Assert.Equal(CloudResourceStatus.OK, curResource.Status);
            }
        }
    }
}
