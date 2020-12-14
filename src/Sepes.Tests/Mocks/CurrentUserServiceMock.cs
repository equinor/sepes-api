using Moq;
using Sepes.Infrastructure.Interface;
using Sepes.Tests.Constants;

namespace Sepes.Tests.Mocks
{
    public static class CurrentUserServiceMock
    {
        public static Mock<ICurrentUserService> GetService()
        {
            var currentUserServiceMock = new Mock<ICurrentUserService>();

            currentUserServiceMock.Setup(us => us.GetUserId()).Returns(UserConstants.COMMON_CUR_USER_OBJECTID);

            return currentUserServiceMock;
        }
    }
}
