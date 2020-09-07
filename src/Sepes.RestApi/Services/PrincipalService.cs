using Microsoft.AspNetCore.Http;
using Sepes.Infrastructure.Interface;
using System.Security.Principal;

namespace Sepes.RestApi.Services
{
    public class PrincipalService : IHasPrincipal
    {
        readonly IHttpContextAccessor _contextAccessor;

        public PrincipalService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public IPrincipal GetPrincipal()
        {
            return _contextAccessor.HttpContext.User;
        }
    }
}
