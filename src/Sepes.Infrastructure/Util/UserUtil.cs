using Sepes.Infrastructure.Dto;
using System;
using System.Security.Claims;
using System.Security.Principal;

namespace Sepes.Infrastructure.Util
{
    public static class UserUtil
    {
        //c4c0daf8-a7a3-41b8-91b3-332439661e04


        const string CLAIM_TENANT = "http://schemas.microsoft.com/identity/claims/tenantid";
        const string CLAIM_OID = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        const string CLAIM_UPN = "http://schemas.microsoft.com/identity/claims/upn";
        const string CLAIM_FIRSTNAME = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname";
        const string CLAIM_SURNAME = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname";
        const string CLAIM_USERNAME = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
        const string CLAIM_USERNAME_PREFERRED = "preferred_username";
        


        const string CLAIM_NAME_2 = "name"; //Cana probably delete

        public static UserDto CreateSepesUser(IPrincipal principal)
        {
            var claimsPrincipal = principal as ClaimsPrincipal;
            var user = new UserDto(claimsPrincipal);
            return user;
        }


        public static string GetTenantId(IPrincipal principal)
        {
            return GetClaimValue(principal, CLAIM_TENANT);
        }

        public static string GetOid(IPrincipal principal)
        {
            return GetClaimValue(principal, CLAIM_OID);      
        }
        public static string GetUsername(IPrincipal principal)
        {
            return principal.Identity.Name;

            //20200828: The attempts below were just desperate tries to get the username. We then discovered that the front end forgot to specify relevant scopes, not giving us the claims we needed

            //string userName;  
            
            //if()

            //if (TryGetClaimValue(principal, CLAIM_UPN, out userName))
            //{
            //    return userName;
            //}
            //else if (TryGetClaimValue(principal, CLAIM_USERNAME, out userName))
            //{
            //    return userName;
            //}
            //else if (TryGetClaimValue(principal, CLAIM_USERNAME_PREFERRED, out userName))
            //{
            //    return userName;
            //}

            //throw new Exception("Unable to determine username for principal: " + principal.Identity.Name);
         
        }

        public static string GetEmail(IPrincipal principal)
        {
            return GetUsername(principal).ToLowerInvariant();
        }

        public static string GetFullName(IPrincipal principal)
        {
            string firstName;
            string surName;

            if(TryGetClaimValue(principal, CLAIM_FIRSTNAME, out firstName))
            {
                if (TryGetClaimValue(principal, CLAIM_SURNAME, out surName))
                {
                    return firstName + " " + surName;
                }
            }

            return "n/a";            
        }       

        static string GetClaimValue(IPrincipal principal, string claimName)
        {
            var claimsPrincipal = principal as ClaimsPrincipal;
            var relevantClaim = claimsPrincipal.FindFirst(claimName);
            return relevantClaim.Value;
        }

        static bool TryGetClaimValue(IPrincipal principal, string claimName, out string claimValue)
        {
            var claimsPrincipal = principal as ClaimsPrincipal;
            var relevantClaim = claimsPrincipal.FindFirst(claimName);

            if(relevantClaim == null)
            {
                claimValue = null;
                return false;
            }

            claimValue = relevantClaim.Value;
            return true;
        }
    }
}
