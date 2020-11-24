﻿using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Constants.Auth;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Util
{
    public static class StudyAccessUtil
    {

        //Permissions
        //AllowedOperatopns
        //Operations
        //App roles
        //Study participant roles

        static List<OperationWithPermissionTwo> AllowedOperations = new List<OperationWithPermissionTwo>() {
            OperationWithPermissionTwo.CreateForAuthorizedUser(UserOperations.Study_Read, appliesOnlyToNonHiddenStudies: true), //As a authorized user, you can read any non-hidden Study
            OperationWithPermissionTwo.CreateForAppRole(UserOperations.Study_Read, appliesOnlyToNonHiddenStudies: false, appliesOnlyIfUserIsStudyOwner:false, AppRoles.Admin), //As a admin, you can read any Study, hidden or not, 
            OperationWithPermissionTwo.CreateForStudyRole(UserOperations.Study_Read, appliesOnlyToNonHiddenStudies: false, StudyRoles.SponsorRep, StudyRoles.VendorAdmin, StudyRoles.VendorContributor, StudyRoles.StudyViewer ), //As a admin, you can read any Study, hidden or not, 
                          
            OperationWithPermissionTwo.CreateForAppRole(UserOperations.Study_Create, appliesOnlyToNonHiddenStudies: false, appliesOnlyIfUserIsStudyOwner:false, AppRoles.Admin, AppRoles.Sponsor), //As a admin or sponsor, you can create a Study 

            OperationWithPermissionTwo.CreateForAppRole(UserOperations.Study_Close, appliesOnlyToNonHiddenStudies: false, appliesOnlyIfUserIsStudyOwner:false, AppRoles.Admin, AppRoles.Sponsor),
            OperationWithPermissionTwo.CreateForStudyRole(UserOperations.Study_Close, appliesOnlyToNonHiddenStudies: false, StudyRoles.SponsorRep),

            OperationWithPermissionTwo.CreateForAppRole(UserOperations.Study_Delete, appliesOnlyToNonHiddenStudies: false, appliesOnlyIfUserIsStudyOwner:false, AppRoles.Admin),

           //METADATA
            OperationWithPermissionTwo.CreateForAppRole(UserOperations.Study_Update_Metadata, appliesOnlyToNonHiddenStudies: false, appliesOnlyIfUserIsStudyOwner:false, AppRoles.Admin, AppRoles.Sponsor),
            OperationWithPermissionTwo.CreateForStudyRole(UserOperations.Study_Update_Metadata, appliesOnlyToNonHiddenStudies: false, StudyRoles.SponsorRep),

            //RESULTS AND LEARNINGS, READ
            OperationWithPermissionTwo.CreateForAuthorizedUser(UserOperations.Study_Read_ResultsAndLearnings, appliesOnlyToNonHiddenStudies: true),
            OperationWithPermissionTwo.CreateForAppRole(UserOperations.Study_Read_ResultsAndLearnings, appliesOnlyToNonHiddenStudies: false, appliesOnlyIfUserIsStudyOwner:false, AppRoles.Admin, AppRoles.Sponsor),
            OperationWithPermissionTwo.CreateForStudyRole(UserOperations.Study_Read_ResultsAndLearnings, appliesOnlyToNonHiddenStudies: false, StudyRoles.StudyViewer),

            //RESULTS AND LEARNINGS, UPDATE
            OperationWithPermissionTwo.CreateForAppRole(UserOperations.Study_Update_ResultsAndLearnings, appliesOnlyToNonHiddenStudies: false, appliesOnlyIfUserIsStudyOwner:false, AppRoles.Admin),
            OperationWithPermissionTwo.CreateForAppRole(UserOperations.Study_Update_ResultsAndLearnings, appliesOnlyToNonHiddenStudies: false, appliesOnlyIfUserIsStudyOwner:true, AppRoles.Sponsor),
            OperationWithPermissionTwo.CreateForStudyRole(UserOperations.Study_Update_ResultsAndLearnings, appliesOnlyToNonHiddenStudies: false, StudyRoles.SponsorRep),

            //STUDY, DATASET CRUD
            OperationWithPermissionTwo.CreateForAppRole(UserOperations.Study_AddRemove_Dataset, appliesOnlyToNonHiddenStudies: false, appliesOnlyIfUserIsStudyOwner:false, AppRoles.Admin, AppRoles.Sponsor),

            //STUDY, ADD/REMOVE PARTICIPANT
            OperationWithPermissionTwo.CreateForAppRole(UserOperations.Study_AddRemove_Participant, appliesOnlyToNonHiddenStudies: false, appliesOnlyIfUserIsStudyOwner:false, AppRoles.Admin, AppRoles.Sponsor),
            OperationWithPermissionTwo.CreateForStudyRole(UserOperations.Study_AddRemove_Participant, appliesOnlyToNonHiddenStudies: false, StudyRoles.SponsorRep),

            //SANDBOX, CRUD
            OperationWithPermissionTwo.CreateForAppRole(UserOperations.Study_Crud_Sandbox, appliesOnlyToNonHiddenStudies: false, appliesOnlyIfUserIsStudyOwner:false, AppRoles.Admin, AppRoles.Sponsor),
            OperationWithPermissionTwo.CreateForStudyRole(UserOperations.Study_Crud_Sandbox, appliesOnlyToNonHiddenStudies: false, StudyRoles.SponsorRep),
        };

        public static bool HasAccessToOperation(IUserService userService, UserOperations operation)
        {
            var onlyRelevantOperations = ForOperationQueryable(operation);

            //First thest this, as it's the most common operation and it requires no db access
            if (TestIfAllowedWithoutRoles(onlyRelevantOperations))
            {
                return true;
            }

            if (AllowOperationBasedOnAppRoles(onlyRelevantOperations, userService))
            {
                return true;
            }

            return false;
        }

        public static async Task<bool> HasAccessToOperationForStudy(IUserService userService, Study study, UserOperations operation)
        {
            var onlyRelevantOperations = ForOperationQueryable(operation);

            //First thest this, as it's the most common operation and it requires no db access
            if (TestIfAllowedWithoutRoles(onlyRelevantOperations, study))
            {
                return true;
            }

            if (await AllowedStudyOperationBasedOnAppRoles(onlyRelevantOperations, userService, study))
            {
                return true;
            }

            if (await AllowedBasedOnStudyRoles(onlyRelevantOperations, userService, study))
            {
                return true;
            }

            return false;
        }

        static IEnumerable<OperationWithPermissionTwo> ForOperationQueryable(UserOperations operation)
        {
            return AllowedOperations.Where(or => or.Operation == operation);
        }

        public static bool TestIfAllowedWithoutRoles(IEnumerable<OperationWithPermissionTwo> relevantOperations, Study study = null)
        {
            var operationsAllowedWithoutRoles = relevantOperations.Where(o => o.Level == PermissionLevel.AuthorizedUser);

            if (study != null && study.Restricted)
            {
                operationsAllowedWithoutRoles = operationsAllowedWithoutRoles.Where(o => o.AppliesOnlyToNonHiddenStudies == false);
            }

            return operationsAllowedWithoutRoles.Any();
        }

        static bool AllowOperationBasedOnAppRoles(IEnumerable<OperationWithPermissionTwo> relevantOperations, IUserService userService)
        {
            var allowedForAppRolesQueryable = relevantOperations.Where(or => or.Level == PermissionLevel.AppRoles);

            if (allowedForAppRolesQueryable.Any())
            {
                var currentUser = userService.GetCurrentUser();

                foreach (var curAllowance in allowedForAppRolesQueryable)
                {
                    if (StudyAccessUtil.UserHasAnyOfTheseAppRoles(currentUser, curAllowance.AllowedForRoles))
                    {
                        return true;
                    }

                }
            }

            return false;
        }

        static async Task<bool> AllowedStudyOperationBasedOnAppRoles(IEnumerable<OperationWithPermissionTwo> relevantOperations, IUserService userService, Study study)
        {
            var allowedForAppRolesQueryable = relevantOperations.Where(or => or.Level == PermissionLevel.AppRoles);

            if (study.Restricted)
            {
                allowedForAppRolesQueryable = allowedForAppRolesQueryable.Where(or => or.AppliesOnlyToNonHiddenStudies == false);
            }

            if (allowedForAppRolesQueryable.Any())
            {
                var currentUserDb = await userService.GetCurrentUserWithStudyParticipantsAsync();

                foreach (var curAllowance in allowedForAppRolesQueryable)
                {
                    if (StudyAccessUtil.UserHasAnyOfTheseAppRoles(currentUserDb, curAllowance.AllowedForRoles))
                    {
                        if (curAllowance.AppliesOnlyIfUserIsStudyOwner)
                        {
                            if (StudyAccessUtil.UserHasAnyOfTheseStudyRoles(currentUserDb.Id, study, StudyRoles.StudyOwner))
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

        static async Task<bool> AllowedBasedOnStudyRoles(IEnumerable<OperationWithPermissionTwo> relevantOperations, IUserService userService, Study study)
        {
            var allowedForStudyRolesQueryable = relevantOperations.Where(or => or.Level == PermissionLevel.StudySpecificRole);

            if (study.Restricted)
            {
                allowedForStudyRolesQueryable = allowedForStudyRolesQueryable.Where(or => or.AppliesOnlyToNonHiddenStudies = false);
            }

            if (allowedForStudyRolesQueryable.Any())
            {
                var currentUser = await userService.GetCurrentUserFromDbAsync();

                foreach (var curOpWithRole in allowedForStudyRolesQueryable)
                {
                    if (StudyAccessUtil.UserHasAnyOfTheseStudyRoles(currentUser.Id, study, curOpWithRole.AllowedForRoles))
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        public static async Task<Study> CheckStudyAccessOrThrow(IUserService userService, Study study, UserOperations operation)
        {
            if (await HasAccessToOperationForStudy(userService, study, operation))
            {
                return study;
            }

            throw new ForbiddenException($"User {userService.GetCurrentUser().EmailAddress} does not have permission to perform operation {operation} on study {study.Id}");
        }
        //public static async Task<Study> CheckStudyAccessOrThrow(IUserService userService, Study study, UserOperations operation)
        //{

        //    if (study.Restricted == false && CheckOperationAgainstThoseWhoAreOpenForAll(userService, study, operation))
        //    {
        //        return study;
        //    }

        //    if (CheckOperationAgainstApplicationRoles(userService, study, operation))
        //    {
        //        return study;
        //    }

        //    await CheckStudyRolesThrowIfNotAllowed(userService, study, operation);

        //    return study;
        //}

        //public static bool CheckOperationAgainstThoseWhoAreOpenForAll(IUserService userService, Study study, UserOperations operation)
        //{
        //    var operationsOpenToEverybody = OperationsForAnyUser.GetOperations();

        //    return (operationsOpenToEverybody.Contains(operation));
        //}

        //public static bool CheckOperationAgainstApplicationRoles(IUserService userService, Study study, UserOperations operation)
        //{
        //    var requiredRoles = GetRequiredApplicationRoles(OperationsAndApplicationRoles.GetOperations(), operation, study.Restricted);


        //    return UserHasRequiredApplicationRole(userService, requiredRoles);



        //}

        //public static HashSet<string> GetRequiredApplicationRoles(Dictionary<Tuple<UserOperations, bool>, HashSet<string>> operationsForRoles, UserOperations operation, bool studyIsHidden)
        //{

        //    HashSet<string> roleSet = null;

        //    //Case, study is hidden
        //    //Can use: false
        //    //Study is open
        //    //Can use: false and true, true first because it's more specific?


        //    if (studyIsHidden)  //Study is hidden, can only use entries where "onlyForNonHidden" is false
        //    {
        //        if (operationsForRoles.TryGetValue(new Tuple<UserOperations, bool>(operation, false), out roleSet))
        //        {
        //            return roleSet;
        //        }

        //    }
        //    else  //Study is NOT hidden, can use entries where "onlyForNonHidden" is true or false, pick true first because they are more specific
        //    {
        //        if (operationsForRoles.TryGetValue(new Tuple<UserOperations, bool>(operation, true), out roleSet))
        //        {
        //            return roleSet;
        //        }
        //        else if (operationsForRoles.TryGetValue(new Tuple<UserOperations, bool>(operation, false), out roleSet))
        //        {
        //            return roleSet;
        //        }
        //    }

        //    return null;
        //}

        public static bool UserHasAnyOfTheseAppRoles(UserDto currentUser, HashSet<string> appRoles)
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

        public static bool UserHasAnyOfTheseAppRoles(UserDto currentUser, params string[] appRoles)
        {
            return UserHasAnyOfTheseAppRoles(currentUser, new HashSet<string>(appRoles));
        }

        static bool UserHasRequiredApplicationRole(IUserService userService, HashSet<string> requiredRoles)
        {
            if (requiredRoles == null || requiredRoles.Count == 0)
            {
                return false;
            }

            var currentUser = userService.GetCurrentUser();

            foreach (var curRole in requiredRoles)
            {
                if (curRole == AppRoles.Admin && currentUser.Admin)
                {
                    return true;
                }
                else if (curRole == AppRoles.Sponsor && currentUser.Sponsor)
                {
                    return true;
                }
                else if (curRole == AppRoles.DatasetAdmin && currentUser.DatasetAdmin)
                {
                    return true;
                }
            }

            return false;
        }

        //public static async Task CheckStudyRolesThrowIfNotAllowed(IUserService userService, Study study, UserOperations operation)
        //{
        //    var requiredRoles = OperationsAndStudyRoles.GetRequiredRoles(operation);

        //    if ((await UserHasAnyOfTheseStudyRoles(userService, study, requiredRoles)) == false)
        //    {
        //        throw new ForbiddenException($"User {userService.GetCurrentUser().EmailAddress} does not have permission to perform operation {operation} on study {study.Id}");
        //    }
        //}

        public static async Task<bool> UserHasAnyOfTheseStudyRoles(IUserService userService, Study study, params string[] requiredRoles)
        {
            var currentUser = await userService.GetCurrentUserFromDbAsync();
            return UserHasAnyOfTheseStudyRoles(currentUser.Id, study, requiredRoles);
        }

        public static bool UserHasAnyOfTheseStudyRoles(int userId, Study study, HashSet<string> requiredRoles)
        {
            foreach (var curParticipant in study.StudyParticipants.Where(p => p.UserId == userId))
            {
                if (requiredRoles.Contains(curParticipant.RoleName))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool UserHasAnyOfTheseStudyRoles(int userId, Study study, params string[] requiredRoles)
        {
            var requiredRolesLookup = new HashSet<string>(requiredRoles);

            return UserHasAnyOfTheseStudyRoles(userId, study, requiredRolesLookup);
        }
    }
}
