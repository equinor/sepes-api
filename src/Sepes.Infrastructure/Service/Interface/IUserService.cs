using Sepes.Common.Dto;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IUserService
    {
        Task<UserDto> GetUserByIdAsync(int userId);

        Task<UserDto> GetCurrentUserAsync();            
    }
}
