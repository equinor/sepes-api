using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Constants.Auth;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Auth;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Util.Auth
{
    public static class StudyAccessUtil
    {
        public static void HasAccessToOperationOrThrow(IUserService userService, UserOperation operation)
        {
            if (HasAccessToOperation(userService, operation) == false)
            {
                throw new ForbiddenException($"User {userService.GetCurrentUser().EmailAddress} does not have permission to perform operation {operation}");
            }
        }

        public static bool HasAccessToOperation(IUserService userService, UserOperation operation)
        {
            var onlyRelevantOperations = AllowedUserOperations.ForOperationQueryable(operation);

            //First thest this, as it's the most common operation and it requires no db access
            if (IsAllowedForEmployeesWithoutAnyRoles(userService, onlyRelevantOperations))
            {
                return true;
            }

            if (IsAllowedBasedOnAppRoles(userService, onlyRelevantOperations))
            {
                return true;
            }

            return false;
        }

        public static async Task<Study> HasAccessToOperationForStudyOrThrow(IUserService userService, Study study, UserOperation operation, string newRole = null)
        {
            if (await HasAccessToOperationForStudy(userService, study, operation, newRole))
            {
                return study;
            }

            throw new ForbiddenException($"User {userService.GetCurrentUser().EmailAddress} does not have permission to perform operation {operation} on study {study.Id}");
        }

        public static async Task<bool> HasAccessToOperationForStudy(IUserService userService, Study study, UserOperation operation, string newRole = null)
        {
            var onlyRelevantOperations = AllowedUserOperations.ForOperationQueryable(operation);

            //First thest this, as it's the most common operation and it requires no db access
            if (IsAllowedForEmployeesWithoutAnyRoles(userService, onlyRelevantOperations, study))
            {
                return true;
            }

            if (await IsAllowedBasedOnAppRoles(userService, onlyRelevantOperations, study, operation, newRole))
            {
                return true;
            }

            if (await IsAllowedBasedOnStudyRoles(userService, onlyRelevantOperations, study, operation, newRole))
            {
                return true;
            }

            return false;
        }

        public static bool IsAllowedForEmployeesWithoutAnyRoles(IUserService userService, IEnumerable<OperationPermission> relevantOperations, Study study = null)
        {
            var currentUser = userService.GetCurrentUser();

            if (!currentUser.Employee)
            {
                return false;
            }

            var operationsAllowedWithoutRoles = AllowedUserOperations.ForAllNonExternalUserLevel(relevantOperations);

            if (study != null && study.Restricted)
            {
                operationsAllowedWithoutRoles = AllowedUserOperations.ForRestrictedStudies(operationsAllowedWithoutRoles);
            }

            return operationsAllowedWithoutRoles.Any();
        }

        static bool IsAllowedBasedOnAppRoles(IUserService userService, IEnumerable<OperationPermission> relevantOperations)
        {
            var allowedForAppRolesQueryable = AllowedUserOperations.ForAppRolesLevel(relevantOperations);

            if (allowedForAppRolesQueryable.Any())
            {
                var currentUser = userService.GetCurrentUser();

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

        static async Task<bool> IsAllowedBasedOnAppRoles(IUserService userService, IEnumerable<OperationPermission> relevantOperations, Study study, UserOperation operation, string newRole = null)
        {
            var allowedForAppRolesQueryable = AllowedUserOperations.ForAppRolesLevel(relevantOperations);

            if (study.Restricted)
            {
                allowedForAppRolesQueryable = AllowedUserOperations.ForRestrictedStudies(allowedForAppRolesQueryable);
            }

            if (allowedForAppRolesQueryable.Any())
            {
                var currentUserDb = await userService.GetCurrentUserWithStudyParticipantsAsync();

                foreach (var curAllowance in allowedForAppRolesQueryable)
                {
                    if (UserHasAnyOfTheseAppRoles(currentUserDb, curAllowance.AllowedForRoles))
                    {
                        if (curAllowance.AppliesOnlyIfUserIsStudyOwner)
                        {
                            if (UserHasAnyOfTheseStudyRoles(currentUserDb.Id, study, operation, newRole, StudyRoles.StudyOwner))
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

        static async Task<bool> IsAllowedBasedOnStudyRoles(IUserService userService, IEnumerable<OperationPermission> relevantOperations, Study study, UserOperation operation, string newRole = null)
        {
            var allowedForStudyRolesQueryable = AllowedUserOperations.ForStudySpecificRolesLevel(relevantOperations);

            if (study.Restricted)
            {
                allowedForStudyRolesQueryable = allowedForStudyRolesQueryable.Where(or => or.AppliesOnlyToNonHiddenStudies == false);
            }

            if (allowedForStudyRolesQueryable.Any())
            {
                var currentUser = await userService.GetCurrentUserFromDbAsync();

                foreach (var curOpWithRole in allowedForStudyRolesQueryable)
                {
                    if (UserHasAnyOfTheseStudyRoles(currentUser.Id, study, curOpWithRole.AllowedForRoles, operation, newRole))
                    {
                        return true;
                    }
                }
            }

            return false;
        }



        static bool UserHasAnyOfTheseStudyRoles(int userId, Study study, HashSet<string> requiredRoles, UserOperation operation, string newRole = null)
        {
            foreach (var curParticipant in study.StudyParticipants.Where(p => p.UserId == userId))
            {
                if (requiredRoles.Contains(curParticipant.RoleName) && !DisqualifiedBySpecialVendorAdminCase(curParticipant, operation, newRole))
                {
                    return true;
                }
            }

            return false;
        }

        static bool UserHasAnyOfTheseStudyRoles(int userId, Study study, UserOperation operation, string newRole, params string[] requiredRoles)
        {
            var requiredRolesLookup = new HashSet<string>(requiredRoles);

            return UserHasAnyOfTheseStudyRoles(userId, study, requiredRolesLookup, operation, newRole);
        }

        static bool DisqualifiedBySpecialVendorAdminCase(StudyParticipant studyParticipant, UserOperation operation, string newRole)
        {
            if (String.IsNullOrWhiteSpace(newRole) == false)
            {
                if (operation == UserOperation.Study_AddRemove_Participant)
                {
                    if (studyParticipant.RoleName == StudyRoles.VendorAdmin)
                    {
                        if (newRole != StudyRoles.VendorContributor && newRole != StudyRoles.VendorAdmin)
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
