using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Service.Interface;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class FunctionUserService : IUserService
    {
        UserDto _cachedUser;
        
        public FunctionUserService()
        {
            _cachedUser = new UserDto("9b0c65cf-9f14-4476-8796-b2de016e1af1", "workeruser@equinor.com", "Worker User", "workeruser@equinor.com", false, false, false);
        }

        public async Task<UserDto> GetCurrentUserAsync()
        {
            return _cachedUser;
        }


        public async Task<UserDto> GetCurrentUserWithStudyParticipantsAsync()
        {
            return await GetCurrentUserAsync();
        }

        public Task<UserDto> GetUserByIdAsync(int userId)
        {
            throw new System.NotImplementedException();
        }
    }
}
