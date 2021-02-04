using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Study;
using Sepes.Tests.Setup;
using Xunit;

namespace Sepes.Tests.Services.DomainServices
{
    public class StudyDeleteServiceTests : StudyServiceTestBase
    {
      

        public StudyDeleteServiceTests()
            :base()
        {
          
        }
        
        [Theory]
        [InlineData(2)]
        [InlineData(7)]
        [InlineData(99999)]
        [InlineData(123456)]
        public async void GetStudyByIdAsync_WillThrow_IfStudyDoesNotExist(int id)
        {
            await ClearTestDatabase();
            var studyService = StudyServiceMockFactory.CreateReadService(_serviceProvider);

            await Assert.ThrowsAsync<Infrastructure.Exceptions.NotFoundException>(() => studyService.GetStudyDtoByIdAsync(id, UserOperation.Study_Read));
        }

        [Theory]
        [InlineData(2)]
        [InlineData(7)]
        [InlineData(99999)]
        [InlineData(123456)]
        public async void DeleteStudyAsync_ShouldThrow_IfStudyDoesNotExist(int id)
        {
            await ClearTestDatabase();
            var studyDeleteService = StudyServiceMockFactory.DeleteService(_serviceProvider);

            await Assert.ThrowsAsync<Infrastructure.Exceptions.NotFoundException>(() => studyDeleteService.DeleteStudyAsync(id));
        }

        [Fact]
        public async void CloseStudyAsync_ShouldClose_IfStudyExists()
        {
            await ClearTestDatabase();
            var studyDeleteService = StudyServiceMockFactory.DeleteService(_serviceProvider);

            var initialStudy = new StudyCreateDto()
            {
                Name = "TestStudy12345",
                Vendor = "Equinor"
            };

            var studyCreateUpdateService = StudyServiceMockFactory.CreateUpdateService(_serviceProvider);
            var createdStudy = await studyCreateUpdateService.CreateStudyAsync(initialStudy);
           
            await studyDeleteService.CloseStudyAsync(createdStudy.Id);
         
            var studyService = StudyServiceMockFactory.CreateReadService(_serviceProvider);
            _ = await Assert.ThrowsAsync<Infrastructure.Exceptions.NotFoundException>(() => studyService.GetStudyDtoByIdAsync(createdStudy.Id, UserOperation.Study_Read));

        }  
    }
}
