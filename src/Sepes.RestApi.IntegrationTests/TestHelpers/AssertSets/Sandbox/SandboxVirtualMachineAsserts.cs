using Sepes.Common.Constants;
using Sepes.Common.Constants.CloudResource;
using Sepes.Common.Dto.VirtualMachine;
using Sepes.Infrastructure.Response.Sandbox;
using Sepes.RestApi.IntegrationTests.Dto;
using System.Collections.Generic;
using Xunit;

namespace Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets.Sandbox
{
    public static class SandboxVirtualMachineAsserts
    {    

        public static void BeforeProvisioning(ApiResponseWrapper<List<VmDto>> responseWrapper, params string[] expectedVmNames)
        {
            ApiResponseBasicAsserts.ExpectSuccess<List<VmDto>>(responseWrapper);

            var sandboxVmResponse = responseWrapper.Content;

            var index = 0;

            foreach (var curResource in sandboxVmResponse)
            {
                Assert.NotNull(curResource.Name);
                Assert.Contains(CloudResourceStatus.CREATING, curResource.Status);
                Assert.Contains(CloudResourceStatus.IN_QUEUE, curResource.Status);

                index++;
            }
        }

        public static void AfterProvisioning(ApiResponseWrapper<List<VmDto>> responseWrapper, params string[] expectedVmNames)
        {
            ApiResponseBasicAsserts.ExpectSuccess<List<VmDto>>(responseWrapper);          

            var sandboxResourceResponse = responseWrapper.Content;

            var index = 0;

            foreach (var curResource in sandboxResourceResponse)
            {
                Assert.NotNull(curResource.Name);             
                Assert.Equal(CloudResourceStatus.OK, curResource.Status);
                index++;
            }
        }

     
    }
}
