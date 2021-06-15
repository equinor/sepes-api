using Sepes.Common.Dto;
using Sepes.Infrastructure.Service.Interface;
using System.Threading.Tasks;
using Sepes.Infrastructure.Util.Auth;

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
            var currentUser = await _userService.GetCurrentUserAsync();

            var result = new UserPermissionDto
            {
                FullName = currentUser.FullName,
                EmailAddress = currentUser.EmailAddress,
                UserName = currentUser.UserName,

                Admin = currentUser.Admin,
                Sponsor = currentUser.Sponsor,
                DatasetAdmin = currentUser.DatasetAdmin,

                CanCreateStudy = OperationAccessUtil.HasAccessToOperation(currentUser, Common.Constants.UserOperation.Study_Create),
                CanRead_PreApproved_Datasets = OperationAccessUtil.HasAccessToOperation(currentUser, Common.Constants.UserOperation.PreApprovedDataset_Read),
                CanEdit_PreApproved_Datasets = OperationAccessUtil.HasAccessToOperation(currentUser, Common.Constants.UserOperation.PreApprovedDataset_Create_Update_Delete)
            };

            return result;
        } 
    }
}
