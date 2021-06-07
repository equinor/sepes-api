using Moq;
using Sepes.Common.Interface;
using Sepes.Tests.Common.Constants;

namespace Sepes.Tests.Mocks
{
    public static class ContextUserServiceMock
    {
        public static Mock<IContextUserService> GetService(bool admin = false, bool sponsor = false, bool datasetAdmin = false, bool employee = false)
        {
            var currentUserServiceMock = new Mock<IContextUserService>();           

            currentUserServiceMock.Setup(us => us.GetCurrentUser()).Returns(new Sepes.Common.Dto.UserDto(
                TestUserConstants.COMMON_CUR_USER_OBJECTID,
                TestUserConstants.COMMON_CUR_USER_UPN,
                TestUserConstants.COMMON_CUR_USER_FULL_NAME,
                TestUserConstants.COMMON_CUR_USER_EMAIL,
                admin,
                sponsor, 
                datasetAdmin,
                employee));
            currentUserServiceMock.Setup(us => us.IsAdmin()).Returns(admin);
            currentUserServiceMock.Setup(us => us.IsSponsor()).Returns(sponsor);
            currentUserServiceMock.Setup(us => us.IsDatasetAdmin()).Returns(datasetAdmin);
            currentUserServiceMock.Setup(us => us.IsEmployee()).Returns(employee);

            return currentUserServiceMock;
        }
    }
}
