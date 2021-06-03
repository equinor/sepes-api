using Sepes.Common.Dto;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IUserService
    {
        Task<UserDto> GetByIdAsync(int userId);

        Task<UserDto> GetCurrentUserAsync();
        Task<bool> IsMockUser();
    }
}
