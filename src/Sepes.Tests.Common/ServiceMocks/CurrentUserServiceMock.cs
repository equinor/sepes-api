using Sepes.Common.Interface;
using Sepes.Tests.Common.Constants;

namespace Sepes.Tests.Common.ServiceMocks
{
    public class CurrentUserServiceMock : ICurrentUserService
    {
        public string GetUserId()
        {
            return TestUserConstants.COMMON_CUR_USER_OBJECTID;
        }      
    }
}
