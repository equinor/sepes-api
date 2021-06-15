using System.Collections.Generic;
using Sepes.Common.Dto;
using Sepes.Infrastructure.Model;

namespace Sepes.Provisioning.Service.Interface
{
    public interface IParticipantRoleTranslatorService
    {
        List<CloudResourceDesiredRoleAssignmentDto> CreateDesiredRolesForStudyDatasetResourceGroup(List<StudyParticipant> participants);
        List<CloudResourceDesiredRoleAssignmentDto> CreateDesiredRolesForSandboxResourceGroup(List<StudyParticipant> participants);
    }
}
