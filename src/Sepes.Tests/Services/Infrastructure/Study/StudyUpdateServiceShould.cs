using Sepes.Common.Dto.Study;
using Sepes.Tests.Setup;
using System;
using Xunit;

namespace Sepes.Tests.Services.DomainServices
{
    public class StudyUpdateServiceShould : StudyServiceTestBase
    {
        public StudyUpdateServiceShould()
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
        public async void ThrowOnMissingRequiredFields(string name, string vendor)
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
