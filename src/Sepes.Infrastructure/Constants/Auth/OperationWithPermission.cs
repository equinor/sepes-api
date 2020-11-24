//using Sepes.Infrastructure.Model;
//using Sepes.Infrastructure.Service.Interface;
//using Sepes.Infrastructure.Util;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Sepes.Infrastructure.Constants.Auth
//{
//    public class OperationWithPermission
//    {

//        public UserOperations Operation { get; set; }

//        public bool AppliesOnlyToNonHidden { get; set; }

//        public bool AllowedForAllAuthorizedUsers { get; set; }


//        public HashSet<string> AllowedForAppRoles { get; set; }

//        public HashSet<string> AllowedForStudyRoles { get; set; }

//        public OperationWithPermission(UserOperations operation, bool onlyNonHidden, bool allowedForAllAuthorizedUsers)
//        {
//            Operation = operation;
//            AppliesOnlyToNonHidden = onlyNonHidden;
//            AllowedForAllAuthorizedUsers = allowedForAllAuthorizedUsers;

//        }

//        public OperationWithPermission(UserOperations operation, bool onlyNonHidden, HashSet<string> allowedForAppRoles = null, HashSet<string> allowedForStudyRoles = null)
//        {
//            Operation = operation;
//            AppliesOnlyToNonHidden = onlyNonHidden;

//            if (allowedForAppRoles != null)
//            {
//                AllowedForAppRoles = allowedForAppRoles;
//            }

//            if (allowedForStudyRoles != null)
//            {
//                AllowedForStudyRoles = allowedForStudyRoles;
//            }
//        }
//    }

//    public static class TestThis
//    {
//        public static HashSet<string> Combine(params string[] args)
//        {
//            return new HashSet<string>(args);
//        }

//        static List<OperationWithPermission> OperationsAndRoles = new List<OperationWithPermission>()
//        {
//            //Study roles only apply for those i have this role for
//              new OperationWithPermission(UserOperations.Study_Read, true, allowedForAllAuthorizedUsers: true), //As a authorized user, you can read any non-hidden Study

//             new OperationWithPermission(UserOperations.Study_Read, false, allowedForAppRoles: Combine(AppRoles.Admin), allowedForStudyRoles: Combine(StudyRoles.SponsorRep, StudyRoles.VendorAdmin, StudyRoles.VendorContributor, StudyRoles.StudyViewer)), //As a admin, you can read any Study, hidden or not, As one of thes study roles, you can read owned study, hidden or not            
                 


//            new OperationWithPermission(UserOperations.Study_Create, false, Combine(AppRoles.Admin, AppRoles.Sponsor)),

//            new OperationWithPermission(UserOperations.Study_Close, false, Combine(AppRoles.Admin, AppRoles.Sponsor), Combine(StudyRoles.SponsorRep)),
//            new OperationWithPermission(UserOperations.Study_Delete, false, Combine(AppRoles.Admin)),

//            new OperationWithPermission(UserOperations.Study_Update_Metadata, false, Combine(AppRoles.Admin, AppRoles.Sponsor), Combine(StudyRoles.SponsorRep)),


//             new OperationWithPermission(UserOperations.Study_Read_ResultsAndLearnings, true, allowedForAllAuthorizedUsers: true),
//             new OperationWithPermission(UserOperations.Study_Read_ResultsAndLearnings, true,  allowedForStudyRoles: Combine(StudyRoles.StudyViewer)),
//            new OperationWithPermission(UserOperations.Study_Read_ResultsAndLearnings, false, Combine(AppRoles.Admin, AppRoles.Sponsor), Combine(StudyRoles.SponsorRep)),



//            new OperationWithPermission(UserOperations.Study_Update_ResultsAndLearnings, false, Combine(AppRoles.Admin, AppRoles.Sponsor), Combine(StudyRoles.SponsorRep)),

//            new OperationWithPermission(UserOperations.Study_AddRemove_Dataset, false, Combine(AppRoles.Admin, AppRoles.Sponsor), Combine(StudyRoles.SponsorRep)),
//            new OperationWithPermission(UserOperations.Study_AddRemove_Participant, false, Combine(AppRoles.Admin, AppRoles.Sponsor), Combine(StudyRoles.SponsorRep)),
//            new OperationWithPermission(UserOperations.Study_Crud_Sandbox, false, Combine(AppRoles.Admin, AppRoles.Sponsor), Combine(StudyRoles.SponsorRep)),

//        };

//        public static async Task<bool> CheckStudyAccessOrThrow(IUserService userService, Study study, UserOperations operation)
//        {

//            if (AllowedForAnyAuthorizedUser(study, operation))  //Check for roles for unauthorized
//            {
//                return true;
//            }
//            else if (AllowedBasedOnAppRoles(userService, study, operation))
//            {
//                return true;
//            }
//            else if (await AllowedBasedOnStudyRoles(userService, study, operation))
//            {
//                return true;
//            }  //Check for study roles   

//            return false;

//        }

//        static IEnumerable<OperationWithPermission> ForOperationQueryable(UserOperations operation)
//        {
//            return OperationsAndRoles.Where(or => or.Operation == operation);
//        }


//        static bool AllowedForAnyAuthorizedUser(Study study, UserOperations operation)
//        {
//            if (study.Restricted == false)
//            {
//                return ForOperationQueryable(operation).Where(or => or.AllowedForAllAuthorizedUsers).Any();
//            }

//            return false;
//        }

//        static bool AllowedBasedOnAppRoles(IUserService userService, Study study, UserOperations operation)
//        {
//            var allowedForAppRolesQueryable = OperationsAndRoles.Where(or => or.Operation == operation && or.AllowedForAppRoles != null);

//            if (study.Restricted)
//            {
//                allowedForAppRolesQueryable = allowedForAppRolesQueryable.Where(or => or.AppliesOnlyToNonHidden = false);
//            }

//            var allowedForAppRoles = allowedForAppRolesQueryable.Select(or => or.AllowedForAppRoles).ToList();

//            if (allowedForAppRoles != null && allowedForAppRoles.Count > 0)
//            {
//                var currentUser = userService.GetCurrentUser();

//                foreach (var curOpWithRole in allowedForAppRoles)
//                {
//                    if (curOpWithRole.Intersect(currentUser.AppRoles).Any())
//                    {
//                        return true;
//                    }
//                }
//            }

//            return false;
//        }

//        static async Task<bool> AllowedBasedOnStudyRoles(IUserService userService, Study study, UserOperations operation)
//        {
//            var allowedForStudyRolesQueryable = OperationsAndRoles.Where(or => or.Operation == operation && or.AllowedForStudyRoles != null);

//            if (study.Restricted)
//            {
//                allowedForStudyRolesQueryable = allowedForStudyRolesQueryable.Where(or => or.AppliesOnlyToNonHidden = false);
//            }

//            var allowedForStudyRoles = allowedForStudyRolesQueryable.Select(or => or.AllowedForStudyRoles).ToList();

//            if (allowedForStudyRoles.Count > 0)
//            {
//                var currentUser = await userService.GetCurrentUserFromDbAsync();

//                foreach (var curOpWithRole in allowedForStudyRoles)
//                {
//                    if (StudyAccessUtil.UserHasRequiredStudyRole(currentUser.Id, study, curOpWithRole))
//                    {
//                        return true;
//                    }
//                }
//            }

//            return false;
//        }
//    }

//}
