using Sepes.Common.Dto.Dataset;
using Sepes.Common.Dto.Study;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Service.Interface;
using System.Threading.Tasks;
using Sepes.Common.Model;
using Sepes.Common.Response.Sandbox;
using Sepes.Infrastructure.Util.Auth;

namespace Sepes.Infrastructure.Util
{
    public static class StudyPermissionsUtil
    {
      public static async Task DecorateDto(IUserService userService, Study studyDb, StudyPermissionsDto dto)
        {
            var currentUser = await userService.GetCurrentUserAsync();

            dto.UpdateMetadata = StudyAccessUtil.HasAccessToOperationForStudy(currentUser, studyDb, Common.Constants.UserOperation.Study_Update_Metadata);
            dto.CloseStudy = StudyAccessUtil.HasAccessToOperationForStudy(currentUser, studyDb, Common.Constants.UserOperation.Study_Close);
            dto.DeleteStudy = StudyAccessUtil.HasAccessToOperationForStudy(currentUser, studyDb, Common.Constants.UserOperation.Study_Delete);

            dto.ReadResulsAndLearnings = StudyAccessUtil.HasAccessToOperationForStudy(currentUser, studyDb, Common.Constants.UserOperation.Study_Read_ResultsAndLearnings);
            dto.UpdateResulsAndLearnings = StudyAccessUtil.HasAccessToOperationForStudy(currentUser, studyDb, Common.Constants.UserOperation.Study_Update_ResultsAndLearnings);

            dto.AddRemoveDataset = StudyAccessUtil.HasAccessToOperationForStudy(currentUser, studyDb, Common.Constants.UserOperation.Study_AddRemove_Dataset);
            dto.AddRemoveSandbox = StudyAccessUtil.HasAccessToOperationForStudy(currentUser, studyDb, Common.Constants.UserOperation.Study_Crud_Sandbox);
            dto.AddRemoveParticipant = StudyAccessUtil.HasAccessToOperationForStudy(currentUser, studyDb, Common.Constants.UserOperation.Study_AddRemove_Participant);           
        }

        public static async Task DecorateDto(IUserService userService, Study studyDb, SandboxPermissions sandboxPermissions, SandboxPhase phase)
        {
            var currentUser = await userService.GetCurrentUserAsync();

            sandboxPermissions.Delete = StudyAccessUtil.HasAccessToOperationForStudy(currentUser, studyDb, Common.Constants.UserOperation.Study_Crud_Sandbox);
            sandboxPermissions.Update = StudyAccessUtil.HasAccessToOperationForStudy(currentUser, studyDb, Common.Constants.UserOperation.Study_Crud_Sandbox);
            sandboxPermissions.EditInboundRules = StudyAccessUtil.HasAccessToOperationForStudy(currentUser, studyDb, Common.Constants.UserOperation.Sandbox_EditInboundRules);
            sandboxPermissions.OpenInternet = phase > SandboxPhase.Open ? currentUser.Admin : StudyAccessUtil.HasAccessToOperationForStudy(currentUser, studyDb, Common.Constants.UserOperation.Sandbox_OpenInternet); //TODO: was it really only admin who could do this?
            sandboxPermissions.IncreasePhase = StudyAccessUtil.HasAccessToOperationForStudy(currentUser, studyDb, Common.Constants.UserOperation.Sandbox_IncreasePhase);
        }

        public static async Task DecorateDtoStudySpecific(IUserService userService, Study studyDb, DatasetPermissionsDto dto)
        {
            var currentUser = await userService.GetCurrentUserAsync();

            dto.EditDataset = StudyAccessUtil.HasAccessToOperationForStudy(currentUser, studyDb, Common.Constants.UserOperation.Study_AddRemove_Dataset);
            dto.DeleteDataset = StudyAccessUtil.HasAccessToOperationForStudy(currentUser, studyDb, Common.Constants.UserOperation.Study_AddRemove_Dataset);          
        }

        public static async Task DecorateDtoPreApproved(IUserService userService, Study studyDb, DatasetPermissionsDto dto)
        {
            var currentUser = await userService.GetCurrentUserAsync();

            dto.EditDataset = StudyAccessUtil.HasAccessToOperationForStudy(currentUser, studyDb, Common.Constants.UserOperation.PreApprovedDataset_Create_Update_Delete);
            dto.DeleteDataset = StudyAccessUtil.HasAccessToOperationForStudy(currentUser, studyDb, Common.Constants.UserOperation.PreApprovedDataset_Create_Update_Delete);
        }
    }
}
