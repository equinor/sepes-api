using Sepes.Common.Constants;
using Sepes.Tests.Setup;
using Xunit;

namespace Sepes.Tests.Services.DomainServices
{
    public class StudyServiceTests : StudyServiceTestBase
    {  
        public StudyServiceTests()
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
            var studyService = StudyServiceMockFactory.ReadService(_serviceProvider);

            await Assert.ThrowsAsync<Infrastructure.Exceptions.NotFoundException>(() => studyService.GetStudyDtoByIdAsync(id, UserOperation.Study_Read));
        }       
    }
}
