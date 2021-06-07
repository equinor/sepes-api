using Sepes.Common.Dto;

namespace Sepes.Common.Interface
{
    public interface IContextUserService
    {
        UserDto GetCurrentUser();

        string GetCurrentUserObjectId();

        bool IsEmployee();

        bool IsAdmin();

        bool IsSponsor();

        bool IsDatasetAdmin();     
    }
}
