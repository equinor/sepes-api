using Sepes.Common.Dto;

namespace Sepes.Common.Interface
{
    public interface IContextUserService
    {
        UserDto GetUser();

        bool IsEmployee();

        bool IsAdmin();

        bool IsSponsor();

        bool IsDatasetAdmin();    
    }
}
