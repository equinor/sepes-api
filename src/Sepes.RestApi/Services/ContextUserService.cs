using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Sepes.Common.Constants;
using Sepes.Common.Dto;
using Sepes.Common.Interface;
using Sepes.Common.Util;
using System;
using System.Linq;
using System.Security.Claims;

namespace Sepes.RestApi.Services
{
    public class ContextUserService : IContextUserService
    {
        readonly IConfiguration _configuration;
        readonly IHttpContextAccessor _httpContextAccessor;

        public ContextUserService(IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = contextAccessor;
        }

        public string GetCurrentUserObjectId()
        {
            var claimsIdentity = GetIdentity();
            return GetUserId(claimsIdentity);
        }

        public UserDto GetCurrentUser()
        {
            var claimsIdentity = GetIdentity();

            var user = new UserDto
            {
                ObjectId = GetUserId(claimsIdentity)
            };

            if (IsMockUser())
            {
                DecorateMockUser(user);
            }
            else
            {
                DecorateNormalUser(claimsIdentity, user);
            }

            ApplyExtendedProps(user);

            return user;

        }

        void DecorateNormalUser(ClaimsIdentity claimsIdentity, UserDto user)
        {
            user.UserName = GetUsername(claimsIdentity);
            user.FullName = GetFullName(claimsIdentity);
            user.EmailAddress = GetEmail(claimsIdentity);
        }

        void DecorateMockUser(UserDto user)
        {
            user.UserName = "mock@user.com";
            user.EmailAddress = "mock@user.com";
            user.FullName = "Mock User";
        }

        public bool IsMockUser()
        {
            var cypressMockUserIdFromConfig = _configuration[ConfigConstants.CYPRESS_MOCK_USER];

            if (string.IsNullOrWhiteSpace(cypressMockUserIdFromConfig))
            {
                return false;
            }

            var currentUserObjectId = GetCurrentUserObjectId();

            return currentUserObjectId.Equals(cypressMockUserIdFromConfig);
        }

        string GetUserId(ClaimsIdentity claimsIdentity)
        {
            var userId = GetClaimValue(claimsIdentity, "http://schemas.microsoft.com/identity/claims/objectidentifier");
            return userId;
        }

        string GetUsername(ClaimsIdentity claimsIdentity)
        {
            if (TryGetClaimValue(claimsIdentity, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn", out string userNameFromClaim))
            {
                return userNameFromClaim;
            }
            else if (TryGetClaimValue(claimsIdentity, "preferred_username", out userNameFromClaim))
            {
                return userNameFromClaim;
            }

            throw new Exception($"Unable to resolve username for user {claimsIdentity.Name}");
        }

        string GetEmail(ClaimsIdentity claimsIdentity)
        {
            var userId = GetClaimValue(claimsIdentity, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
            return userId;
        }

        string GetFullName(ClaimsIdentity claimsIdentity)
        {
            return GetClaimValue(claimsIdentity, "name");
        }

        public bool IsEmployee(ClaimsPrincipal principal)
        {
            var employeeAdGroups = ConfigUtil.GetCommaSeparatedConfigValueAndThrowIfEmpty(_configuration, ConfigConstants.EMPLOYEE_ROLE);

            foreach (var curEmployeeAdGroup in employeeAdGroups.Where(c=> principal.IsInRole(c)))
            {
                return true;
            }

            return false;
        }       

        void ApplyExtendedProps(UserDto user)
        {
            var principal = GetPrincipal();

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

            if (principal.IsInRole(AppRoles.External))
            {
                user.External = true;
            }

            if (IsEmployee(principal))
            {
                user.Employee = true;
            }           
        }

        ClaimsIdentity GetIdentity()
        {
            return _httpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity;
        }

        ClaimsPrincipal GetPrincipal()
        {
            return _httpContextAccessor.HttpContext.User;
        }    

        string GetClaimValue(ClaimsIdentity claimsIdentity, string claimType)
        {
            var claimValue = claimsIdentity.FindFirst(claimType).Value;
            return claimValue;
        }

        bool TryGetClaimValue(ClaimsIdentity claimsIdentity, string claimType, out string claimValue)
        {
            var claim = claimsIdentity.FindFirst(claimType);

            if (claim != null && !string.IsNullOrWhiteSpace(claim.Value))
            {
                claimValue = claim.Value;
                return true;
            }

            claimValue = null;
            return false;
        }
    }
}
