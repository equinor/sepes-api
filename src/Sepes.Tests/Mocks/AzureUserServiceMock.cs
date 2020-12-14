using Moq;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Tests.Constants;

namespace Sepes.Tests.Mocks
{
    public static class AzureUserServiceMock
    {
        public static Mock<IAzureUserService> GetService()
        {
            var currentUserServiceMock = new Mock<IAzureUserService>();

            currentUserServiceMock.Setup(us => us.GetUserAsync(UserConstants.COMMON_CUR_USER_OBJECTID))
                .ReturnsAsync(new Infrastructure.Dto.Azure.AzureUserDto() {
                    DisplayName = UserConstants.COMMON_CUR_USER_FULL_NAME,
                    Mail = UserConstants.COMMON_CUR_USER_EMAIL,
                    UserPrincipalName = UserConstants.COMMON_CUR_USER_UPN 
                });

            return currentUserServiceMock;
        }
    }
}
