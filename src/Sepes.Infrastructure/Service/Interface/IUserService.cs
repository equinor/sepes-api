using Sepes.Infrastructure.Dto;

namespace Sepes.Infrastructure.Service.Interface
{
    public interface IUserService
    {
        SepesUser GetCurrentUser();        
    }
}
