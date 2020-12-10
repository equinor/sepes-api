using Sepes.Infrastructure.Dto.Auth;
using System.Collections.Generic;
using System.Linq;

namespace Sepes.Infrastructure.Constants.Auth
{
    //This class describes what a user is allowed to do. If a operation is not described here, it's not allowed
    //One line describes allowed actions for a type of user
    //A line can also have limiting properties, prefixed "appliesOnlyTo"
    //Permission to perform an operation can be given in three different levels, therefore, it might take 3 lines to define allowed operations for all relevant user types: 
    //Level 0: Some operations are allowed for all authenticated users, typically limited to NON-hidden studies
    //Level 1: Some operations are allowed based on Application roles, those that are fethed from AD
    //Level 2: Some operations are allowed based on Study specific roles
    public static class AllowedUserOperations
    {
        public static List<OperationPermission> OperationSet = new List<OperationPermission>() {

            //STUDY READ
            OperationPermission.CreateForAllNonExternalUser(UserOperation.Study_Read, appliesOnlyToNonHiddenStudies: true),
            OperationPermission.CreateForAppRole(UserOperation.Study_Read, appliesOnlyToNonHiddenStudies: false, appliesOnlyIfUserIsStudyOwner:false, AppRoles.Admin),
            OperationPermission.CreateForAppRole(UserOperation.Study_Read, appliesOnlyToNonHiddenStudies: false, appliesOnlyIfUserIsStudyOwner:true, AppRoles.Sponsor),
            OperationPermission.CreateForStudyRole(UserOperation.Study_Read, appliesOnlyToNonHiddenStudies: false, StudyRoles.SponsorRep, StudyRoles.SponsorRep, StudyRoles.VendorAdmin, StudyRoles.VendorContributor, StudyRoles.StudyViewer ),
                  
            //STUDY CREATE
            OperationPermission.CreateForAppRole(UserOperation.Study_Create, appliesOnlyToNonHiddenStudies: false, appliesOnlyIfUserIsStudyOwner:false, AppRoles.Admin, AppRoles.Sponsor),

            //STUDY METADATA
            OperationPermission.CreateForAppRole(UserOperation.Study_Update_Metadata, appliesOnlyToNonHiddenStudies: false, appliesOnlyIfUserIsStudyOwner:false, AppRoles.Admin),
            OperationPermission.CreateForAppRole(UserOperation.Study_Update_Metadata, appliesOnlyToNonHiddenStudies: false, appliesOnlyIfUserIsStudyOwner:true, AppRoles.Sponsor),
            OperationPermission.CreateForStudyRole(UserOperation.Study_Update_Metadata, appliesOnlyToNonHiddenStudies: false, StudyRoles.SponsorRep),

            //RESULTS AND LEARNINGS, READ
            OperationPermission.CreateForAllNonExternalUser(UserOperation.Study_Read_ResultsAndLearnings, appliesOnlyToNonHiddenStudies: true),
            OperationPermission.CreateForAppRole(UserOperation.Study_Read_ResultsAndLearnings, appliesOnlyToNonHiddenStudies: false, appliesOnlyIfUserIsStudyOwner:false, AppRoles.Admin, AppRoles.Sponsor),
            OperationPermission.CreateForStudyRole(UserOperation.Study_Read_ResultsAndLearnings, appliesOnlyToNonHiddenStudies: false, StudyRoles.SponsorRep, StudyRoles.StudyViewer),

            //RESULTS AND LEARNINGS, UPDATE
            OperationPermission.CreateForAppRole(UserOperation.Study_Update_ResultsAndLearnings, appliesOnlyToNonHiddenStudies: false, appliesOnlyIfUserIsStudyOwner:false, AppRoles.Admin),
            OperationPermission.CreateForAppRole(UserOperation.Study_Update_ResultsAndLearnings, appliesOnlyToNonHiddenStudies: false, appliesOnlyIfUserIsStudyOwner:true, AppRoles.Sponsor),
            OperationPermission.CreateForStudyRole(UserOperation.Study_Update_ResultsAndLearnings, appliesOnlyToNonHiddenStudies: false, StudyRoles.SponsorRep),

            //STUDY CLOSE
            OperationPermission.CreateForAppRole(UserOperation.Study_Close, appliesOnlyToNonHiddenStudies: false, appliesOnlyIfUserIsStudyOwner:false, AppRoles.Admin, AppRoles.Sponsor),
            OperationPermission.CreateForStudyRole(UserOperation.Study_Close, appliesOnlyToNonHiddenStudies: false, StudyRoles.SponsorRep),

            //STUDY DELETE
            OperationPermission.CreateForAppRole(UserOperation.Study_Delete, appliesOnlyToNonHiddenStudies: false, appliesOnlyIfUserIsStudyOwner:false, AppRoles.Admin),     

            //STUDY, DATASETS
            OperationPermission.CreateForAppRole(UserOperation.Study_AddRemove_Dataset, appliesOnlyToNonHiddenStudies: false, appliesOnlyIfUserIsStudyOwner:false, AppRoles.Admin, AppRoles.Sponsor),
            OperationPermission.CreateForStudyRole(UserOperation.Study_AddRemove_Dataset, appliesOnlyToNonHiddenStudies: false, StudyRoles.SponsorRep),

            //STUDY, ADD/REMOVE PARTICIPANT
            OperationPermission.CreateForAppRole(UserOperation.Study_AddRemove_Participant, appliesOnlyToNonHiddenStudies: false, appliesOnlyIfUserIsStudyOwner:false, AppRoles.Admin),
            OperationPermission.CreateForAppRole(UserOperation.Study_AddRemove_Participant, appliesOnlyToNonHiddenStudies: false, appliesOnlyIfUserIsStudyOwner:true, AppRoles.Sponsor),
            OperationPermission.CreateForStudyRole(UserOperation.Study_AddRemove_Participant, appliesOnlyToNonHiddenStudies: false, StudyRoles.SponsorRep, StudyRoles.VendorAdmin),

            //SANDBOX, CRUD
            OperationPermission.CreateForAppRole(UserOperation.Study_Crud_Sandbox, appliesOnlyToNonHiddenStudies: false, appliesOnlyIfUserIsStudyOwner:false, AppRoles.Admin, AppRoles.Sponsor),
            OperationPermission.CreateForStudyRole(UserOperation.Study_Crud_Sandbox, appliesOnlyToNonHiddenStudies: false, StudyRoles.SponsorRep, StudyRoles.VendorAdmin),

            //PRE-APPROVED DATASETS
            OperationPermission.CreateForAllNonExternalUser(UserOperation.PreApprovedDataset_Read),
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

        public static IEnumerable<OperationPermission> ForAllNonExternalUserLevel(IEnumerable<OperationPermission> source)
        {
            return ForLevel(source, PermissionLevel.AllNonExternalUser);
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
