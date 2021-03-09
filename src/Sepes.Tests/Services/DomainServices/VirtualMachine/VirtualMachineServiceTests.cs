using Sepes.Infrastructure.Dto.VirtualMachine;
using Sepes.Tests.Setup;
using Xunit;

namespace Sepes.Tests.Services.DomainServices.VirtualMachine
{
    public class VirtualMachineServiceTests : ServiceTestBase
    {
        public VirtualMachineServiceTests()
           : base()
        {

        }


        [Fact]
        public async void CreateVmWithInvalidPassword()
        {
            var invalidPassword = "123";
            var virtualMachineLookupService = VirtualMachineMockServiceFactory.GetVirtualMachineCreateService(_serviceProvider);
            var validationOfName = virtualMachineLookupService.CreateAsync(1, new VirtualMachineCreateDto { Password= invalidPassword });

            await Assert.ThrowsAsync<System.Exception>(async () => await validationOfName);
        }

        [Fact]
        public async void CheckvalidPassword()
        {
            var validPassword = "!1Qwertyuiop";
            var virtualMachineLookupService = VirtualMachineMockServiceFactory.GetVirtualMachineCreateService(_serviceProvider);
            virtualMachineLookupService.ValidateVmPasswordOrThrow(validPassword);
        }
    }
}
