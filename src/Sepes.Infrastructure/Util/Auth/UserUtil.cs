using Microsoft.Extensions.Configuration;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model.Config;
using System;
using System.Security.Claims;
using System.Security.Principal;

namespace Sepes.Infrastructure.Util.Auth
{
    public static class UserUtil
    {
        
        const string CLAIM_TENANT = "http://schemas.microsoft.com/identity/claims/tenantid";

        public static UserDto CreateSepesUser(IConfiguration config, IPrincipal principal)
        {
            var claimsPrincipal = principal as ClaimsPrincipal;            

            var user = new UserDto(                
                GetOid(config, claimsPrincipal),
                GetUsername(config, claimsPrincipal),
                GetFullName(config, claimsPrincipal),
                GetEmail(config, claimsPrincipal));

            if (principal.IsInRole(AppRoles.Admin))
            {
                user.Admin = true;
                user.AppRoles.Add(AppRoles.Admin);
            }

            if (principal.IsInRole(AppRoles.Sponsor))
            {
                user.Sponsor = true;
                user.AppRoles.Add(AppRoles.Sponsor);
            }

            if (principal.IsInRole(AppRoles.DatasetAdmin))
            {
                user.DatasetAdmin = true;
                user.AppRoles.Add(AppRoles.DatasetAdmin);
            }      

            return user;
        }

        public static string GetTenantId(IPrincipal principal)
        {
            return GetClaimValue(principal, CLAIM_TENANT);
        }

        public static string GetOid(IConfiguration config, IPrincipal principal)
        {
            var claimKey = config[ConfigConstants.CLAIM_OID];
            return GetClaimValue(principal, claimKey);
        }

        public static string GetUsername(IConfiguration config, IPrincipal principal)
        {
            var claimKey = config[ConfigConstants.CLAIM_USERNAME];     
            return GetClaimValue(principal, claimKey);
        }

        public static string GetEmail(IConfiguration config, IPrincipal principal)
        {
            var claimKey = config[ConfigConstants.CLAIM_EMAIL];
            return GetClaimValue(principal, claimKey).ToLower();
        }

        public static string GetFullName(IConfiguration config, IPrincipal principal)
        {
            var claimKey = config[ConfigConstants.CLAIM_FULLNAME];
            return GetClaimValue(principal, claimKey);
        }

        static string GetClaimValue(IPrincipal principal, string claimName)
        {
            string claimValue = null;

            if(TryGetClaimValue(principal, claimName, out claimValue))
            {
               return claimValue;               
            }

            throw new NullReferenceException($"Claim with name {claimName} not present!");        
        }

        static bool TryGetClaimValue(IPrincipal principal, string claimName, out string claimValue)
        {
            var claimsPrincipal = principal as ClaimsPrincipal;
            var relevantClaim = claimsPrincipal.FindFirst(claimName);

            if (relevantClaim == null)
            {
                claimValue = null;
                return false;
            }

            claimValue = relevantClaim.Value;
            return true;
        }
    }
}
