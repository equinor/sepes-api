using Sepes.Common.Constants;
using Sepes.Common.Exceptions;
using Sepes.Tests.Setup;
using Xunit;

namespace Sepes.Tests.Services.DomainServices
{
    public class StudyReadServiceTests : StudyServiceTestBase
    {

        [Theory]
        [InlineData(2)]
        [InlineData(7)]
        [InlineData(99999)]
        [InlineData(123456)]
        public async void GetStudyByIdAsync_WillThrow_IfStudyDoesNotExist(int id)
        {
            await ClearTestDatabase();
            var studyService = StudyServiceMockFactory.StudyEfReadService(_serviceProvider);

            await Assert.ThrowsAsync<NotFoundException>(() => studyService.GetStudyDtoByIdAsync(id, UserOperation.Study_Read));
        }       
    }
}
