using Sepes.Common.Dto;
using Sepes.Infrastructure.Service.Interface;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class UserPermissionService : IUserPermissionService
    {
        readonly IUserService _userService;
        readonly IOperationPermissionService _operationPermissionService;

        public UserPermissionService(IUserService userService, IOperationPermissionService operationPermissionService)
        {
            _userService = userService;
            _operationPermissionService = operationPermissionService;
        }      

        public async Task<UserPermissionDto> GetUserPermissionsAsync()
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

                CanCreateStudy = await _operationPermissionService.HasAccessToOperation(Common.Constants.UserOperation.Study_Create),
                CanRead_PreApproved_Datasets = await _operationPermissionService.HasAccessToOperation(Common.Constants.UserOperation.PreApprovedDataset_Read),
                CanEdit_PreApproved_Datasets = await _operationPermissionService.HasAccessToOperation(Common.Constants.UserOperation.PreApprovedDataset_Create_Update_Delete)
            };

            return result;
        } 
    }
}
