using Sepes.Infrastructure.Dto.Dataset;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Tests.Setup;
using Sepes.Tests.Setup.ModelFactory;
using System;
using System.Collections.Generic;
using Xunit;

namespace Sepes.Tests.Services
{
    public class StudySpecificDatasetServiceTests : DatasetServiceTestBase
    {      

        public StudySpecificDatasetServiceTests()
            :base()
        {
            
        }

        //Todo: Replace by integration test that actually uses SQL database
        //[Fact]
        //public async void CreateStudySpecificDataset_WhenStudyIsMissing_ShouldFail()
        //{
        //    var db = await ClearTestDatabase();           
        //    var service = DatasetServiceMockFactory.GetStudySpewcificDatasetService(_serviceProvider);
        //    await Assert.ThrowsAsync<NotFoundException>(() => service.CreateStudySpecificDatasetAsync(1, new DatasetCreateUpdateInputBaseDto() { Name = "testds", Location = "norwayeast", Classification = "open" }, "192.168.1.1"));

        //}


        [Fact]
        public async void CreateStudySpecificDataset_WhenStudyIsMissingWbs_ShouldFail()
        {
            var db = await ClearTestDatabase();
            
            StudyPopulator.Add(db, "Test Study 1", "Vendor for TS1", null, 1);
            await db.SaveChangesAsync();

            var studyWithoutWbs = StudyFactory.Create();
            studyWithoutWbs.WbsCode = null;

            var datasetService = DatasetServiceMockFactory.GetStudySpecificDatasetService(_serviceProvider, new List<Study>() { studyWithoutWbs });        
            await Assert.ThrowsAsync<Exception>(() => datasetService.CreateStudySpecificDatasetAsync(1, new DatasetCreateUpdateInputBaseDto() { Name = "testds", Location = "norwayeast", Classification = "open" }, "192.168.1.1"));         
        
        }

            
    }
}
