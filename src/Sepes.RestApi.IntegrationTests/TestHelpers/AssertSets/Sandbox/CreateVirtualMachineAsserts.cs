using Sepes.Infrastructure.Dto.VirtualMachine;
using Sepes.RestApi.IntegrationTests.Dto;
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
            Assert.Equal(createRequest.OperatingSystem, responseWrapper.Content.OperatingSystem);
            Assert.Equal(sandboxRegion, responseWrapper.Content.Region);//Same region as sandbox
        }
    }
}
