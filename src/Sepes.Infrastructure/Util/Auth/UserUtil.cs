using Microsoft.Extensions.Configuration;
using Sepes.Common.Constants;
using Sepes.Common.Dto;
using Sepes.Common.Dto.Azure;
using Sepes.Infrastructure.Model;
using System.Security.Principal;
using Sepes.Common.Interface;
using Sepes.Common.Util;

namespace Sepes.Infrastructure.Util.Auth
{
    public static class UserUtil
    {  
        public static User CreateDbUserFromAzureUser(string objectId,  AzureUserDto azureUser)
        {
            var userDb = new User();
            userDb.ObjectId = objectId;
            userDb.FullName = azureUser.DisplayName;
            userDb.UserName = azureUser.UserPrincipalName;
            userDb.EmailAddress = azureUser.Mail;
            userDb.CreatedBy = azureUser.UserPrincipalName;
            return userDb;
        }      

        public static void ApplyExtendedProps(IConfiguration config, IPrincipalService principalService, UserDto user)
        {
            if (principalService.IsAdmin())
            {
                user.Admin = true;
                user.AppRoles.Add(AppRoles.Admin);
            }

            if (principalService.IsSponsor())
            {
                user.Sponsor = true;
                user.AppRoles.Add(AppRoles.Sponsor);
            }

            if (principalService.IsDatasetAdmin())
            {
                user.DatasetAdmin = true;
                user.AppRoles.Add(AppRoles.DatasetAdmin);
            }

            if (principalService.IsEmployee())
            {
                user.Employee = true;          
            }
        }

        public static bool UserHasAdminRole(IPrincipal principal)
        {
            return principal.IsInRole(AppRoles.Admin);
        }

        public static bool UserHasDatasetAdminRole(IPrincipal principal)
        {
            return principal.IsInRole(AppRoles.DatasetAdmin);
        }

        public static bool UserHasSponsorRole(IPrincipal principal)
        {
            return principal.IsInRole(AppRoles.Sponsor);
        }

        public static bool UserHasEmployeeRole(IConfiguration config, IPrincipal principal)
        {
            var employeeAdGroups = ConfigUtil.GetCommaSeparatedConfigValueAndThrowIfEmpty(config, ConfigConstants.EMPLOYEE_ROLE);

            foreach (var curEmployeeAdGroup in employeeAdGroups)
            {
                if (principal.IsInRole(curEmployeeAdGroup))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
