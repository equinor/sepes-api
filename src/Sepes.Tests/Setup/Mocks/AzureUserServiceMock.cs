using Moq;
using Sepes.Azure.Dto;
using Sepes.Azure.Service.Interface;
using Sepes.Tests.Common.Constants;

namespace Sepes.Tests.Mocks
{
    public static class AzureUserServiceMock
    {
        public static Mock<IAzureUserService> GetService()
        {
            var currentUserServiceMock = new Mock<IAzureUserService>();

            currentUserServiceMock.Setup(us => us.GetUserAsync(UserTestConstants.COMMON_CUR_USER_OBJECTID))
                .ReturnsAsync(new AzureUserDto() {
                    DisplayName = UserTestConstants.COMMON_CUR_USER_FULL_NAME,
                    Mail = UserTestConstants.COMMON_CUR_USER_EMAIL,
                    UserPrincipalName = UserTestConstants.COMMON_CUR_USER_UPN 
                });

            return currentUserServiceMock;
        }
    }
}
