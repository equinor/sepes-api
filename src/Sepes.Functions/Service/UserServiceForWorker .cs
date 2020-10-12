using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Service.Interface;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class UserServiceForWorker : IUserService
    {
        UserDto _cachedUser;
        
        public UserServiceForWorker()
        {
            _cachedUser = new UserDto("9b0c65cf-9f14-4476-8796-b2de016e1af1", "workeruser@equinor.com", "Worker User", "workeruser@equinor.com", false, false, false);
        }

        public UserDto GetCurrentUser()
        {
            return _cachedUser;
        }

        public async Task<UserDto> GetCurrentUserFromDbAsync()
        {
            return GetCurrentUser();
        }

        public async Task<UserDto> GetCurrentUserWithStudyParticipantsAsync()
        {
            return GetCurrentUser();
        }

        public async Task<UserPermissionDto> GetUserPermissionsAsync()
        {
            return new UserPermissionDto ();
        }
    }
}
