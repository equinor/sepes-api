using Sepes.Common.Constants;
using Sepes.Common.Constants.Auth;
using Sepes.Common.Dto;
using Sepes.Common.Exceptions;

namespace Sepes.Infrastructure.Util.Auth
{
    public static class OperationAccessUtil
    {
        public static void HasAccessToOperationOrThrow(UserDto currentUser, UserOperation operation)
        {
            if (!HasAccessToOperation(currentUser, operation))
            {
                throw new ForbiddenException($"User {currentUser.EmailAddress} does not have permission to perform operation {operation}");
            }
        }      

        public static bool HasAccessToOperation(UserDto currentUser, UserOperation operation)
        {
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
    }
}
