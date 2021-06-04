using Sepes.Common.Dto;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IUserService
    {
        Task<UserDto> GetByDbIdAsync(int userId);

        Task<UserDto> GetByObjectIdAsync(string objectId);

        Task<UserDto> GetCurrentUserAsync(bool includeDbId = true);

        Task<UserDto> EnsureExists(UserDto user);

        Task<bool> IsMockUser();
    }
}
