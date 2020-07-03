using Microsoft.Extensions.DependencyInjection;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Tests.Setup;
using System;
using System.Collections.Generic;
using System.Text;
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
            Services.AddTransient<IDatasetService, DatasetService>();
            ServiceProvider = Services.BuildServiceProvider();
        }

        async void AddTestDataset(int id)
        {
            var db = ServiceProvider.GetService<SepesDbContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            Dataset dataset = new Dataset()
            {
                Id = id,
                Name = "TestDataset",
                Description = "For Testing",
                Location = "Norway West",
                Classification = "Internal",
                LRAId = 1337,
                DataId = 420,
                SourceSystem = "SAP",
                CountryOfOrigin = "Norway",
                StudyNo = null
            };

            db.Datasets.Add(dataset);
            await db.SaveChangesAsync();

        }

        [Fact]
        public async void GetDatasetsLookupAsync_ShouldReturnDatasets_IfExists()
        {
            AddTestDataset(5);
            IDatasetService datasetService = ServiceProvider.GetService<IDatasetService>();

            IEnumerable<DatasetListItemDto> result = await datasetService.GetDatasetsLookupAsync();
            Assert.NotNull(result);
        }

        [Fact]
        public async void GetDatasetByIdAsync_ShouldReturnDataset_IfExists()
        {
            AddTestDataset(10);
            IDatasetService datasetService = ServiceProvider.GetService<IDatasetService>();

            DatasetDto result = await datasetService.GetDatasetByIdAsync(10);
            Assert.NotNull(result);
        }

        [Theory]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(1337)]
        public async void GetDatasetByIdAsync_ShouldThrow_IfDoesNotExist(int id)
        {
            AddTestDataset(1);
            IDatasetService datasetService = ServiceProvider.GetService<IDatasetService>();

            System.Threading.Tasks.Task<DatasetDto> result = datasetService.GetDatasetByIdAsync(id);
            await Assert.ThrowsAsync<Sepes.Infrastructure.Exceptions.NotFoundException>(async () => await result);
        }

    }
}
