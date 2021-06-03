using Microsoft.Extensions.Configuration;
using Sepes.Common.Constants;
using Sepes.Common.Dto;
using Sepes.Infrastructure.Model;
using System.Security.Principal;
using Sepes.Azure.Dto;
using Sepes.Common.Interface;
using Sepes.Common.Util;

namespace Sepes.Infrastructure.Util.Auth
{
    public static class UserUtil
    {       

        //public static void ApplyExtendedProps(IConfiguration config, IPrincipalService principalService, UserDto user)
       

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
