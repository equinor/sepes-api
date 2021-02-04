using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Constants.Auth;
using Sepes.Infrastructure.Dto;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepes.Infrastructure.Util.Auth
{
    public static class StudyAccessQueryBuilder
    {
        const string ALWAYS_TRUE = "1 = 1";
        const string NON_HIDDEN_CRITERIA = " AND s.Restricted = 0";

        public static string CreateAccessWhereClause(UserDto currentUser, UserOperation operation)
        {
            var ors = new List<string>();         

            var relevantPermissions = AllowedUserOperations.ForOperationQueryable(operation); 

            var allowedForAppRolesQueryable = AllowedUserOperations.ForAppRolesLevel(relevantPermissions);

            if (allowedForAppRolesQueryable.Any())
            {
                foreach (var curPermission in allowedForAppRolesQueryable)
                {
                    if (
                        (currentUser.Admin && curPermission.AllowedForRoles.Contains(AppRoles.Admin))
                        || (currentUser.Sponsor && curPermission.AllowedForRoles.Contains(AppRoles.Sponsor))
                        || (currentUser.DatasetAdmin && curPermission.AllowedForRoles.Contains(AppRoles.DatasetAdmin))
                        )
                    {
                        var currentOr = ALWAYS_TRUE;

                        //Permission will be granted without restrictions (typically admin)
                        if(!curPermission.AppliesOnlyToNonHiddenStudies && !curPermission.AppliesOnlyIfUserIsStudyOwner)
                        {
                            return null;
                        }

                        if (curPermission.AppliesOnlyToNonHiddenStudies)
                        {
                            currentOr += NON_HIDDEN_CRITERIA;
                        }

                        if (curPermission.AppliesOnlyIfUserIsStudyOwner)
                        {
                            currentOr += UserHasRole(currentUser, StudyRoles.StudyOwner);
                        }

                        ors.Add(currentOr);
                    }
                }              
            }

            if (currentUser.Employee)
            {
                var operationsAllowedForEmployeesOnly = AllowedUserOperations.ForAllNonExternalUserLevel(relevantPermissions);

                foreach (var curPermission in operationsAllowedForEmployeesOnly)
                {
                    var currentOr = ALWAYS_TRUE;

                    if (curPermission.AppliesOnlyToNonHiddenStudies)
                    {
                        currentOr += NON_HIDDEN_CRITERIA;
                    }

                    ors.Add(currentOr);
                }
            }

            var allowedForStudyRolesQueryable = AllowedUserOperations.ForStudySpecificRolesLevel(relevantPermissions);

            if (allowedForStudyRolesQueryable.Any())
            {
                foreach (var curPermission in allowedForStudyRolesQueryable)
                {
                    var currentOr = ALWAYS_TRUE;

                    if (curPermission.AppliesOnlyToNonHiddenStudies)
                    {
                        currentOr += NON_HIDDEN_CRITERIA;
                    }

                    currentOr += UserHasRole(currentUser, curPermission.AllowedForRoles);

                    ors.Add(currentOr);
                }
            }

            if(ors.Count > 0)
            {
                var whereClauseBuilder = new StringBuilder();

                foreach(var curOr in ors)
                {
                    if(whereClauseBuilder.Length > 0)
                    {
                        whereClauseBuilder.Append(" OR ");
                    }

                    whereClauseBuilder.Append($"({curOr})");
                }

                return whereClauseBuilder.ToString();
            }

            return null;
        }
             

        public static string UserHasRole(UserDto currentUser, string roleName)
        {
            return UserHasRoleUndecidedEqualType(currentUser, $" = '{roleName}'");
        }

        public static string UserHasRole(UserDto currentUser, HashSet<string> roleNames)
        {
            var sbRoleNames = new StringBuilder();
            
            foreach(var curRoleName in roleNames)
            {
                if(sbRoleNames.Length > 0)
                {
                    sbRoleNames.Append(",");
                }

                sbRoleNames.Append($"'{curRoleName}'");
            }
            
            string.Join(",", roleNames);
            return UserHasRoleUndecidedEqualType(currentUser, $" IN ({sbRoleNames})");
        }

        public static string UserHasRoleUndecidedEqualType(UserDto currentUser, string roleNameEqualsPart)
        {
            return $" AND sp.UserId = {currentUser.Id} AND sp.RoleName {roleNameEqualsPart}";
        }       
    }
}
