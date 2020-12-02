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
            dto.UpdateMetadata = await StudyAccessUtil.HasAccessToOperationForStudy(userService, studyDb, Constants.UserOperation.Study_Update_Metadata);
            dto.CloseStudy = await StudyAccessUtil.HasAccessToOperationForStudy(userService, studyDb, Constants.UserOperation.Study_Close);
            dto.DeleteStudy = await StudyAccessUtil.HasAccessToOperationForStudy(userService, studyDb, Constants.UserOperation.Study_Delete);

            dto.ReadResulsAndLearnings = await StudyAccessUtil.HasAccessToOperationForStudy(userService, studyDb, Constants.UserOperation.Study_Read_ResultsAndLearnings);
            dto.UpdateResulsAndLearnings = await StudyAccessUtil.HasAccessToOperationForStudy(userService, studyDb, Constants.UserOperation.Study_Update_ResultsAndLearnings);

            dto.AddRemoveDataset = await StudyAccessUtil.HasAccessToOperationForStudy(userService, studyDb, Constants.UserOperation.Study_AddRemove_Dataset);
            dto.AddRemoveSandbox = await StudyAccessUtil.HasAccessToOperationForStudy(userService, studyDb, Constants.UserOperation.Study_Crud_Sandbox);
            dto.AddRemoveParticipant = await StudyAccessUtil.HasAccessToOperationForStudy(userService, studyDb, Constants.UserOperation.Study_AddRemove_Participant);           
        }

        public static async Task DecorateDto(IUserService userService, Study studyDb, SandboxPermissionsDto dto)
        {
            dto.Delete = await StudyAccessUtil.HasAccessToOperationForStudy(userService, studyDb, Constants.UserOperation.Study_Crud_Sandbox);
            dto.Update = await StudyAccessUtil.HasAccessToOperationForStudy(userService, studyDb, Constants.UserOperation.Study_Crud_Sandbox);
            dto.EditRules = await StudyAccessUtil.HasAccessToOperationForStudy(userService, studyDb, Constants.UserOperation.Study_Crud_Sandbox); //TODO: Revise, should be based on the edit rules operation?
        }
    }
}
