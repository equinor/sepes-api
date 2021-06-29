using Sepes.Infrastructure.Service;
using Sepes.Infrastructure.Service.Interface;

namespace Sepes.Tests.Setup
{
    public static class OperationPermissionServiceMockFactory
    {      

        public static IOperationPermissionService Create(IUserService userService = null) {                    

            return new OperationPermissionService(userService == null ? UserFactory.GetUserServiceMockForAdmin(1).Object : userService);
        }       
    }
}
