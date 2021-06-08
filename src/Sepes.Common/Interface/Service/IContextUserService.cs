using Sepes.Common.Dto;

namespace Sepes.Common.Interface
{
    public interface IContextUserService
    {
        UserDto GetCurrentUser();

        string GetCurrentUserObjectId();   
        bool IsMockUser();
    }
}
