using Sepes.Common.Dto;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService.Interface
{
    public interface IUserModelService
    {
        Task<UserDto> GetByIdAsync(int userId);
        Task<UserDto> GetByObjectIdAsync(string objectId);
        Task TryCreate(string objectId, string userName, string emailAddress, string fullName, string createdBy);      
    }
}
