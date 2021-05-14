using Sepes.Common.Dto;

namespace Sepes.Common.Constants.Auth
{
    public static class UserConstants
    {
        public const string WORKER_USER_ID = "WORKER_USER";

        public const string WORKER_USER_FULL_NAME = "Worker User";

        public const string WORKER_USER_USERNAME = "workeruser@equinor.com";

        public static UserDto WORKER_USER = new UserDto(WORKER_USER_ID, WORKER_USER_USERNAME, WORKER_USER_FULL_NAME, WORKER_USER_USERNAME, true, false, false);
    }
}
