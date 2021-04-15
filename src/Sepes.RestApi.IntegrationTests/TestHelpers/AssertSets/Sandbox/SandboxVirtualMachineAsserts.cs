using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Dto.VirtualMachine;
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

            var sandboxResourceResponse = responseWrapper.Content;

            var index = 0;

            foreach (var curResource in sandboxResourceResponse)
            {
                Assert.NotNull(curResource.Name);
                IsRequiredType(index, curResource, expectedVmNames);
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
                IsRequiredType(index, curResource, expectedVmNames);
                Assert.Equal(CloudResourceStatus.OK, curResource.Status);
                index++;
            }
        }

        static void IsRequiredType(int index, VmDto resource, string[] expectedVms = null)
        {
            if (index < SandboxExpectedResources.Length)
            {
                Assert.Equal(SandboxExpectedResources[index], resource.Type);
            }
            else if (expectedVms != null && expectedVms.Length > 0)
            {
                Assert.Equal(AzureResourceTypeFriendlyName.VirtualMachine, resource.Type);
                Assert.Equal(expectedVms[index - SandboxExpectedResources.Length], resource.Name);
            }
        }
    }
}
