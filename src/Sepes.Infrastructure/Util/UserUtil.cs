using System.Security.Claims;
using System.Security.Principal;

namespace Sepes.Infrastructure.Util
{
    public static class UserUtil
    {
        const string EMAIL_CLAIM = "name";

        public static string GetFullName(IPrincipal principal)
        {
            var claimsPrincipal = principal as ClaimsPrincipal;

            var relevantClaim = claimsPrincipal.FindFirst(EMAIL_CLAIM);  
            
            return relevantClaim.Value;
        }

        public static string GetEmail(IPrincipal principal)
        {
            return principal.Identity.Name;
        }
    }
}
