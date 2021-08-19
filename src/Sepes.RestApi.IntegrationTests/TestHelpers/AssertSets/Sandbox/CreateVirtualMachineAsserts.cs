using Sepes.Common.Dto.VirtualMachine;
using Sepes.RestApi.IntegrationTests.TestHelpers.Dto;
using Xunit;

namespace Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets.Sandbox
{
    public static class CreateVirtualMachineAsserts
    {
        public static void ExpectSuccess(VirtualMachineCreateDto createRequest, string sandboxRegion, ApiResponseWrapper<VmDto> responseWrapper)
        {
            ApiResponseBasicAsserts.ExpectSuccess<VmDto>(responseWrapper);

            Assert.NotEqual<int>(0, responseWrapper.Content.Id);
            Assert.Contains(createRequest.Name, responseWrapper.Content.Name);        
            Assert.Equal(sandboxRegion, responseWrapper.Content.Region);//Same region as sandbox
        }
    }
}
