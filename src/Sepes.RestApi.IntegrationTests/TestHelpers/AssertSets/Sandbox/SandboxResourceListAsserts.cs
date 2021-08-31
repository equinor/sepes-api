using Sepes.Common.Constants;
using Sepes.Common.Constants.CloudResource;
using Sepes.Common.Response.Sandbox;
using Sepes.RestApi.IntegrationTests.TestHelpers.Dto;
using System.Collections.Generic;
using Xunit;

namespace Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets.Sandbox
{
    public static class SandboxResourceListAsserts
    {
        readonly static string[] SandboxExpectedResources = {

            AzureResourceTypeFriendlyName.ResourceGroup,
            AzureResourceTypeFriendlyName.StorageAccount,
            AzureResourceTypeFriendlyName.NetworkSecurityGroup,
            AzureResourceTypeFriendlyName.VirtualNetwork,
            AzureResourceTypeFriendlyName.Bastion
        };     

        public static void BeforeProvisioning(ApiResponseWrapper<List<SandboxResourceLight>> responseWrapper, params string[] expectedVmNames )
        {
            SandboxResourceAsserts(responseWrapper, false);
            VmAsserts(responseWrapper, false, false, expectedVmNames);            
        }

        public static void AfterProvisioning(ApiResponseWrapper<List<SandboxResourceLight>> responseWrapper, params string[] expectedVmNames)
        {
            SandboxResourceAsserts(responseWrapper, true);
            VmAsserts(responseWrapper, false, false, expectedVmNames);
        }

        public static void AfterProvisioning_VmDeleted(ApiResponseWrapper<List<SandboxResourceLight>> responseWrapper, bool vmDeleteFinished, params string[] expectedVmNames)
        {
            SandboxResourceAsserts(responseWrapper, true);
            VmAsserts(responseWrapper, true, vmDeleteFinished, expectedVmNames); 
        }

        public static void SandboxResourceAsserts(ApiResponseWrapper<List<SandboxResourceLight>> responseWrapper, bool createFinished)
        {
            ApiResponseBasicAsserts.ExpectSuccess<List<SandboxResourceLight>>(responseWrapper);
                 
            var index = 0;

            foreach (var curResource in responseWrapper.Content)
            {
                if (index > 4)
                    break;

                Assert.NotNull(curResource.Name);
                IsRequiredType(index, curResource);

                if (createFinished)
                {
                    Assert.Equal(CloudResourceStatus.OK, curResource.Status);
                }
                else
                {
                    Assert.Contains(CloudResourceStatus.CREATING, curResource.Status);
                    Assert.Contains(CloudResourceStatus.IN_QUEUE, curResource.Status);
                }
                
                index++;
            }
        }

        public static void VmAsserts(ApiResponseWrapper<List<SandboxResourceLight>> responseWrapper, bool vmDeleted, bool vmDeleteFinished, params string[] expectedVmNames)
        {
            ApiResponseBasicAsserts.ExpectSuccess<List<SandboxResourceLight>>(responseWrapper);

            var index = 0;

            foreach (var curResource in responseWrapper.Content)
            {
                if (index < 4)
                    continue;

                Assert.NotNull(curResource.Name);
                IsRequiredType(index, curResource, expectedVmNames);

                 if(!vmDeleted)
                {
                    Assert.Contains(CloudResourceStatus.CREATING, curResource.Status);
                    Assert.Contains(CloudResourceStatus.IN_QUEUE, curResource.Status);
                }
                 else if (!vmDeleteFinished)
                {
                    Assert.Contains(CloudResourceStatus.DELETING, curResource.Status);
                    Assert.Contains(CloudResourceStatus.IN_QUEUE, curResource.Status);
                }
                 else
                {
                    Assert.Contains(CloudResourceStatus.DELETING, curResource.Status);
                    Assert.Contains(CloudResourceStatus.IN_QUEUE, curResource.Status);
                }
                
                index++;
            }
        }

       

        static void IsRequiredType(int index, SandboxResourceLight resource, string[] expectedVms = null)
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
