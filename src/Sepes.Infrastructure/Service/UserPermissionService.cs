using Sepes.Common.Dto;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Common.Util.Auth;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class UserPermissionService : IUserPermissionService
    {
        readonly IUserService _userService;      
        
        public UserPermissionService(IUserService userService)
        {
            _userService = userService;
        }      

        public  async Task<UserPermissionDto> GetUserPermissionsAsync()
        {
            var userFromDb = await _userService.GetCurrentUserAsync();

            var result = new UserPermissionDto();

            result.FullName = userFromDb.FullName;
            result.EmailAddress = userFromDb.EmailAddress;
            result.UserName = userFromDb.UserName;

            result.Admin = userFromDb.Admin;
            result.Sponsor = userFromDb.Sponsor;
            result.DatasetAdmin = userFromDb.DatasetAdmin;

            var currentUser = await _userService.GetCurrentUserAsync();

            result.CanCreateStudy = StudyAccessUtil.HasAccessToOperation(currentUser, Constants.UserOperation.Study_Create);
            result.CanRead_PreApproved_Datasets = StudyAccessUtil.HasAccessToOperation(currentUser, Constants.UserOperation.PreApprovedDataset_Read);
            result.CanEdit_PreApproved_Datasets = StudyAccessUtil.HasAccessToOperation(currentUser, Constants.UserOperation.PreApprovedDataset_Create_Update_Delete);

            return result;
        } 
    }
}
