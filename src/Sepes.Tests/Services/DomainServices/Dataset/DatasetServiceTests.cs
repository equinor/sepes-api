using Sepes.Infrastructure.Dto.Dataset;
using Sepes.Tests.Setup;
using System.Collections.Generic;
using Xunit;

namespace Sepes.Tests.Services
{
    public class DatasetServiceTests : DatasetServiceTestBase
    {
     

        public DatasetServiceTests()
            :base()
        {
    
        }          


        [Fact]
        public async void GetDatasetsLookupAsync_ShouldReturnDatasets_IfExists()
        {
           await  RefreshAndSeedTestDatabase(5);
            var datasetService = DatasetServiceMockFactory.GetDatasetService(_serviceProvider);

            IEnumerable<DatasetListItemDto> result = await datasetService.GetDatasetsLookupAsync();
            Assert.NotNull(result);
        }

        [Fact]
        public async void GetDatasetsAsync_ShouldReturnDatasets_IfExists()
        {
            await RefreshAndSeedTestDatabase(5);
            var datasetService = DatasetServiceMockFactory.GetDatasetService(_serviceProvider);

            IEnumerable<DatasetDto> result = await datasetService.GetDatasetsAsync();
            Assert.NotNull(result);
        }

        [Fact]
        public async void GetDatasetByIdAsync_ShouldReturnDataset_IfExists()
        {
            await RefreshAndSeedTestDatabase(10);
            var datasetService = DatasetServiceMockFactory.GetDatasetService(_serviceProvider);

            DatasetDto result = await datasetService.GetDatasetByDatasetIdAsync(10);
            Assert.NotNull(result);
        }

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
    }
}
