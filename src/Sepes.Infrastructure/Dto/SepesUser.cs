using Sepes.Infrastructure.Util;
using System.Security.Claims;

namespace Sepes.Infrastructure.Dto
{
    public class SepesUser
    {
        public ClaimsPrincipal Principal { get; private set; }

        public string Oid { get; set; }

        public string UserName { get; private set; }

        public string Email { get; private set; }

        public string FullName { get; private set; }

        public SepesUser (ClaimsPrincipal principal)
        {
            Principal = principal;
            Oid = UserUtil.GetOid(Principal);
            UserName = UserUtil.GetUsername(Principal);
            Email = UserUtil.GetEmail(Principal);
            FullName = UserUtil.GetFullName(Principal);
        }        
    }
}
