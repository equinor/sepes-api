using Sepes.Common.Dto.VirtualMachine;
using Sepes.RestApi.IntegrationTests.TestHelpers.Dto;
using Xunit;

namespace Sepes.RestApi.IntegrationTests.TestHelpers.AssertSets.Sandbox
{
    public static class VirtualMachineExtendedInfoAsserts
    {
        public static void BeforeProvisioningExpectSuccess(ApiResponseWrapper<VmExtendedDto> responseWrapper)
        {
            ApiResponseBasicAsserts.ExpectSuccess<VmExtendedDto>(responseWrapper);

            Assert.Empty(responseWrapper.Content.Disks);
            Assert.Empty(responseWrapper.Content.NICs);
            Assert.Null(responseWrapper.Content.OsType);
            Assert.Null(responseWrapper.Content.PowerState);
            Assert.Null(responseWrapper.Content.PrivateIp);
            Assert.Null(responseWrapper.Content.PublicIp);
            Assert.Null(responseWrapper.Content.Size);
            Assert.Null(responseWrapper.Content.SizeName);


        }

        public static void AfterProvisioningExpectSuccess(ApiResponseWrapper<VmExtendedDto> responseWrapper)
        {
            ApiResponseBasicAsserts.ExpectSuccess<VmExtendedDto>(responseWrapper);

            Assert.Empty(responseWrapper.Content.Disks);
            Assert.Empty(responseWrapper.Content.NICs);
            Assert.Null(responseWrapper.Content.OsType);
            Assert.Null(responseWrapper.Content.PowerState);
            Assert.Null(responseWrapper.Content.PrivateIp);
            Assert.Null(responseWrapper.Content.PublicIp);
            Assert.Null(responseWrapper.Content.Size);
            Assert.Null(responseWrapper.Content.SizeName);

        }
    }
}
