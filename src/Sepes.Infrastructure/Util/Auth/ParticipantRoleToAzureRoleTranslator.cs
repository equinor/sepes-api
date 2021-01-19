using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Constants.Auth;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Service.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Util.Auth
{
    public static class ParticipantRoleToAzureRoleTranslator
    {
        public static List<CloudResourceDesiredRoleAssignmentDto> CreateListOfDesiredRoles(List<StudyParticipant> participants)
        {
            var desiredRolesLookup = new Dictionary<string, CloudResourceDesiredRoleAssignmentDto>();

            foreach (var curParticipant in participants)
            {
                if (Translate(curParticipant.RoleName, out string translatedRoleId))
                {
                    if (desiredRolesLookup.ContainsKey(translatedRoleId) == false)
                    {
                        desiredRolesLookup.Add(translatedRoleId, new CloudResourceDesiredRoleAssignmentDto(curParticipant.User.ObjectId, translatedRoleId));
                    }
                }
            }

            return desiredRolesLookup.Values.ToList();
        }

        public static async Task TranslateAndAddBasedOnParticipantListForScheduledResourceGroup(ICloudResourceRoleAssignmentCreateService cloudResourceRoleAssignmentCreateService, int resourceDbId, List<StudyParticipant> participants)
        {
            foreach (var curParticipant in participants)
            {
                if (Translate(curParticipant.RoleName, out string translatedRoleId))
                {
                    await cloudResourceRoleAssignmentCreateService.AddAsync(resourceDbId, curParticipant.User.ObjectId, translatedRoleId);
                }
            }
        }

        public static async Task TranslateAndAddBasedOnParticipantList(ICloudResourceRoleAssignmentCreateService cloudResourceRoleAssignmentCreateService, int resourceDbId, List<StudyParticipant> participants)
        {
            foreach (var curParticipant in participants)
            {
                if (Translate(curParticipant.RoleName, out string translatedRoleId))
                {
                    await cloudResourceRoleAssignmentCreateService.AddAsync(resourceDbId, curParticipant.User.ObjectId, translatedRoleId);
                }
            }
        }

        public static bool Translate(string studyParticipantRole, out string translatedRole)
        {
            switch (studyParticipantRole)
            {
                case StudyRoles.StudyOwner:
                    translatedRole = AzureRoleIds.CONTRIBUTOR;
                    return true;
                case StudyRoles.SponsorRep:
                    translatedRole = AzureRoleIds.READ;
                    return true;
                case StudyRoles.VendorAdmin:
                    translatedRole = AzureRoleIds.READ;
                    return true;
                case StudyRoles.VendorContributor:
                    translatedRole = AzureRoleIds.READ;
                    return true;
            }

            translatedRole = null;
            return false;
        }
    }
}
