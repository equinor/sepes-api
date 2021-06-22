using Sepes.Common.Constants;
using Sepes.Common.Constants.Auth;
using Sepes.Common.Exceptions;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util.Auth;
using System;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class OperationPermissionService : IOperationPermissionService
    {
        readonly IUserService _userService;

        public OperationPermissionService(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }      

        public async Task<bool> HasAccessToOperation(UserOperation operation)
        {
            var currentUser = await _userService.GetCurrentUserAsync();

            var onlyRelevantOperations = AllowedUserOperations.ForOperationQueryable(operation);

            //First test this, as it's the most common operation and it requires no db access
            if (StudyAccessUtil.IsAllowedForEmployeesWithoutAnyRoles(currentUser, onlyRelevantOperations))
            {
                return true;
            }

            if (StudyAccessUtil.IsAllowedBasedOnAppRoles(currentUser, onlyRelevantOperations))
            {
                return true;
            }

            return false;
        }

        public async Task HasAccessToOperationOrThrow(UserOperation operation)
        {
            var currentUser = await _userService.GetCurrentUserAsync();

            if (!(await HasAccessToOperation(operation)))
            {
                throw new ForbiddenException($"User {currentUser.EmailAddress} does not have permission to perform operation {operation}");
            }
        }
    }
}
