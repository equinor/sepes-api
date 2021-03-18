using Sepes.Infrastructure.Dto.Study;
using Sepes.Tests.Setup;
using System;
using Xunit;

namespace Sepes.Tests.Services.DomainServices
{
    public class StudyUpdateServiceTest : StudyServiceTestBase
    {
        public StudyUpdateServiceTest()
            :base()
        {
           
        }     

        [Theory]
        [InlineData(null, "")]
        [InlineData("", "null")]
        [InlineData(null, "Bouvet")]
        [InlineData("", "Bouvet")]
        [InlineData("TestStudy", null)]
        [InlineData("TestStudy", "")]
        public async void UpdatingStudyDetailsWithoutRequiredFieldsShouldBeWellHandled(string name, string vendor)
        {
            await ClearTestDatabase();

            var studyCreateService = StudyServiceMockFactory.CreateService(_serviceProvider);

            var initialStudy = new StudyCreateDto()
            {
                Name = "TestStudy12345",
                Vendor = "Equinor"
            };

            var createdStudy = await studyCreateService.CreateAsync(initialStudy);            

            var studyWithoutReqFields = new StudyUpdateDto()
            {              
                Name = name,
                Vendor = vendor
            };

            var studyUpdateService = StudyServiceMockFactory.UpdateService(_serviceProvider);
            await Assert.ThrowsAsync<ArgumentException>(() => studyUpdateService.UpdateMetadataAsync(createdStudy.Id, studyWithoutReqFields));
        }
    }
}
