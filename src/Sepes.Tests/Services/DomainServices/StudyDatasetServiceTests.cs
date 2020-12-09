using Microsoft.Extensions.DependencyInjection;
using Sepes.Infrastructure.Dto.Study;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Tests.Setup;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Sepes.Tests.Services
{
    public class StudyDatasetServiceTests
    {
        public ServiceCollection Services { get; private set; }
        public ServiceProvider ServiceProvider { get; protected set; }

        public StudyDatasetServiceTests()
        {
            Services = BasicServiceCollectionFactory.GetServiceCollectionWithInMemory();
            Services.AddTransient<IStudyService, StudyService>();
            Services.AddTransient<IStudyDatasetService, StudyDatasetService>();
            ServiceProvider = Services.BuildServiceProvider();
        }

        void RefreshTestDatabase()
        {
            var db = ServiceProvider.GetService<SepesDbContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }

        async Task<StudyDetailsDto> AddStudyToTestDatabase(int studyId)
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
            var datasetService = ServiceProvider.GetService<IStudyDatasetService>();

            await Assert.ThrowsAsync<NotFoundException>(() => datasetService.AddPreApprovedDatasetToStudyAsync(providedStudyId, providedDatasetId));
        }

        [Fact]
        public async void AddDatasetFromStudyAsync_ShouldAddDataset()
        {
            int datasetId = 1;
            int studyId = 1;
            SeedTestDatabase(datasetId);
            var studyDto = AddStudyToTestDatabase(studyId);
            var studyDatasetService = DatasetServiceMockFactory.GetStudyDatasetService(ServiceProvider);

            await studyDatasetService.AddPreApprovedDatasetToStudyAsync(studyId, datasetId);
            var dataset = await studyDatasetService.GetDatasetByStudyIdAndDatasetIdAsync(studyId, datasetId);
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
            var datasetService = ServiceProvider.GetService<IStudyDatasetService>();

            await datasetService.AddPreApprovedDatasetToStudyAsync(studyId, datasetId);
            await Assert.ThrowsAsync<NotFoundException>(() => datasetService.RemoveDatasetFromStudyAsync(providedStudyId, providedDatasetId));
        }

        [Fact]
        public async void RemoveDatasetFromStudyAsync_ShouldRemoveDataset()
        {
            int datasetId = 1;
            int studyId = 1;
            SeedTestDatabase(datasetId);
            var studyDto = AddStudyToTestDatabase(studyId);
            var datasetService = ServiceProvider.GetService<IStudyDatasetService>();

            await datasetService.AddPreApprovedDatasetToStudyAsync(studyId, datasetId);
            await datasetService.RemoveDatasetFromStudyAsync(studyId, datasetId);
            await Assert.ThrowsAsync<NotFoundException>(() => datasetService.GetDatasetByStudyIdAndDatasetIdAsync(studyId, datasetId));
        }

        //[Theory]
        //[InlineData(null, "Norway", "Restricted")]
        //[InlineData("TestDataset", null, "Internal")]
        //[InlineData("TestDataset2", "Western Europe", null)]
        //public async void AddStudySpecificDatasetAsync_WithoutRequiredAttributes_ShouldFail(string name, string location, string classification)
        //{
        //    RefreshTestDatabase();
        //    var datasetService = ServiceProvider.GetService<IStudyDatasetService>();

        //    var createdStudy = await AddStudyToTestDatabase(1);

        //    var datasetWithoutRequiredFields = new StudySpecificDatasetDto()
        //    {
        //        Name = name,
        //        Location = location,
        //        Classification = classification
        //    };

        //    await Assert.ThrowsAsync<ArgumentException>(() => datasetService.CreateStudySpecificDatasetAsync((int)createdStudy.Id, datasetWithoutRequiredFields));
        //}
    }
}
