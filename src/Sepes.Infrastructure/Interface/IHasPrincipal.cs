using System.Security.Principal;

namespace Sepes.Infrastructure.Interface
{
    public interface IHasPrincipal
    {
        IPrincipal GetPrincipal();
    }
}
