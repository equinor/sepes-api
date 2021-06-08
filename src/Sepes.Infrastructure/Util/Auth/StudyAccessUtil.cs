using Sepes.Common.Constants;
using Sepes.Common.Constants.Auth;
using Sepes.Common.Dto;
using Sepes.Common.Dto.Auth;
using Sepes.Common.Interface;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sepes.Infrastructure.Util.Auth
{
    public static class StudyAccessUtil
    {

        public static bool HasAccessToOperationForStudy(UserDto currentUser, IHasStudyPermissionDetails studyPermissionDetails, UserOperation operation, string roleBeingAddedOrRemoved = null)
        {
            var onlyRelevantOperations = AllowedUserOperations.ForOperationQueryable(operation);

            //First thest this, as it's the most common operation and it requires no db access
            if (IsAllowedForEmployeesWithoutAnyRoles(currentUser, onlyRelevantOperations, studyPermissionDetails))
            {
                return true;
            }

            if (IsAllowedBasedOnAppRoles(currentUser, onlyRelevantOperations, studyPermissionDetails, operation, roleBeingAddedOrRemoved))
            {
                return true;
            }

            if (IsAllowedBasedOnStudyRoles(currentUser, onlyRelevantOperations, studyPermissionDetails, operation, roleBeingAddedOrRemoved))
            {
                return true;
            }

            return false;
        }

        public static bool IsAllowedForEmployeesWithoutAnyRoles(UserDto currentUser, IEnumerable<OperationPermission> relevantOperations, IHasStudyPermissionDetails studyPermissionDetails = null)
        {
            if (!currentUser.Employee)
            {
                return false;
            }

            var operationsAllowedWithoutRoles = AllowedUserOperations.ForAllNonExternalUserLevel(relevantOperations);

            if (studyPermissionDetails != null && studyPermissionDetails.Restricted)
            {
                operationsAllowedWithoutRoles = AllowedUserOperations.ForRestrictedStudies(operationsAllowedWithoutRoles);
            }

            return operationsAllowedWithoutRoles.Any();
        }

        public static bool IsAllowedBasedOnAppRoles(UserDto currentUser, IEnumerable<OperationPermission> relevantOperations)
        {
            var allowedForAppRolesQueryable = AllowedUserOperations.ForAppRolesLevel(relevantOperations);

            if (allowedForAppRolesQueryable.Any())
            {
                foreach (var curAllowance in allowedForAppRolesQueryable)
                {
                    if (UserHasAnyOfTheseAppRoles(currentUser, curAllowance.AllowedForRoles))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool IsAllowedBasedOnAppRoles(UserDto currentUser, IEnumerable<OperationPermission> relevantOperations, IHasStudyPermissionDetails studyPermissionDetails, UserOperation operation, string roleBeingAddedOrRemoved = null)
        {
            var allowedForAppRolesQueryable = AllowedUserOperations.ForAppRolesLevel(relevantOperations);

            if (studyPermissionDetails.Restricted)
            {
                allowedForAppRolesQueryable = AllowedUserOperations.ForRestrictedStudies(allowedForAppRolesQueryable);
            }

            if (allowedForAppRolesQueryable.Any())
            {
                foreach (var curAllowance in allowedForAppRolesQueryable)
                {
                    if (UserHasAnyOfTheseAppRoles(currentUser, curAllowance.AllowedForRoles))
                    {
                        if (curAllowance.AppliesOnlyIfUserIsStudyOwner)
                        {
                            if (UserHasAnyOfTheseStudyRoles(currentUser.Id, studyPermissionDetails, operation, roleBeingAddedOrRemoved, StudyRoles.StudyOwner))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }

                }
            }

            return false;
        }

        public static bool IsAllowedBasedOnStudyRoles(UserDto currentUser, IEnumerable<OperationPermission> relevantOperations, IHasStudyPermissionDetails studyPermissionDetails, UserOperation operation, string roleBeingAddedOrRemoved = null)
        {
            var allowedForStudyRolesQueryable = AllowedUserOperations.ForStudySpecificRolesLevel(relevantOperations);

            if (studyPermissionDetails.Restricted)
            {
                allowedForStudyRolesQueryable = allowedForStudyRolesQueryable.Where(or => !or.AppliesOnlyToNonHiddenStudies);
            }

            if (allowedForStudyRolesQueryable.Any())
            {
                foreach (var curOpWithRole in allowedForStudyRolesQueryable)
                {
                    if (UserHasAnyOfTheseStudyRoles(currentUser.Id, studyPermissionDetails, curOpWithRole.AllowedForRoles, operation, roleBeingAddedOrRemoved))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        static bool UserHasAnyOfTheseStudyRoles(int userId, IHasStudyPermissionDetails studyPermissionDetails, HashSet<string> requiredRoles, UserOperation operation, string roleBeingAddedOrRemoved = null)
        {
            if (studyPermissionDetails.UsersAndRoles.TryGetValue(userId, out HashSet<string> rolesForUser))
            {
                foreach (var curRoleForUser in rolesForUser)
                {
                    if (requiredRoles.Contains(curRoleForUser) && !DisqualifiedBySpecialVendorAdminCase(curRoleForUser, operation, roleBeingAddedOrRemoved))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        static bool UserHasAnyOfTheseStudyRoles(int userId, IHasStudyPermissionDetails studyPermissionDetails, UserOperation operation, string roleBeingAddedOrRemoved, params string[] requiredRoles)
        {
            var requiredRolesLookup = new HashSet<string>(requiredRoles);

            return UserHasAnyOfTheseStudyRoles(userId, studyPermissionDetails, requiredRolesLookup, operation, roleBeingAddedOrRemoved);
        }

        static bool DisqualifiedBySpecialVendorAdminCase(string roleName, UserOperation operation, string roleBeingAddedOrRemoved)
        {
            if (!String.IsNullOrWhiteSpace(roleBeingAddedOrRemoved))
            {
                if (operation == UserOperation.Study_AddRemove_Participant)
                {
                    if (roleName == StudyRoles.VendorAdmin)
                    {
                        if (roleBeingAddedOrRemoved != StudyRoles.VendorContributor && roleBeingAddedOrRemoved != StudyRoles.VendorAdmin)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        static bool UserHasAnyOfTheseAppRoles(UserDto currentUser, HashSet<string> appRoles)
        {
            if (currentUser == null)
            {
                throw new ArgumentNullException("currentUser");
            }

            if (appRoles == null || appRoles.Count() == 0)
            {
                throw new ArgumentNullException("requiredRoles");
            }

            if (currentUser.AppRoles == null)
            {
                return false;
            }

            return currentUser.AppRoles.Intersect(appRoles).Any();
        }
    }
}
