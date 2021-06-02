using Sepes.Common.Constants;
using Sepes.Common.Dto;
using Sepes.Common.Exceptions;
using Sepes.Common.Interface;

namespace Sepes.Infrastructure.Util.Auth
{
    public static class AuthExceptionFactory
    {
        public static ForbiddenException CreateForbiddenException(UserDto user, IHasStudyPermissionDetails studyPermissionDetails, UserOperation operation)
        {
            return CreateForbiddenException(user.EmailAddress, studyPermissionDetails.StudyId, operation);
        }

        public static ForbiddenException CreateForbiddenException(UserDto user, int studyId, UserOperation operation)
        {
            return CreateForbiddenException(user.EmailAddress, studyId, operation);
        }

        public static ForbiddenException CreateForbiddenException(string username, int studyId, UserOperation operation)
        {
            return new ForbiddenException($"User {username} does not have permission to perform operation {operation} on study {studyId}");
        }
    }
}
