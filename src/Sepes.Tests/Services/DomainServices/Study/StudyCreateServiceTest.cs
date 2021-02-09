using Sepes.Infrastructure.Dto.Study;
using Sepes.Tests.Setup;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Sepes.Tests.Services.DomainServices
{
    public class StudyCreateServiceTest : StudyServiceTestBase
    {
        public StudyCreateServiceTest()
            :base()
        {
           
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task CreatingStudyWithoutNameShouldFail(string name)
        {
            await ClearTestDatabase();

            var studyCreateService = StudyServiceMockFactory.CreateService(_serviceProvider);

            var studyWithInvalidName = new StudyCreateDto()
            {
                Name = name,
                Vendor = "Bouvet"
            };

            await Assert.ThrowsAsync<ArgumentException>(() => studyCreateService.CreateAsync(studyWithInvalidName));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async void CreatingStudyWithoutVendorShouldFail(string vendor)
        {
            await ClearTestDatabase();

            var studyCreateService = StudyServiceMockFactory.CreateService(_serviceProvider);

            var studyWithInvalidVendor = new StudyCreateDto()
            {
                Name = "TestStudy",
                Vendor = vendor
            };

            await Assert.ThrowsAsync<ArgumentException>(() => studyCreateService.CreateAsync(studyWithInvalidVendor));
        }     
    }
}
