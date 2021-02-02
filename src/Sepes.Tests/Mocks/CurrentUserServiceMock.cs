using Moq;
using Sepes.Infrastructure.Interface;
using Sepes.Tests.Common.Constants;

namespace Sepes.Tests.Mocks
{
    public static class CurrentUserServiceMock
    {
        public static Mock<ICurrentUserService> GetService()
        {
            var currentUserServiceMock = new Mock<ICurrentUserService>();

            currentUserServiceMock.Setup(us => us.GetUserId()).Returns(TestUserConstants.COMMON_CUR_USER_OBJECTID);

            return currentUserServiceMock;
        }
    }
}
