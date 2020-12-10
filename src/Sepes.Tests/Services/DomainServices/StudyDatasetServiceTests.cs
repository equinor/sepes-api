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
    public class StudyDatasetServiceTests : DatasetServiceTestBase
    {     

        public StudyDatasetServiceTests()
            :base()
        {
          
        }    

        async Task<StudyDetailsDto> RefreshAndAddStudyToTestDb(int studyId, int datasetId)
        {
            var db = await RefreshAndSeedTestDatabase(datasetId);

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

           

        [Theory]
        [InlineData(1337, 1)]
        [InlineData(1, 1337)]
        [InlineData(1337, 1337)]
        public async void AddDatasetToStudyAsync_ShouldThrow_IfDatasetOrStudyDoesNotExist(int providedStudyId, int providedDatasetId)
        {           
            int studyId = 1;
            int datasetId = 1;
            await RefreshAndAddStudyToTestDb(studyId, datasetId);
          
            var studyDatasetService = DatasetServiceMockFactory.GetStudyDatasetService(_serviceProvider);

            await Assert.ThrowsAsync<NotFoundException>(() => studyDatasetService.AddPreApprovedDatasetToStudyAsync(providedStudyId, providedDatasetId));
        }

        [Fact]
        public async void AddDatasetFromStudyAsync_ShouldAddDataset()
        {
            int studyId = 1;
            int datasetId = 1;
            await RefreshAndAddStudyToTestDb(studyId, datasetId);

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
            int studyId = 1;
            int datasetId = 1;
            await RefreshAndAddStudyToTestDb(studyId, datasetId);
            var datasetService = DatasetServiceMockFactory.GetStudyDatasetService(_serviceProvider);

            await datasetService.AddPreApprovedDatasetToStudyAsync(studyId, datasetId);
            await Assert.ThrowsAsync<NotFoundException>(() => datasetService.RemovePreApprovedDatasetFromStudyAsync(providedStudyId, providedDatasetId));
        }

        [Fact]
        public async void RemoveDatasetFromStudyAsync_ShouldRemoveDataset()
        {
            int studyId = 1;
            int datasetId = 1;
            await RefreshAndAddStudyToTestDb(studyId, datasetId);
            var datasetService = DatasetServiceMockFactory.GetStudyDatasetService(_serviceProvider);

            await datasetService.AddPreApprovedDatasetToStudyAsync(studyId, datasetId);
            await datasetService.RemovePreApprovedDatasetFromStudyAsync(studyId, datasetId);
            await Assert.ThrowsAsync<NotFoundException>(() => datasetService.GetDatasetByStudyIdAndDatasetIdAsync(studyId, datasetId));
        }

    }
}
