using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Service.Interface;
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
            var userFromDb = await _userService.GetCurrentUserFromDbAsync();

            var result = new UserPermissionDto();
            result.Admin = userFromDb.Admin;
            result.Sponsor = userFromDb.Sponsor;
            result.DatasetAdmin = userFromDb.DatasetAdmin;

            result.CanCreateStudy = userFromDb.Admin || userFromDb.Sponsor;
            result.CanAdministerDatasets = userFromDb.Admin || userFromDb.DatasetAdmin;

            return result;
        } 
    }
}
