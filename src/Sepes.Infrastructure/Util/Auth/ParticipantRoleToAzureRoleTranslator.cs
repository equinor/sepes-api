using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Constants.Auth;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sepes.Infrastructure.Util.Auth
{
    public static class ParticipantRoleToAzureRoleTranslator
    {
        public static List<CloudResourceDesiredRoleAssignmentDto> CreateDesiredRolesForStudyResourceGroup(List<StudyParticipant> participants)
        {
            return CreateDesiredRolesWithTranslator(participants, TranslateForStudyResourceGroup);
        }

        public static List<CloudResourceDesiredRoleAssignmentDto> CreateDesiredRolesForSandboxResourceGroup(List<StudyParticipant> participants)
        {
            return CreateDesiredRolesWithTranslator(participants, TranslateForSandboxResourceGroup);            
        }      

        static List<CloudResourceDesiredRoleAssignmentDto> CreateDesiredRolesWithTranslator(List<StudyParticipant> participants, Func<string, string> translator)
        {
            var desiredRolesLookup = new Dictionary<Tuple<string, string>, CloudResourceDesiredRoleAssignmentDto>();

            foreach (var curParticipant in participants)
            {
                var translatedRole = translator(curParticipant.RoleName);

                if (!String.IsNullOrWhiteSpace(translatedRole))
                {
                    var lookupKey = CreateAssignmentLookupKey(curParticipant.User.ObjectId, translatedRole);

                    if (!desiredRolesLookup.ContainsKey(lookupKey))
                    {
                        desiredRolesLookup.Add(lookupKey, new CloudResourceDesiredRoleAssignmentDto(curParticipant.User.ObjectId, translatedRole));
                    }
                }
            }

            return desiredRolesLookup.Values.ToList();
        }

        static Tuple<string, string> CreateAssignmentLookupKey(string principalId, string roleId)
        {
            return new Tuple<string, string>(principalId, roleId);
        }

        public static string TranslateForStudyResourceGroup(string studyParticipantRole)
        {
            return studyParticipantRole switch
            {
                StudyRoles.StudyOwner => AzureRoleIds.READ,
                StudyRoles.SponsorRep => AzureRoleIds.READ,
                _ => null,
            };
        }

        public static string TranslateForSandboxResourceGroup(string studyParticipantRole)
        {
            return studyParticipantRole switch
            {
                StudyRoles.StudyOwner => AzureRoleIds.CONTRIBUTOR,
                StudyRoles.SponsorRep => AzureRoleIds.READ,
                StudyRoles.VendorAdmin => AzureRoleIds.READ,
                StudyRoles.VendorContributor => AzureRoleIds.READ,
                _ => null,
            };
        }
    }
}
