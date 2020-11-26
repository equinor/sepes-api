using Sepes.Infrastructure.Dto.Auth;
using System.Collections.Generic;
using System.Linq;

namespace Sepes.Infrastructure.Constants.Auth
{
    public static class AllowedUserOperations
    {
        public static List<OperationPermission> OperationSet = new List<OperationPermission>() {
            OperationPermission.CreateForAuthorizedUser(UserOperation.Study_Read, appliesOnlyToNonHiddenStudies: true), //As a authorized user, you can read any non-hidden Study
            OperationPermission.CreateForAppRole(UserOperation.Study_Read, appliesOnlyToNonHiddenStudies: false, appliesOnlyIfUserIsStudyOwner:false, AppRoles.Admin), //As a admin, you can read any Study, hidden or not, 
            OperationPermission.CreateForStudyRole(UserOperation.Study_Read, appliesOnlyToNonHiddenStudies: false, StudyRoles.SponsorRep, StudyRoles.VendorAdmin, StudyRoles.VendorContributor, StudyRoles.StudyViewer ), //As a admin, you can read any Study, hidden or not, 
                          
            OperationPermission.CreateForAppRole(UserOperation.Study_Create, appliesOnlyToNonHiddenStudies: false, appliesOnlyIfUserIsStudyOwner:false, AppRoles.Admin, AppRoles.Sponsor), //As a admin or sponsor, you can create a Study 

            OperationPermission.CreateForAppRole(UserOperation.Study_Close, appliesOnlyToNonHiddenStudies: false, appliesOnlyIfUserIsStudyOwner:false, AppRoles.Admin, AppRoles.Sponsor),
            OperationPermission.CreateForStudyRole(UserOperation.Study_Close, appliesOnlyToNonHiddenStudies: false, StudyRoles.SponsorRep),

            OperationPermission.CreateForAppRole(UserOperation.Study_Delete, appliesOnlyToNonHiddenStudies: false, appliesOnlyIfUserIsStudyOwner:false, AppRoles.Admin),

           //METADATA
            OperationPermission.CreateForAppRole(UserOperation.Study_Update_Metadata, appliesOnlyToNonHiddenStudies: false, appliesOnlyIfUserIsStudyOwner:false, AppRoles.Admin, AppRoles.Sponsor),
            OperationPermission.CreateForStudyRole(UserOperation.Study_Update_Metadata, appliesOnlyToNonHiddenStudies: false, StudyRoles.SponsorRep),

            //RESULTS AND LEARNINGS, READ
            OperationPermission.CreateForAuthorizedUser(UserOperation.Study_Read_ResultsAndLearnings, appliesOnlyToNonHiddenStudies: true),
            OperationPermission.CreateForAppRole(UserOperation.Study_Read_ResultsAndLearnings, appliesOnlyToNonHiddenStudies: false, appliesOnlyIfUserIsStudyOwner:false, AppRoles.Admin, AppRoles.Sponsor),
            OperationPermission.CreateForStudyRole(UserOperation.Study_Read_ResultsAndLearnings, appliesOnlyToNonHiddenStudies: false, StudyRoles.StudyViewer),

            //RESULTS AND LEARNINGS, UPDATE
            OperationPermission.CreateForAppRole(UserOperation.Study_Update_ResultsAndLearnings, appliesOnlyToNonHiddenStudies: false, appliesOnlyIfUserIsStudyOwner:false, AppRoles.Admin),
            OperationPermission.CreateForAppRole(UserOperation.Study_Update_ResultsAndLearnings, appliesOnlyToNonHiddenStudies: false, appliesOnlyIfUserIsStudyOwner:true, AppRoles.Sponsor),
            OperationPermission.CreateForStudyRole(UserOperation.Study_Update_ResultsAndLearnings, appliesOnlyToNonHiddenStudies: false, StudyRoles.SponsorRep),

            //STUDY, DATASET CRUD
            OperationPermission.CreateForAppRole(UserOperation.Study_AddRemove_Dataset, appliesOnlyToNonHiddenStudies: false, appliesOnlyIfUserIsStudyOwner:false, AppRoles.Admin, AppRoles.Sponsor),

            //STUDY, ADD/REMOVE PARTICIPANT
            OperationPermission.CreateForAppRole(UserOperation.Study_AddRemove_Participant, appliesOnlyToNonHiddenStudies: false, appliesOnlyIfUserIsStudyOwner:false, AppRoles.Admin, AppRoles.Sponsor),
            OperationPermission.CreateForStudyRole(UserOperation.Study_AddRemove_Participant, appliesOnlyToNonHiddenStudies: false, StudyRoles.SponsorRep),

            //SANDBOX, CRUD
            OperationPermission.CreateForAppRole(UserOperation.Study_Crud_Sandbox, appliesOnlyToNonHiddenStudies: false, appliesOnlyIfUserIsStudyOwner:false, AppRoles.Admin, AppRoles.Sponsor),
            OperationPermission.CreateForStudyRole(UserOperation.Study_Crud_Sandbox, appliesOnlyToNonHiddenStudies: false, StudyRoles.SponsorRep),

            //PRE-APPROVED DATASETS
            OperationPermission.CreateForAuthorizedUser(UserOperation.PreApprovedDataset_Read),
            OperationPermission.CreateForAppRole(UserOperation.PreApprovedDataset_Create_Update_Delete, appliesOnlyToNonHiddenStudies: false, appliesOnlyIfUserIsStudyOwner:false, AppRoles.Admin, AppRoles.DatasetAdmin),
        };

        public static IEnumerable<OperationPermission> ForOperationQueryable(UserOperation operation)
        {
            return OperationSet.Where(or => or.Operation == operation);
        }

        public static IEnumerable<OperationPermission> ForLevel(IEnumerable<OperationPermission> source, PermissionLevel level)
        {
            return source.Where(or => or.Level == level);
        }

        public static IEnumerable<OperationPermission> ForAuthorizedUserLevel(IEnumerable<OperationPermission> source)
        {
            return ForLevel(source, PermissionLevel.AuthorizedUser);
        }

        public static IEnumerable<OperationPermission> ForAppRolesLevel(IEnumerable<OperationPermission> source)
        {
            return ForLevel(source, PermissionLevel.AppRoles);
        }

        public static IEnumerable<OperationPermission> ForStudySpecificRolesLevel(IEnumerable<OperationPermission> source)
        {
            return ForLevel(source, PermissionLevel.StudySpecificRole);
        }

        public static IEnumerable<OperationPermission> ForRestrictedStudies(IEnumerable<OperationPermission> source)
        {
            return source.Where(or => or.AppliesOnlyToNonHiddenStudies == false);
        }
    }
}
