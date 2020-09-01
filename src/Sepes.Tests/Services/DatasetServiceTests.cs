using Microsoft.Extensions.DependencyInjection;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Tests.Setup;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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

        async Task<StudyDto> AddStudyToTestDatabase(int studyId)
        {
            var studyService = ServiceProvider.GetService<IStudyService>();
            StudyCreateDto study = new StudyCreateDto()
            {               
                Name = "TestStudy",
                Vendor = "Bouvet",
                WbsCode = "1234.1345afg"
            };

            return await studyService.CreateStudyAsync(study);
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

        [Theory]
        [InlineData(1337, 1)]
        [InlineData(1, 1337)]
        [InlineData(1337, 1337)]
        public async void AddDatasetToStudyAsync_ShouldThrow_IfDatasetOrStudyDoesNotExist(int providedStudyId, int providedDatasetId)
        {
            int datasetId = 1;
            int studyId = 1;
            SeedTestDatabase(datasetId);
            var studyDto = AddStudyToTestDatabase(studyId);
            var datasetService = ServiceProvider.GetService<IDatasetService>();

            await Assert.ThrowsAsync<NotFoundException>(() => datasetService.AddDatasetToStudyAsync(providedStudyId, providedDatasetId));
        }

        [Fact]
        public async void AddDatasetFromStudyAsync_ShouldAddDataset()
        {
            int datasetId = 1;
            int studyId = 1;
            SeedTestDatabase(datasetId);
            var studyDto = AddStudyToTestDatabase(studyId);
            var datasetService = ServiceProvider.GetService<IDatasetService>();

            await datasetService.AddDatasetToStudyAsync(studyId, datasetId);
            var dataset = await datasetService.GetDatasetByStudyIdAndDatasetIdAsync(studyId, datasetId);
            Assert.NotNull(dataset);
        }

        [Theory]
        [InlineData(1337, 1)]
        [InlineData(1, 1337)]
        [InlineData(1337, 1337)]
        public async void RemoveDatasetFromStudyAsync_ShouldThrow_IfDatasetOrStudyDoesNotExist(int providedStudyId, int providedDatasetId)
        {
            int datasetId = 1;
            int studyId = 1;
            SeedTestDatabase(datasetId);
            var studyDto = AddStudyToTestDatabase(studyId);
            var datasetService = ServiceProvider.GetService<IDatasetService>();

            await datasetService.AddDatasetToStudyAsync(studyId, datasetId);
            await Assert.ThrowsAsync<NotFoundException>(() => datasetService.RemoveDatasetFromStudyAsync(providedStudyId, providedDatasetId));
        }

        [Fact]
        public async void RemoveDatasetFromStudyAsync_ShouldRemoveDataset()
        {
            int datasetId = 1;
            int studyId = 1;
            SeedTestDatabase(datasetId);
            var studyDto = AddStudyToTestDatabase(studyId);
            var datasetService = ServiceProvider.GetService<IDatasetService>();

            await datasetService.AddDatasetToStudyAsync(studyId, datasetId);
            await datasetService.RemoveDatasetFromStudyAsync(studyId, datasetId);
            await Assert.ThrowsAsync<NotFoundException>(() => datasetService.GetDatasetByStudyIdAndDatasetIdAsync(studyId, datasetId));
        }

        [Theory]
        [InlineData(null, "Norway", "Restricted")]
        [InlineData("TestDataset", null, "Internal")]
        [InlineData("TestDataset2", "Western Europe", null)]
        public async void AddStudySpecificDatasetAsync_WithoutRequiredAttributes_ShouldFail(string name, string location, string classification)
        {
            RefreshTestDatabase();
            IDatasetService datasetService = ServiceProvider.GetService<IDatasetService>();

            var createdStudy = await AddStudyToTestDatabase(1);

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
