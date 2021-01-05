using Sepes.Infrastructure.Dto.Study;
using Sepes.Tests.Setup;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Sepes.Tests.Services.DomainServices
{
    public class StudyCreateUpdateServiceTest : StudyServiceTestBase
    {
        public StudyCreateUpdateServiceTest()
            :base()
        {
           
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task CreatingStudyWithoutNameShouldFail(string name)
        {
            await ClearTestDatabase();

            var studyCreateUpdateService = StudyServiceMockFactory.CreateUpdateService(_serviceProvider);

            var studyWithInvalidName = new StudyCreateDto()
            {
                Name = name,
                Vendor = "Bouvet"
            };

            await Assert.ThrowsAsync<ArgumentException>(() => studyCreateUpdateService.CreateStudyAsync(studyWithInvalidName));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async void CreatingStudyWithoutVendorShouldFail(string vendor)
        {
            await ClearTestDatabase();

            var studyCreateUpdateService = StudyServiceMockFactory.CreateUpdateService(_serviceProvider);

            var studyWithInvalidVendor = new StudyCreateDto()
            {
                Name = "TestStudy",
                Vendor = vendor
            };

            await Assert.ThrowsAsync<ArgumentException>(() => studyCreateUpdateService.CreateStudyAsync(studyWithInvalidVendor));
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

            var studyCreateUpdateService = StudyServiceMockFactory.CreateUpdateService(_serviceProvider);

            var initialStudy = new StudyCreateDto()
            {
                Name = "TestStudy12345",
                Vendor = "Equinor"
            };

            var createdStudy = await studyCreateUpdateService.CreateStudyAsync(initialStudy);
            int studyId = (int)createdStudy.Id;

            var studyWithoutReqFields = new StudyDto()
            {
                Id = studyId,
                Name = name,
                Vendor = vendor
            };

            await Assert.ThrowsAsync<ArgumentException>(() => studyCreateUpdateService.UpdateStudyMetadataAsync(studyId, studyWithoutReqFields));
        }
    }
}
