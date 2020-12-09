using Sepes.Infrastructure.Dto.Dataset;
using Sepes.Infrastructure.Exceptions;
using Sepes.Tests.Setup;
using System;
using Xunit;

namespace Sepes.Tests.Services
{
    public class StudySpecificDatasetServiceTests : DatasetServiceTestBase
    {      

        public StudySpecificDatasetServiceTests()
            :base()
        {
            
        }

        [Fact]
        public async void CreateStudySpecificDataset_WhenStudyIsMissing_ShouldFail()
        {
            var datasetService = DatasetServiceMockFactory.GetStudySpecificDatasetService(_serviceProvider);
            await Assert.ThrowsAsync<NotFoundException>(() => datasetService.CreateStudySpecificDatasetAsync(1, new DatasetCreateUpdateInputBaseDto() { Name = "testds", Location = "norwayeast", Classification = "open" }));

        }


        [Fact]
        public async void CreateStudySpecificDataset_WhenStudyIsMissingWbs_ShouldFail()
        {
            var db = GetFreshTestDatabase();
            
            StudyPopulator.Add(db, "Test Study 1", "Vendor for TS1", null, 1);
            await db.SaveChangesAsync();

            var datasetService = DatasetServiceMockFactory.GetStudySpecificDatasetService(_serviceProvider);        
            await Assert.ThrowsAsync<Exception>(() => datasetService.CreateStudySpecificDatasetAsync(1, new DatasetCreateUpdateInputBaseDto() { Name = "testds", Location = "norwayeast", Classification = "open" }));         
        
        }

            
    }
}
