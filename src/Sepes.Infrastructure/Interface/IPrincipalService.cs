using System.Security.Principal;

namespace Sepes.Infrastructure.Interface
{
    public interface IPrincipalService
    {
        IPrincipal GetPrincipal();    
    }
}
