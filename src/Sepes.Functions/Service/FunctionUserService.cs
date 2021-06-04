using Sepes.Common.Dto;
using Sepes.Infrastructure.Service.Interface;
using System.Threading.Tasks;

namespace Sepes.Functions.Service
{
    public class FunctionUserService : IUserService
    {
        readonly UserDto _cachedUser;
        
        public FunctionUserService()
        {
            _cachedUser = new UserDto("9b0c65cf-9f14-4476-8796-b2de016e1af1", "workeruser@equinor.com", "Worker User", "workeruser@equinor.com", admin: true, false, false);
        }

        public async Task<UserDto> GetByDbIdAsync(int userId)
        {
            return await Task.FromResult(_cachedUser);
        }

        public async Task<bool> IsMockUser()
        {
            return await Task.FromResult(false);
        }

        public async Task<UserDto> GetByObjectIdAsync(string objectId)
        {
            return await Task.FromResult(_cachedUser);
        }

        public async Task<UserDto> GetCurrentUserAsync(bool includeDbId = true)
        {
            return await Task.FromResult(_cachedUser);
        }

        public async Task<UserDto> EnsureExists(UserDto user)
        {
            return await Task.FromResult(_cachedUser);
        }
    }
}
