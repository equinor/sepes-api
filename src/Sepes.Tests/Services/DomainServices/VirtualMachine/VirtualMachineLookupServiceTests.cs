using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.VirtualMachine;
using Sepes.Tests.Setup;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Sepes.Tests.Services.DomainServices.VirtualMachine
{
    public class VirtualMachineLookupServiceTests: VirtualMachineLookupServiceBase
    {
        public VirtualMachineLookupServiceTests()
           : base()
        {

        }


        [Theory]
        [InlineData("admin", AzureVmConstants.WINDOWS, false)]
        [InlineData("admin123", AzureVmConstants.WINDOWS, true)]
        [InlineData("admin123.", AzureVmConstants.WINDOWS, false)]
        [InlineData("user5", AzureVmConstants.WINDOWS, true)]
        [InlineData("user5", AzureVmConstants.LINUX, false)]
        public void GetVirtualMachineUserNameValdiation(string name, string operatingSystem, Boolean expectedResult)
        {
            var virtualMachineLookupService = VirtualMachineMockFactory.GetVirtualMachineLookupService(_serviceProvider);
            var validationOfName = virtualMachineLookupService.CheckIfUsernameIsValidOrThrow(new VmUsernameDto { OperativeSystemType = operatingSystem, Username=name});

            Assert.Equal(validationOfName.isValid, expectedResult);
        }

        [Theory]
        [InlineData("study1", "sandbox1", "james", "vm-study1-sandbox1-james")]
        public void GetVirtualMachineVMNameValdiation(string studyName, string sandboxName, string prefix, string expectedResult)
        {
            var virtualMachineLookupService = VirtualMachineMockFactory.GetVirtualMachineLookupService(_serviceProvider);
            var calculatedName = virtualMachineLookupService.CalculateName(studyName, sandboxName, prefix);

            Assert.Equal(calculatedName, expectedResult);
        }
    }
}
