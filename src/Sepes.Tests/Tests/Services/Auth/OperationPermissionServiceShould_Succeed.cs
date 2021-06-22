using Moq;
using Sepes.Common.Constants;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Tests.Setup;
using System.Threading.Tasks;
using Xunit;

namespace Sepes.Tests.Services.DomainServices
{
    public partial class OperationPermissionServiceShould : ServiceTestBase
    {
        [Theory]     
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public async Task SucceedOn_StudyCreate_IfCorrectPermissions(bool admin, bool sponsor)
        {
            var userService = GetUserServiceMock(admin: admin, sponsor: sponsor);
            await PerformSucceedingTest(userService, UserOperation.Study_Create);
        }

        [Theory]
        [InlineData(false, true)]
        [InlineData(true, true)]

        public async Task SucceedOn_StudyDelete_IfCorrectPermissions(bool employee, bool admin)
        {
            var userService = GetUserServiceMock(employee: employee, admin: admin);
            await PerformSucceedingTest(userService, UserOperation.Study_Delete);
        }

        [Fact]

        public async Task SucceedOn_ReadPreApprovedDataset_IfCorrectPermissions()
        {
            var userService = GetUserServiceMock(employee: true);
            await PerformSucceedingTest(userService, UserOperation.PreApprovedDataset_Read);
        }


        [Theory]
        [InlineData(false, true, false)]
        [InlineData(false, false, true)]
        [InlineData(false, true, true)]
        [InlineData(true, true, false)]
        [InlineData(true, false, true)]
        [InlineData(true, true, true)]

        public async Task SucceedOn_CreateOrUpdatePreApprovedDataset_IfCorrectPermissions(bool employee, bool admin, bool datasetAdmin)
        {
            var userService = GetUserServiceMock(employee: employee, admin: admin, datasetAdmin: datasetAdmin);
            await PerformSucceedingTest(userService, UserOperation.PreApprovedDataset_Create_Update_Delete);
        }

        async Task PerformSucceedingTest(Mock<IUserService> userService, UserOperation userOperation)
        {
            var operationPermissionService = OperationPermissionServiceMockFactory.Create(userService.Object);

            var allowed = await operationPermissionService.HasAccessToOperation(userOperation);
            Assert.True(allowed);

            await operationPermissionService.HasAccessToOperationOrThrow(userOperation);
        }

    }
}
