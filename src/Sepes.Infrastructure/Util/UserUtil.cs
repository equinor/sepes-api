using Sepes.Infrastructure.Dto;
using System;
using System.Security.Claims;
using System.Security.Principal;

namespace Sepes.Infrastructure.Util
{
    public static class UserUtil
    {
        //c4c0daf8-a7a3-41b8-91b3-332439661e04

        const string CLAIM_OID = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        const string CLAIM_UPN = "http://schemas.microsoft.com/identity/claims/upn";
        const string CLAIM_FIRSTNAME = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname";
        const string CLAIM_SURNAME = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname";
        const string CLAIM_USERNAME = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
        const string CLAIM_NAME_2 = "name"; //Cana probably delete

        public static SepesUser CreateSepesUser(IPrincipal principal)
        {
            var claimsPrincipal = principal as ClaimsPrincipal;
            var user = new SepesUser(claimsPrincipal);
            return user;
        }       

        public static string GetOid(IPrincipal principal)
        {
            return GetClaimValue(principal, CLAIM_OID);      
        }
        public static string GetUsername(IPrincipal principal)
        {
            return GetClaimValue(principal, CLAIM_USERNAME);
        }

        public static string GetEmail(IPrincipal principal)
        {
            return GetUsername(principal).ToLowerInvariant();
        }

        public static string GetFullName(IPrincipal principal)
        {
            var firstName = GetClaimValue(principal, CLAIM_FIRSTNAME);
            var surName = GetClaimValue(principal, CLAIM_SURNAME);

            return firstName + " " + surName;      
        }       

        static string GetClaimValue(IPrincipal principal, string claimName)
        {
            var claimsPrincipal = principal as ClaimsPrincipal;
            var relevantClaim = claimsPrincipal.FindFirst(claimName);
            return relevantClaim.Value;
        }
    }
}
