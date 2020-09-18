using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service;
using Sepes.Infrastructure.Service.Interface;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Util
{
    public static class StudyAccessUtil
    {
        public static async Task<Study> GetStudyAndCheckAccessOrThrow(SepesDbContext db, IUserService userService, int studyId, string accessType = AccessType.STUDY_READ)
        {
            var studyFromDb = await StudyQueries.GetStudyOrThrowAsync(studyId, db);

            if (accessType == AccessType.STUDY_READ)
            {
                await StudyAccessUtil.ThrowIfUserCannotViewStudy(userService, studyFromDb);
            }
            else if (accessType == AccessType.STUDY_UPDATE)
            {
                await StudyAccessUtil.ThrowIfUserCannotUpdateStudy(userService, studyFromDb);
            }
            else if (accessType == AccessType.STUDY_DELETE)
            {
                await StudyAccessUtil.ThrowIfUserCannotDeleteStudy(userService, studyFromDb);
            }

            return studyFromDb;
        }



        public static async Task ThrowIfUserCannotViewStudy(IUserService userService, Study study)
        {
            if (await CanViewStudy(userService, study) == false)
            {
                throw new ForbiddenException($"User {userService.GetCurrentUser().Email} does not have access to study {study.Id}");
            }
        }

        public static async Task ThrowIfUserCannotUpdateStudy(IUserService userService, Study study)
        {
            if (await CanViewStudy(userService, study) == false)
            {
                throw new ForbiddenException($"User {userService.GetCurrentUser().Email} does not have access to study {study.Id}");
            }
        }

        public static async Task ThrowIfUserCannotDeleteStudy(IUserService userService, Study study)
        {

            if (!userService.CurrentUserIsAdmin())
            {
                throw new ForbiddenException("This action requires Admin role!");
            }

            //if (await CanViewStudy(userService, study) == false)
            //{
            //    throw new ForbiddenException($"User {userService.GetCurrentUser().Email} does not have access to study {study.Id}");
            //}

        }
        public static async Task<bool> CanViewStudy(IUserService userService, Study study)
        {
            if (study.Restricted == false)
            {
                return true;
            }

            if (study.StudyParticipants == null || study.StudyParticipants.Count == 0)
            {
                return false;
            }

            if (await UserHasRoleForStudy(userService, study,
                StudyRoles.StudyOwner,
                StudyRoles.StudyViewer,
                StudyRoles.VendorAdmin,
                StudyRoles.VendorContributor,
                StudyRoles.SponsorRep))
            {
                return true;
            }

            return false;
        }

        static async Task<bool> UserHasRoleForStudy(IUserService userService, Study study, params string[] requiredRoles)
        {
            var currentUser = await userService.GetCurrentUserFromDbAsync();

            foreach (var curParticipant in study.StudyParticipants.Where(p => p.UserId == currentUser.Id))
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
