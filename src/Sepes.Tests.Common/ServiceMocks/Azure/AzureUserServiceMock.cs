using Microsoft.Graph;
using Sepes.Common.Dto.Azure;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Tests.Common.Constants;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.RestApi.IntegrationTests.Setup
{
    public class AzureUserServiceMock : IAzureUserService
    {
        public Task<AzureUserDto> GetUserAsync(string id)
        {
            var user = new AzureUserDto { UserPrincipalName = TestUserConstants.COMMON_CUR_USER_UPN, DisplayName = TestUserConstants.COMMON_CUR_USER_FULL_NAME, Mail = TestUserConstants.COMMON_CUR_USER_EMAIL };
            return Task.FromResult(user);
        }      

        public Task<List<User>> SearchUsersAsync(string search, int limit, CancellationToken cancellationToken = default)
        {
            var userList = new List<User>
            {
                new User { Id = TestUserConstants.COMMON_NEW_PARTICIPANT_OBJECTID, DisplayName = TestUserConstants.COMMON_NEW_PARTICIPANT_FULL_NAME, Mail = TestUserConstants.COMMON_NEW_PARTICIPANT_EMAIL, UserPrincipalName = TestUserConstants.COMMON_NEW_PARTICIPANT_UPN }
            };

            return Task.FromResult(userList);
        }       
    }
}
