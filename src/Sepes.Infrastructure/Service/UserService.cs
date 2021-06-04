using Microsoft.Extensions.Configuration;
using Sepes.Azure.Dto;
using Sepes.Azure.Service.Interface;
using Sepes.Common.Constants;
using Sepes.Common.Dto;
using Sepes.Common.Interface;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class UserService : IUserService
    {
        UserDto _cachedUser;
       
        readonly IConfiguration _config;

        readonly IUserModelService _userModelService;      
        readonly IContextUserService _contextUserService;
        readonly IAzureUserService _azureUserService;

        public UserService(IConfiguration config, IUserModelService userModelService, IContextUserService contextUserService, IAzureUserService azureUserService)
        {
            _config = config;            
            _userModelService = userModelService;         
            _contextUserService = contextUserService;
            _azureUserService = azureUserService;
        }

        public async Task<UserDto> GetByDbIdAsync(int userId)
        {
            return await _userModelService.GetByIdAsync(userId);
        }

        public async Task<UserDto> GetByObjectIdAsync(string objectId)
        {
            return await _userModelService.GetByObjectIdAsync(objectId);
        }

        public async Task<UserDto> EnsureExists(UserDto user)
        {
            await EnsureDbUserExistsAndSetDbIdOnDto(user);

            if(user.Id == 0)
            {
                throw new Exception($"Unable to ensure user {user.ObjectId} exists in DB");
            }

            return user;
        }

        public async Task<UserDto> GetCurrentUserAsync(bool includeDbId = true)
        {
            if (_cachedUser == null)
            {
                _cachedUser = _contextUserService.GetUser();

                if (includeDbId)
                {
                    await EnsureDbUserExistsAndSetDbIdOnDto(_cachedUser);
                }             
               
            }
            else if(includeDbId && _cachedUser.Id == 0)
            {
                await EnsureDbUserExistsAndSetDbIdOnDto(_cachedUser);
            }

            return _cachedUser;
        }

       

        async Task EnsureDbUserExistsAndSetDbIdOnDto(UserDto user)
        {
            var userFromDb = await _userModelService.GetByObjectIdAsync(user.ObjectId);

            if (userFromDb == null)
            { 
                var cypressMockUser = _config[ConfigConstants.CYPRESS_MOCK_USER];

                AzureUserDto userFromAzure;

                if (!String.IsNullOrWhiteSpace(cypressMockUser) && user.ObjectId.Equals(cypressMockUser))
                {
                    userFromAzure = new AzureUserDto
                    {
                        DisplayName = "Mock User",
                        UserPrincipalName = "mock@user.com",
                        Mail = "mock@user.com"
                    };
                }
                else
                {
                    userFromAzure = await _azureUserService.GetUserAsync(user.ObjectId);

                    if (userFromAzure == null)
                    {
                        throw new Exception($"Unable to get info on logged in user from Azure. User id: {user.ObjectId}");
                    }
                }

                await _userModelService.TryCreate(user.ObjectId, userFromAzure.UserPrincipalName, userFromAzure.Mail, userFromAzure.DisplayName, userFromAzure.UserPrincipalName);

                userFromDb = await _userModelService.GetByObjectIdAsync(user.ObjectId);
            }

            user.Id = userFromDb.Id;
        }
       

        public async Task<bool> IsMockUser()
        {
            var currentUser = await GetCurrentUserAsync(false);
            var cypressMockUser = _config[ConfigConstants.CYPRESS_MOCK_USER];

            if (currentUser.ObjectId.ToLowerInvariant() == cypressMockUser.ToLowerInvariant())
            {
                return true;
            }

            return false;
        }

       
    }
}
