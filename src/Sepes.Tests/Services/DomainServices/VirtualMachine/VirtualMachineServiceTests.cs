using Sepes.Infrastructure.Dto.VirtualMachine;
using Sepes.Tests.Setup;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Sepes.Tests.Services.DomainServices.VirtualMachine
{
    public class VirtualMachineServiceTests : VirtualMachineServiceBase
    {
        public VirtualMachineServiceTests()
           : base()
        {

        }


        [Fact]
        public async void CreateVmWithInvalidPassword()
        {
            var invalidPassword = "123";
            var virtualMachineLookupService = VirtualMachineMockFactory.GetVirtualMachineService(_serviceProvider);
            var validationOfName = virtualMachineLookupService.CreateAsync(1, new CreateVmUserInputDto { Password= invalidPassword });

            await Assert.ThrowsAsync<System.Exception>(async () => await validationOfName);
        }
        [Fact]
        public async void CreateVmShouldNotReturnNull()
        {
            var validPassword = "aA!1aasfgawsge33";
            var virtualMachineLookupService = VirtualMachineMockFactory.GetVirtualMachineService(_serviceProvider);
            var vm = await virtualMachineLookupService.CreateAsync(1, new CreateVmUserInputDto { Password = validPassword, 
                DataDisks = new List<string>(new string[] { "Disk1" }), Name= "vm-study1-sandbox1-james", 
                OperatingSystem="windows", Size="Size1", Username="james1" });

            Assert.NotNull(vm);
        }
    }
}
