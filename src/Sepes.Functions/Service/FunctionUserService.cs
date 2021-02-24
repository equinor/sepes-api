using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Service.Interface;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class FunctionUserService : IUserService
    {
        readonly UserDto _cachedUser;
        
        public FunctionUserService()
        {
            _cachedUser = new UserDto("9b0c65cf-9f14-4476-8796-b2de016e1af1", "workeruser@equinor.com", "Worker User", "workeruser@equinor.com", admin: true, false, false);
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<UserDto> GetCurrentUserAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
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
