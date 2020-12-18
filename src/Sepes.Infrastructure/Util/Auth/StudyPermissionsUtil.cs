using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Dto.Study;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util.Auth;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Util
{
    public static class StudyPermissionsUtil
    {
      public static async Task DecorateDto(IUserService userService, Study studyDb, StudyPermissionsDto dto)
        {
            var currentUser = await userService.GetCurrentUserWithStudyParticipantsAsync();

            dto.UpdateMetadata = StudyAccessUtil.HasAccessToOperationForStudy(currentUser, studyDb, Constants.UserOperation.Study_Update_Metadata);
            dto.CloseStudy = StudyAccessUtil.HasAccessToOperationForStudy(currentUser, studyDb, Constants.UserOperation.Study_Close);
            dto.DeleteStudy = StudyAccessUtil.HasAccessToOperationForStudy(currentUser, studyDb, Constants.UserOperation.Study_Delete);

            dto.ReadResulsAndLearnings = StudyAccessUtil.HasAccessToOperationForStudy(currentUser, studyDb, Constants.UserOperation.Study_Read_ResultsAndLearnings);
            dto.UpdateResulsAndLearnings = StudyAccessUtil.HasAccessToOperationForStudy(currentUser, studyDb, Constants.UserOperation.Study_Update_ResultsAndLearnings);

            dto.AddRemoveDataset = StudyAccessUtil.HasAccessToOperationForStudy(currentUser, studyDb, Constants.UserOperation.Study_AddRemove_Dataset);
            dto.AddRemoveSandbox = StudyAccessUtil.HasAccessToOperationForStudy(currentUser, studyDb, Constants.UserOperation.Study_Crud_Sandbox);
            dto.AddRemoveParticipant = StudyAccessUtil.HasAccessToOperationForStudy(currentUser, studyDb, Constants.UserOperation.Study_AddRemove_Participant);           
        }

        public static async Task DecorateDto(IUserService userService, Study studyDb, SandboxPermissionsDto dto, SandboxPhase phase)
        {
            var currentUser = await userService.GetCurrentUserWithStudyParticipantsAsync();

            dto.Delete = StudyAccessUtil.HasAccessToOperationForStudy(currentUser, studyDb, Constants.UserOperation.Study_Crud_Sandbox);
            dto.Update = StudyAccessUtil.HasAccessToOperationForStudy(currentUser, studyDb, Constants.UserOperation.Study_Crud_Sandbox);
            dto.EditInboundRules = StudyAccessUtil.HasAccessToOperationForStudy(currentUser, studyDb, Constants.UserOperation.Sandbox_EditInboundRules);
            dto.OpenInternet = phase > SandboxPhase.Open ? currentUser.Admin : StudyAccessUtil.HasAccessToOperationForStudy(currentUser, studyDb, Constants.UserOperation.Sandbox_OpenInternet);
            dto.IncreasePhase = StudyAccessUtil.HasAccessToOperationForStudy(currentUser, studyDb, Constants.UserOperation.Sandbox_IncreasePhase);
        }
    }
}
