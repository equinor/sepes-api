using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Sepes.Infrastructure.Dto.Study;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Tests.Setup;
using System.Threading.Tasks;
using Xunit;

namespace Sepes.Tests.Services
{
    public class StudyDatasetServiceTests
    {
        readonly ServiceCollection _services;
        readonly ServiceProvider _serviceProvider;

        public StudyDatasetServiceTests()
        {
            _services = BasicServiceCollectionFactory.GetServiceCollectionWithInMemory();
            _serviceProvider = _services.BuildServiceProvider();
        }

        SepesDbContext GetFreshTestDatabase()
        {
            var db = _serviceProvider.GetService<SepesDbContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            return db;
        }

        async Task<StudyDetailsDto> AddStudyToTestDatabase(int studyId)
        {
            var db = GetFreshTestDatabase();

            var study = new Study()
            {   
                Id= studyId,
                Name = "TestStudy",
                Vendor = "Bouvet",
                WbsCode = "1234.1345afg"
            };
            db.Studies.Add(study);

            await db.SaveChangesAsync();

            var mapper = _serviceProvider.GetService<IMapper>();
            return mapper.Map<StudyDetailsDto>(study);
        }

        async void SeedTestDatabase(int datasetId)
        {
            var db = GetFreshTestDatabase();
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
            var studyDatasetService = DatasetServiceMockFactory.GetStudyDatasetService(_serviceProvider);

            await Assert.ThrowsAsync<NotFoundException>(() => studyDatasetService.AddPreApprovedDatasetToStudyAsync(providedStudyId, providedDatasetId));
        }

        [Fact]
        public async void AddDatasetFromStudyAsync_ShouldAddDataset()
        {
            int datasetId = 1;
            int studyId = 1;
            SeedTestDatabase(datasetId);
            var studyDto = AddStudyToTestDatabase(studyId);
            var studyDatasetService = DatasetServiceMockFactory.GetStudyDatasetService(_serviceProvider);

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
            var datasetService = DatasetServiceMockFactory.GetStudyDatasetService(_serviceProvider);

            await datasetService.AddPreApprovedDatasetToStudyAsync(studyId, datasetId);
            await Assert.ThrowsAsync<NotFoundException>(() => datasetService.RemovePreApprovedDatasetFromStudyAsync(providedStudyId, providedDatasetId));
        }

        [Fact]
        public async void RemoveDatasetFromStudyAsync_ShouldRemoveDataset()
        {
            int datasetId = 1;
            int studyId = 1;
            SeedTestDatabase(datasetId);
            var studyDto = AddStudyToTestDatabase(studyId);
            var datasetService = DatasetServiceMockFactory.GetStudyDatasetService(_serviceProvider);

            await datasetService.AddPreApprovedDatasetToStudyAsync(studyId, datasetId);
            await datasetService.RemovePreApprovedDatasetFromStudyAsync(studyId, datasetId);
            await Assert.ThrowsAsync<NotFoundException>(() => datasetService.GetDatasetByStudyIdAndDatasetIdAsync(studyId, datasetId));
        }

    }
}
