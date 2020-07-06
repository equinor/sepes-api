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
            Services.AddTransient<IStudyService, StudyService>();
            Services.AddTransient<IDatasetService, DatasetService>();
            ServiceProvider = Services.BuildServiceProvider();
        }

        void RefreshTestDatabase()
        {
            var db = ServiceProvider.GetService<SepesDbContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
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
            SeedTestDatabase(5);
            IDatasetService datasetService = ServiceProvider.GetService<IDatasetService>();

            IEnumerable<DatasetListItemDto> result = await datasetService.GetDatasetsLookupAsync();
            Assert.NotNull(result);
        }

        [Fact]
        public async void GetDatasetByIdAsync_ShouldReturnDataset_IfExists()
        {
            SeedTestDatabase(10);
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
            SeedTestDatabase(1);
            IDatasetService datasetService = ServiceProvider.GetService<IDatasetService>();

            System.Threading.Tasks.Task<DatasetDto> result = datasetService.GetDatasetByIdAsync(id);
            await Assert.ThrowsAsync<Sepes.Infrastructure.Exceptions.NotFoundException>(async () => await result);
        }

        [Theory]
        [InlineData(null, "Norway", "Restricted")]
        [InlineData("TestDataset", null, "Internal")]
        [InlineData("TestDataset2", "Western Europe", null)]
        public async void AddStudySpecificDatasetAsync_WithoutRequiredAttributes_ShouldFail(string name, string location, string classification)
        {
            RefreshTestDatabase();
            IStudyService studyService = ServiceProvider.GetService<IStudyService>();
            IDatasetService datasetService = ServiceProvider.GetService<IDatasetService>();

            var study = new StudyDto()
            {
                Name = "TestStudy",
                Vendor = "Bouvet"
            };

            var createdStudy = await studyService.CreateStudyAsync(study);

            var datasetWithoutRequiredFields = new StudySpecificDatasetDto()
            {
                Name = name,
                Location = location,
                Classification = classification
            };

            await Assert.ThrowsAsync<ArgumentException>(() => datasetService.AddStudySpecificDatasetAsync((int)createdStudy.Id, datasetWithoutRequiredFields));
        }
    }
}
