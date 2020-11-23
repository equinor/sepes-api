using Sepes.Infrastructure.Constants;
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

        public static async Task<Study> CheckStudyAccessOrThrow(IUserService userService, Study study, UserOperations operation)
        {

            if (study.Restricted == false && CheckOperationAgainstThoseWhoAreOpenForAll(userService, study, operation))
            {
                return study;
            }

            if (CheckOperationAgainstApplicationRoles(userService, study, operation))
            {
                return study;
            }

            await CheckStudyRolesThrowIfNotAllowed(userService, study, operation);

            return study;
        }

        public static bool CheckOperationAgainstThoseWhoAreOpenForAll(IUserService userService, Study study, UserOperations operation)
        {
            var operationsOpenToEverybody = OperationsForAnyUser.GetOperations();

            return (operationsOpenToEverybody.Contains(operation)); 
        }

        public static bool CheckOperationAgainstApplicationRoles(IUserService userService, Study study, UserOperations operation)
        {
            var requiredRoles = GetRequiredApplicationRoles(OperationsAndApplicationRoles.GetOperations(), operation, study.Restricted);


            return UserHasRequiredApplicationRole(userService, requiredRoles);


            // //STUDY 
            // if (operation == UserOperations.StudyCreate)
            // {
            //     if (currentUser.Admin || currentUser.Sponsor)
            //     {
            //         return true;
            //     }
            // }
            // else if (operation == UserOperations.StudyRead && study.Restricted == false)
            // {
            //     return true;
            // }
            //else if (operation == UserOperations.StudyDelete)
            // {
            //     if (currentUser.Admin || currentUser.Sponsor)
            //     {
            //         return true;
            //     }
            // }

            // //Sponsors should be able to add sandbox
            // //TODO: Verify that they should have access to create sandbox for restricted studies in which they don't own
            // else if (operation == UserOperations.StudyAddRemoveSandbox)
            // {
            //     if (study.Restricted == false && (currentUser.Sponsor))
            //     {
            //         return true;
            //     }
            // }          

            // return false;
        }

        public static HashSet<string> GetRequiredApplicationRoles(Dictionary<Tuple<UserOperations, bool>, HashSet<string>> operationsForRoles, UserOperations operation, bool studyIsHidden)
        {

            HashSet<string> roleSet = null;

            //Case, study is hidden
            //Can use: false
            //Study is open
            //Can use: false and true, true first because it's more specific?


            if (studyIsHidden)  //Study is hidden, can only use entries where "onlyForNonHidden" is false
            {
                if (operationsForRoles.TryGetValue(new Tuple<UserOperations, bool>(operation, false), out roleSet))
                {
                    return roleSet;
                }

            }
            else  //Study is NOT hidden, can use entries where "onlyForNonHidden" is true or false, pick true first because they are more specific
            {
                if (operationsForRoles.TryGetValue(new Tuple<UserOperations, bool>(operation, true), out roleSet))
                {
                    return roleSet;
                }
                else if (operationsForRoles.TryGetValue(new Tuple<UserOperations, bool>(operation, false), out roleSet))
                {
                    return roleSet;
                }
            }

            return null;
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




        public static async Task CheckStudyRolesThrowIfNotAllowed(IUserService userService, Study study, UserOperations operation)
        {
            var requiredRoles = OperationsAndStudyRoles.GetRequiredRoles(operation);

            if ((await UserHasRequiredStudyRole(userService, study, requiredRoles)) == false)
            {
                throw new ForbiddenException($"User {userService.GetCurrentUser().EmailAddress} does not have permission to perform operation {operation} on study {study.Id}");
            }
        }

        static async Task<bool> UserHasRequiredStudyRole(IUserService userService, Study study, HashSet<string> requiredRoles)
        {
            var currentUser = await userService.GetCurrentUserFromDbAsync();
            return UserHasRequiredStudyRole(currentUser.Id, study, requiredRoles);
        }

        static bool UserHasRequiredStudyRole(int userId, Study study, HashSet<string> requiredRoles)
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


    }
}
