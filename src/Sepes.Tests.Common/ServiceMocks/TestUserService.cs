using Sepes.Infrastructure.Interface;
using Sepes.Tests.Common.Constants;

namespace Sepes.Tests.Common.ServiceMocks
{
    public class IntegrationTestUserService : ICurrentUserService
    {
        public string GetUserId()
        {
            return TestUserConstants.COMMON_CUR_USER_OBJECTID;
        }      
    }
}
