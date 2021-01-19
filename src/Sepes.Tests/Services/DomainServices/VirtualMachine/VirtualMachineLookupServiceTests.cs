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
            //await RefreshAndSeedTestDatabase(5);
            var virtualMachineLookupService = DatasetServiceMockFactory.GetVirtualMachineLookupService(_serviceProvider);
            var validationOfName = virtualMachineLookupService.CheckIfUsernameIsValidOrThrow(name);

            //IEnumerable<DatasetListItemDto> result = await datasetService.GetDatasetsLookupAsync();
            Assert.Equal(validationOfName.isValid, expectedResult);
        }

        [Theory]
        [InlineData("study1", "sandbox1", "james", "vm-study1-sandbox1-james")]
        public async void GetVirtualMachineVMNameValdiation(string studyName, string sandboxName, string prefix, string expectedResult)
        {
            //await RefreshAndSeedTestDatabase(5);
            var virtualMachineLookupService = DatasetServiceMockFactory.GetVirtualMachineLookupService(_serviceProvider);
            var calculatedName = virtualMachineLookupService.CalculateName(studyName, sandboxName, prefix);

            //IEnumerable<DatasetListItemDto> result = await datasetService.GetDatasetsLookupAsync();
            Assert.Equal(calculatedName, expectedResult);
        }
        /*

        [Theory]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(1337)]
        public async void GetDatasetByIdAsync_ShouldThrow_IfDoesNotExist(int id)
        {
            await RefreshAndSeedTestDatabase(1);
            var datasetService = DatasetServiceMockFactory.GetDatasetService(_serviceProvider);

            System.Threading.Tasks.Task<DatasetDto> result = datasetService.GetDatasetByDatasetIdAsync(id);
            await Assert.ThrowsAsync<Sepes.Infrastructure.Exceptions.NotFoundException>(async () => await result);
        }
        */
    }
}
