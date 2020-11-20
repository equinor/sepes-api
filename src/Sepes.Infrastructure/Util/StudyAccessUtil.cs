using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Constants;
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
            if (operation == UserOperations.StudyCreate)
            {
                if (userService.GetCurrentUser().Admin || userService.GetCurrentUser().Sponsor)
                {
                    return study;
                }
            }

            //Sponsors should be able to add sandbox
            //TODO: Verify that they should have access to create sandbox for restricted studies in which they don't own
            if (operation == UserOperations.StudyAddRemoveSandbox)
            {
                if (userService.GetCurrentUser().Sponsor) {

                    return study;
                } 
            }

            //No study specific roles required
            if (operation == UserOperations.StudyRead && study.Restricted == false)
            {
                return study;
            }

            await ThrowIfOperationNotAllowed(userService, study, operation);

            return study;
        }


        public static async Task ThrowIfOperationNotAllowed(IUserService userService, Study study, UserOperations operation)
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
