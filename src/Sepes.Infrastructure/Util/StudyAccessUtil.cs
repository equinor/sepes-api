using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Service.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Util
{
    public static class StudyAccessUtil
    {

        public static async Task<Study> CheckStudyAccessOrThrow(IUserService userService, Study study, UserOperations operation)
        { 
            if(CheckOperationAgainstApplicationRoles(userService, study, operation))
            {
                return study;
            }

            await CheckStudyRolesThrowIfNotAllowed(userService, study, operation);

            return study;
        }

        public static bool CheckOperationAgainstApplicationRoles(IUserService userService, Study study, UserOperations operation)
        {
            var currentUser = userService.GetCurrentUser();

            //STUDY 
            if (operation == UserOperations.StudyCreate)
            {
                if (currentUser.Admin || currentUser.Sponsor)
                {
                    return true;
                }
            }
            else if (operation == UserOperations.StudyRead && study.Restricted == false)
            {
                return true;
            }
           else if (operation == UserOperations.StudyDelete)
            {
                if (currentUser.Admin || currentUser.Sponsor)
                {
                    return true;
                }
            }

            //Sponsors should be able to add sandbox
            //TODO: Verify that they should have access to create sandbox for restricted studies in which they don't own
            else if (operation == UserOperations.StudyAddRemoveSandbox)
            {
                if (study.Restricted == false && (currentUser.Sponsor))
                {
                    return true;
                }
            }
            
          

            return false;
        }


        public static async Task CheckStudyRolesThrowIfNotAllowed(IUserService userService, Study study, UserOperations operation)
        {
            var requiredRoles = UserOperationsAndRequiredRoles.GetRequiredRoles(operation);

            if ((await UserHasRequiredStudyRole(userService, study, requiredRoles)) == false)
            {
                throw new ForbiddenException($"User {userService.GetCurrentUser().EmailAddress} does not have permission to perform operation {operation} on study {study.Id}");
            }
        }

        static async Task<bool> UserHasRequiredStudyRole(IUserService userService, Study study, params string[] requiredRoles)
        {
            var currentUser = await userService.GetCurrentUserFromDbAsync();
            return UserHasRequiredStudyRole(currentUser.Id, study, requiredRoles);
        }

        static bool UserHasRequiredStudyRole(int userId, Study study, params string[] requiredRoles)
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
