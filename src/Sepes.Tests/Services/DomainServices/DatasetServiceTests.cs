using Microsoft.Extensions.DependencyInjection;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Tests.Setup;
using System.Collections.Generic;
using Xunit;

namespace Sepes.Tests.Services
{
    public class DatasetServiceTests
    {
        public ServiceCollection Services { get; private set; }
        public ServiceProvider ServiceProvider { get; protected set; }

        public DatasetServiceTests()
        {
            Services = BasicServiceCollectionFactory.GetServiceCollectionWithInMemory();
            Services.AddTransient<IStudyService, StudyService>();
            Services.AddTransient<IDatasetService, DatasetService>();
            ServiceProvider = Services.BuildServiceProvider();
        }          

        async void SeedTestDatabase(int datasetId)
        {
            var db = ServiceProvider.GetService<SepesDbContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            Dataset dataset = new Dataset()
            {
                Id = datasetId,
                Name = "TestDataset",
                Description = "For Testing",
                Location = "Norway West",
                Classification = "Internal",
                StorageAccountName = "testdataset",
                LRAId = 1337,
                DataId = 420,
                SourceSystem = "SAP",
                CountryOfOrigin = "Norway",
                StudyId = null
            };

            db.Datasets.Add(dataset);
            await db.SaveChangesAsync();
        }

        [Fact]
        public async void GetDatasetsLookupAsync_ShouldReturnDatasets_IfExists()
        {
            SeedTestDatabase(5);
            IDatasetService datasetService = ServiceProvider.GetService<IDatasetService>();

            IEnumerable<DatasetListItemDto> result = await datasetService.GetDatasetsLookupAsync();
            Assert.NotNull(result);
        }

        [Fact]
        public async void GetDatasetsAsync_ShouldReturnDatasets_IfExists()
        {
            SeedTestDatabase(5);
            IDatasetService datasetService = ServiceProvider.GetService<IDatasetService>();

            IEnumerable<DatasetDto> result = await datasetService.GetDatasetsAsync();
            Assert.NotNull(result);
        }

        [Fact]
        public async void GetDatasetByIdAsync_ShouldReturnDataset_IfExists()
        {
            SeedTestDatabase(10);
            IDatasetService datasetService = ServiceProvider.GetService<IDatasetService>();

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
            SeedTestDatabase(1);
            IDatasetService datasetService = ServiceProvider.GetService<IDatasetService>();

            System.Threading.Tasks.Task<DatasetDto> result = datasetService.GetDatasetByDatasetIdAsync(id);
            await Assert.ThrowsAsync<Sepes.Infrastructure.Exceptions.NotFoundException>(async () => await result);
        }      
    }
}
