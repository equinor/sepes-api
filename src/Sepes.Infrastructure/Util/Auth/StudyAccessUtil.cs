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
        public static void HasAccessToOperationOrThrow(UserDto currentUser, UserOperation operation)
        {
            if (!HasAccessToOperation(currentUser, operation))
            {
                throw new ForbiddenException($"User {currentUser.EmailAddress} does not have permission to perform operation {operation}");
            }
        }

        public static bool HasAccessToOperation(UserDto currentUser, UserOperation operation)
        {
            var onlyRelevantOperations = AllowedUserOperations.ForOperationQueryable(operation);

            //First thest this, as it's the most common operation and it requires no db access
            if (IsAllowedForEmployeesWithoutAnyRoles(currentUser, onlyRelevantOperations))
            {
                return true;
            }

            if (IsAllowedBasedOnAppRoles(currentUser, onlyRelevantOperations))
            {
                return true;
            }

            return false;
        }

        //Userservice

        //User

        //User returns

        public static void CheckAccesAndThrowIfMissing(UserDto currentUser, Study study, UserOperation operation, string newRole = null)
        { 
            if (!HasAccessToOperationForStudy(currentUser, study, operation, newRole))
            {
                throw StudyAccessUtil.CreateForbiddenException(currentUser, study, operation);
            }
        }

        public static async Task CheckAccesAndThrowIfMissing(IUserService userService, Study study, UserOperation operation, string newRole = null)
        {
            var currentUser = await userService.GetCurrentUserWithStudyParticipantsAsync();

            CheckAccesAndThrowIfMissing(currentUser, study, operation, newRole);
        }
   

        public static Study HasAccessToOperationForStudyOrThrow(UserDto currentUser, Study study, UserOperation operation, string newRole = null)
        {
            CheckAccesAndThrowIfMissing(currentUser, study, operation, newRole);

            return study;           
        }

        public static ForbiddenException CreateForbiddenException(UserDto user, Study study, UserOperation operation)
        {
            return new ForbiddenException($"User {user.EmailAddress} does not have permission to perform operation {operation} on study {study.Id}");
        }

        public static async Task<bool> HasAccessToOperationForStudyAsync(IUserService userService, Study study, UserOperation operation, string newRole = null)
        {
            var currentUser = await userService.GetCurrentUserWithStudyParticipantsAsync();
            return HasAccessToOperationForStudy(currentUser, study, operation, newRole);
        }

        public static bool HasAccessToOperationForStudy(UserDto currentUser, Study study, UserOperation operation, string newRole = null)
        {
            var onlyRelevantOperations = AllowedUserOperations.ForOperationQueryable(operation);

            //First thest this, as it's the most common operation and it requires no db access
            if (IsAllowedForEmployeesWithoutAnyRoles(currentUser, onlyRelevantOperations, study))
            {
                return true;
            }

            if (IsAllowedBasedOnAppRoles(currentUser, onlyRelevantOperations, study, operation, newRole))
            {
                return true;
            }

            if (IsAllowedBasedOnStudyRoles(currentUser, onlyRelevantOperations, study, operation, newRole))
            {
                return true;
            }

            return false;
        }

        public static bool IsAllowedForEmployeesWithoutAnyRoles(UserDto currentUser, IEnumerable<OperationPermission> relevantOperations, Study study = null)
        {
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

        static bool IsAllowedBasedOnAppRoles(UserDto currentUser, IEnumerable<OperationPermission> relevantOperations)
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

        static bool IsAllowedBasedOnAppRoles(UserDto currentUser, IEnumerable<OperationPermission> relevantOperations, Study study, UserOperation operation, string newRole = null)
        {
            var allowedForAppRolesQueryable = AllowedUserOperations.ForAppRolesLevel(relevantOperations);

            if (study.Restricted)
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
                            if (UserHasAnyOfTheseStudyRoles(currentUser.Id, study, operation, newRole, StudyRoles.StudyOwner))
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

        static bool IsAllowedBasedOnStudyRoles(UserDto currentUser, IEnumerable<OperationPermission> relevantOperations, Study study, UserOperation operation, string newRole = null)
        {
            var allowedForStudyRolesQueryable = AllowedUserOperations.ForStudySpecificRolesLevel(relevantOperations);

            if (study.Restricted)
            {
                allowedForStudyRolesQueryable = allowedForStudyRolesQueryable.Where(or => !or.AppliesOnlyToNonHiddenStudies);
            }

            if (allowedForStudyRolesQueryable.Any())
            {
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
            if (!String.IsNullOrWhiteSpace(newRole))
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
