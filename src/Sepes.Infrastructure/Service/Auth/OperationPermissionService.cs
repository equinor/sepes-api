using Sepes.Common.Constants;
using Sepes.Common.Constants.Auth;
using Sepes.Common.Dto;
using Sepes.Common.Exceptions;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util.Auth;
using System;
using System.Text;
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

            return HasAccessToOperation(currentUser, operation);
        }

        public async Task HasAccessToAnyOperationOrThrow(params UserOperation[] operations)
        {
            if (operations.Length == 0)
            {
                throw new ArgumentException("Must specify one or more operations");
            }

            var currentUser = await _userService.GetCurrentUserAsync();

            bool accessToAny = false;

            foreach (var curOperation in operations)
            {
                if (HasAccessToOperation(currentUser, curOperation))
                {
                    return;
                }
            }

            if (!accessToAny)
            {
                var sbOperations = new StringBuilder();

                foreach (var curOperation in operations)
                {
                    if (sbOperations.Length > 0)
                    {
                        sbOperations.Append(", ");
                    }

                    sbOperations.Append(curOperation);
                }

                throw new ForbiddenException($"User {currentUser.EmailAddress} does not have permission to perform any these operations {sbOperations}");
            }
        }

        public async Task HasAccessToOperationOrThrow(UserOperation operation)
        {
            var currentUser = await _userService.GetCurrentUserAsync();

            if (!(HasAccessToOperation(currentUser, operation)))
            {
                throw new ForbiddenException($"User {currentUser.EmailAddress} does not have permission to perform operation {operation}");
            }
        }

        bool HasAccessToOperation(UserDto user, UserOperation operation)
        {
            var onlyRelevantOperations = AllowedUserOperations.ForOperationQueryable(operation);

            //First test this, as it's the most common operation and it requires no db access
            if (StudyAccessUtil.IsAllowedForEmployeesWithoutAnyRoles(user, onlyRelevantOperations))
            {
                return true;
            }

            if (StudyAccessUtil.IsAllowedBasedOnAppRoles(user, onlyRelevantOperations))
            {
                return true;
            }

            return false;
        }
    }
}
