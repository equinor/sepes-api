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
        [InlineData("admin", false)]
        [InlineData("admin123", true)]
        [InlineData("admin123.", false)]
        public async void GetVirtualMachineUserNameValdiation(string name, Boolean expectedResult)
        {
            var virtualMachineLookupService = DatasetServiceMockFactory.GetVirtualMachineLookupService(_serviceProvider);
            var validationOfName = virtualMachineLookupService.CheckIfUsernameIsValidOrThrow(name);

            Assert.Equal(validationOfName.isValid, expectedResult);
        }

        [Theory]
        [InlineData("study1", "sandbox1", "james", "vm-study1-sandbox1-james")]
        public async void GetVirtualMachineVMNameValdiation(string studyName, string sandboxName, string prefix, string expectedResult)
        {
            var virtualMachineLookupService = DatasetServiceMockFactory.GetVirtualMachineLookupService(_serviceProvider);
            var calculatedName = virtualMachineLookupService.CalculateName(studyName, sandboxName, prefix);

            Assert.Equal(calculatedName, expectedResult);
        }
    }
}
