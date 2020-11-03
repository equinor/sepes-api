using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service;
using Sepes.Infrastructure.Service.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Util
{
    public static class StudyAccessUtil
    {
        //Scenarios
        //Might come with list of study, might come with single

        //Wonder if has access it role. Not solved here
        //Wonder if has study specific role, solved here
        //Might have study Id
        //Might have list of studies
        //Remember to get user from db as late as possible

        public static IQueryable<Study> GetStudiesIncludingRestrictedForCurrentUser(SepesDbContext db, int userId)
        {

            //As of now, if you have ANY role associated with a study, you can view it
            return db.Studies
                .Include(s => s.StudyParticipants)
                    .ThenInclude(sp => sp.User)
                .Where(s => s.Restricted == false || s.StudyParticipants.Where(sp => sp.UserId == userId).Any() && s.DeletedAt.HasValue == false);
        }


        public static async Task<Study> GetStudyAndCheckAccessOrThrow(SepesDbContext db, IUserService userService, int studyId, UserOperations operation)
        {
            var studyFromDb = await StudyQueries.GetStudyOrThrowAsync(studyId, db);

            ////TODO: remove and fix
            //if (userService.CurrentUserIsAdmin())
            //{
            //    return studyFromDb;
            //}

            //Sponsors should be able to add sandbox
            //TODO: Verify that they should have access to create sandbox for restricted studies in which they don't own
            if (operation == UserOperations.StudyAddRemoveSandbox)
            {
                if (userService.GetCurrentUser().Sponsor) {

                    return studyFromDb;
                } 
            }

            //No study specific roles required
            if (operation == UserOperations.StudyReadOwnRestricted && studyFromDb.Restricted == false)
            {
                return studyFromDb;
            }

            await ThrowIfOperationNotAllowed(userService, studyFromDb, operation);

            return studyFromDb;
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
