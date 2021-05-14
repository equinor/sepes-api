using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sepes.Common.Dto;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service;
using Sepes.Tests.Mocks;
using System.Threading.Tasks;
using Xunit;

namespace Sepes.Tests.Services.DomainServices
{
    public class UserServiceTest : ServiceTestBase
    {
        public UserServiceTest()
            : base()
        {
        }

        [Theory]
        [InlineData(false, false, false, false)]
        [InlineData(true, false, false, false)]
        [InlineData(false, true, false, false)]
        [InlineData(false, false, true, false)]
        [InlineData(false, false, false, true)]
        public async Task Requesting_User_ShouldReturnUserObjectWithRelevantProperties(bool admin, bool sponsor, bool datasetAdmin, bool employee)
        {
            var userService = await GetUserServiceWithMocks(admin, sponsor, datasetAdmin, employee);

            var userWithoutStudyParticipants = await userService.GetCurrentUserAsync();
            UserDtoAsserts(userWithoutStudyParticipants, admin, sponsor, datasetAdmin, employee);

            var sameUserButWithStudyParticipants = await userService.GetCurrentUserAsync();
            UserDtoAsserts(sameUserButWithStudyParticipants, admin, sponsor, datasetAdmin, employee);

        }

        void UserDtoAsserts(UserDto user, bool admin, bool sponsor, bool datasetAdmin, bool employee)
        {
            Assert.Equal(admin, user.Admin);
            Assert.Equal(sponsor, user.Sponsor);
            Assert.Equal(datasetAdmin, user.DatasetAdmin);
            Assert.Equal(employee, user.Employee);
        }

        async Task<UserService> GetUserServiceWithMocks(bool admin = false, bool sponsor = false, bool datasetAdmin = false, bool employee = false)
        {
            await ClearTestDatabaseAddUser();

            var config = _serviceProvider.GetService<IConfiguration>();
            var db = _serviceProvider.GetService<SepesDbContext>();
            var mapper = _serviceProvider.GetService<IMapper>();

            var currentUserServiceMock = CurrentUserServiceMock.GetService();
            var principalServiceMock = PrincipalServiceMock.GetService(admin: admin, sponsor: sponsor, datasetAdmin: datasetAdmin, employee: employee);
            var azureUserServiceMock = AzureUserServiceMock.GetService();

            return new UserService(config, db, mapper, currentUserServiceMock.Object, principalServiceMock.Object, azureUserServiceMock.Object);
        }
    }
}
