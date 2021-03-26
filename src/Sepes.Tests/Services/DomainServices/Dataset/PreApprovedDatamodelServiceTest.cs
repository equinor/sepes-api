using Sepes.Tests.Setup;
using Xunit;

namespace Sepes.Tests.Services
{
    public class PreApprovedDatamodelServiceTest : DatasetServiceTestBase
    {    

        public PreApprovedDatamodelServiceTest()
            :base()
        {
    
        }

        [Theory]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(1337)]
        public async void GetDatasetByIdAsync_ShouldThrow_IfDoesNotExist(int id)
        {
            var datasetId = 1;

            _ = await ClearTestDatabase();
            var datasets = CreateTestDatasetList(datasetId);

            var preApprovedDatasetModelService = DatasetServiceMockFactory.GetPreApprovedDatasetModelService(_serviceProvider);
           
            await Assert.ThrowsAsync<Sepes.Infrastructure.Exceptions.NotFoundException>(async () => await preApprovedDatasetModelService.GetByIdAsync(id, Infrastructure.Constants.UserOperation.Study_Read));
        }
    }
}
