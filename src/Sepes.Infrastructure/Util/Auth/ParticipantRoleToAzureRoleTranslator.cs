using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Constants.Auth;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Util.Auth
{
    public static class ParticipantRoleToAzureRoleTranslator
    {
        public static async Task TranslateAndAddBasedOnParticipantListForScheduledResourceGroup(ICloudResourceRoleAssignmentCreateService cloudResourceRoleAssignmentCreateService, int resourceDbId, string targetResourceId, List<StudyParticipant> participants)
        {
            foreach (var curParticipant in participants)
            {
                var translatedRoleId = Translate(curParticipant.RoleName);                
                await cloudResourceRoleAssignmentCreateService.AddAsync(resourceDbId, curParticipant.User.ObjectId, translatedRoleId);
            }
        }
        public static async Task TranslateAndAddBasedOnParticipantList(ICloudResourceRoleAssignmentCreateService cloudResourceRoleAssignmentCreateService, int resourceDbId, string targetResourceId, List<StudyParticipant> participants)
        {
            foreach (var curParticipant in participants)
            {
                var translatedRoleId = Translate(curParticipant.RoleName);
                var roleDefinitionId = AzureRoleIds.CreateUrl(targetResourceId, translatedRoleId);
                await cloudResourceRoleAssignmentCreateService.AddAsync(resourceDbId, curParticipant.User.ObjectId, roleDefinitionId);
            }
        }

        public static string Translate(string studyParticipantRole)
        {
            switch (studyParticipantRole)
            {
                case StudyRoles.StudyOwner:
                    return AzureRoleIds.READ;

            }

            throw new ArgumentException($"Unable to resolve resource role assignment from study participant role: {studyParticipantRole}");
        }
    }
}
