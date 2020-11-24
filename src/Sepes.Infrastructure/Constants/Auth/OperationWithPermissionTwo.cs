using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Constants.Auth
{
    public enum PermissionLevel { AuthorizedUser, AppRoles, StudySpecificRole }

    public class OperationWithPermissionTwo
    {
        public UserOperations Operation { get; set; }

        public PermissionLevel Level { get; set; }

        public bool AppliesOnlyToNonHiddenStudies { get; set; }

        public bool AppliesOnlyIfUserIsStudyOwner { get; set; }

        public HashSet<string> AllowedForRoles { get; set; }


        public static OperationWithPermissionTwo CreateForAuthorizedUser(UserOperations operation, bool appliesOnlyToNonHiddenStudies)
        {
            return new OperationWithPermissionTwo() { Operation = operation, Level = PermissionLevel.AuthorizedUser };
        }

        public static OperationWithPermissionTwo CreateForAppRole(UserOperations operation, bool appliesOnlyToNonHiddenStudies, bool appliesOnlyIfUserIsStudyOwner, params string[] allowedForRoles)
        {
            return new OperationWithPermissionTwo() { Operation = operation, Level = PermissionLevel.AppRoles, AllowedForRoles = new HashSet<string>(allowedForRoles), AppliesOnlyToNonHiddenStudies = appliesOnlyToNonHiddenStudies, AppliesOnlyIfUserIsStudyOwner = appliesOnlyIfUserIsStudyOwner };
        }

        public static OperationWithPermissionTwo CreateForStudyRole(UserOperations operation, bool appliesOnlyToNonHiddenStudies, params string[] allowedForRoles)
        {
            return new OperationWithPermissionTwo() { Operation = operation, Level = PermissionLevel.StudySpecificRole, AllowedForRoles = new HashSet<string>(allowedForRoles), AppliesOnlyToNonHiddenStudies = appliesOnlyToNonHiddenStudies };
        }
    }

    public static class TestThisTwo
    {
        public static HashSet<string> Combine(params string[] args)
        {
            return new HashSet<string>(args);
        }

        static List<OperationWithPermissionTwo> OperationsAndRoles = new List<OperationWithPermissionTwo>()
        {
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
            return OperationsAndRoles.Where(or => or.Operation == operation);
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
                allowedForAppRolesQueryable = allowedForAppRolesQueryable.Where(or => or.AppliesOnlyToNonHiddenStudies = false);
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
    }

}
