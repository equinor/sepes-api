using Microsoft.AspNetCore.Http;
using Sepes.Infrastructure.Interface;

namespace Sepes.RestApi
{
    public class CurrentUserService : ICurrentUserService
    {
        readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetUserId()
        {
            var identity = _httpContextAccessor.HttpContext?.User?.Identity as System.Security.Claims.ClaimsIdentity;
            var userId = identity.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;//System.Security.Claims.ClaimTypes.NameIdentifier
            return userId;
        }       
    }
}
