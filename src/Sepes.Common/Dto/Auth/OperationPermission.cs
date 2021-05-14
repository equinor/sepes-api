using Sepes.Common.Constants;
using System.Collections.Generic;

namespace Sepes.Common.Dto.Auth
{
    public enum PermissionLevel { AllNonExternalUser, AppRoles, StudySpecificRole }

    public class OperationPermission
    {
        public UserOperation Operation { get; set; }

        public PermissionLevel Level { get; set; }

        public bool AppliesOnlyToNonHiddenStudies { get; set; }

        public bool AppliesOnlyIfUserIsStudyOwner { get; set; }

        public HashSet<string> AllowedForRoles { get; set; }


        public static OperationPermission CreateForAllNonExternalUser(UserOperation operation, bool appliesOnlyToNonHiddenStudies = false)
        {
            return new OperationPermission() { Operation = operation, Level = PermissionLevel.AllNonExternalUser, AppliesOnlyToNonHiddenStudies = appliesOnlyToNonHiddenStudies };
        }

        public static OperationPermission CreateForAppRole(UserOperation operation, bool appliesOnlyToNonHiddenStudies, bool appliesOnlyIfUserIsStudyOwner, params string[] roles)
        {
            return new OperationPermission() { Operation = operation, Level = PermissionLevel.AppRoles, AllowedForRoles = new HashSet<string>(roles), AppliesOnlyToNonHiddenStudies = appliesOnlyToNonHiddenStudies, AppliesOnlyIfUserIsStudyOwner = appliesOnlyIfUserIsStudyOwner };
        }

        public static OperationPermission CreateForStudyRole(UserOperation operation, bool appliesOnlyToNonHiddenStudies, params string[] roles)
        {
            return new OperationPermission() { Operation = operation, Level = PermissionLevel.StudySpecificRole, AllowedForRoles = new HashSet<string>(roles), AppliesOnlyToNonHiddenStudies = appliesOnlyToNonHiddenStudies };
        }
    }
}
