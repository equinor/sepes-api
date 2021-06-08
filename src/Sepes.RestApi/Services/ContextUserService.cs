using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Sepes.Common.Constants;
using Sepes.Common.Dto;
using Sepes.Common.Interface;
using Sepes.Common.Util;
using System.Security.Claims;
using System.Security.Principal;

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

            var user = new UserDto();
            user.ObjectId = GetUserId(claimsIdentity);

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
            var userId = GetClaimValue(claimsIdentity, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn");
            return userId;
        }

        string GetEmail(ClaimsIdentity claimsIdentity)
        {
            var userId = GetClaimValue(claimsIdentity, "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
            return userId;
        }

        string GetFullName(ClaimsIdentity claimsIdentity)
        {
            return GetClaimValue(claimsIdentity, "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
        }

        public bool IsEmployee()
        {
            var employeeAdGroups = ConfigUtil.GetCommaSeparatedConfigValueAndThrowIfEmpty(_configuration, ConfigConstants.EMPLOYEE_ROLE);

            foreach (var curEmployeeAdGroup in employeeAdGroups)
            {
                if (GetPrincipal().IsInRole(curEmployeeAdGroup))
                {
                    return true;
                }
            }

            return false;
        }

        bool IsAdmin()
        {
            return GetPrincipal().IsInRole(AppRoles.Admin);
        }

        bool IsDatasetAdmin()
        {
            return GetPrincipal().IsInRole(AppRoles.DatasetAdmin);
        }

        bool IsSponsor()
        {
            return GetPrincipal().IsInRole(AppRoles.Sponsor);
        }

        IPrincipal GetPrincipal()
        {
            return _httpContextAccessor.HttpContext.User;
        }

        ClaimsIdentity GetIdentity()
        {
            return _httpContextAccessor.HttpContext?.User?.Identity as ClaimsIdentity;
        }

        string GetClaimValue(ClaimsIdentity claimsIdentity, string claimType)
        {
            var claimValue = claimsIdentity.FindFirst(claimType).Value;
            return claimValue;
        }
     
        void ApplyExtendedProps(UserDto user)
        {
            if (IsAdmin())
            {
                user.Admin = true;
                user.AppRoles.Add(AppRoles.Admin);
            }

            if (IsSponsor())
            {
                user.Sponsor = true;
                user.AppRoles.Add(AppRoles.Sponsor);
            }

            if (IsDatasetAdmin())
            {
                user.DatasetAdmin = true;
                user.AppRoles.Add(AppRoles.DatasetAdmin);
            }

            if (IsEmployee())
            {
                user.Employee = true;
            }
        }
    }
}
