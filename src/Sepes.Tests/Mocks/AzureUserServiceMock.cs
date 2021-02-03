﻿using Moq;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Tests.Common.Constants;

namespace Sepes.Tests.Mocks
{
    public static class AzureUserServiceMock
    {
        public static Mock<IAzureUserService> GetService()
        {
            var currentUserServiceMock = new Mock<IAzureUserService>();

            currentUserServiceMock.Setup(us => us.GetUserAsync(TestUserConstants.COMMON_CUR_USER_OBJECTID))
                .ReturnsAsync(new Infrastructure.Dto.Azure.AzureUserDto() {
                    DisplayName = TestUserConstants.COMMON_CUR_USER_FULL_NAME,
                    Mail = TestUserConstants.COMMON_CUR_USER_EMAIL,
                    UserPrincipalName = TestUserConstants.COMMON_CUR_USER_UPN 
                });

            return currentUserServiceMock;
        }
    }
}
