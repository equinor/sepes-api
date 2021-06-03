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
        readonly ICurrentUserService _currentUserService;
        readonly IContextUserService _principalService;
        readonly IAzureUserService _azureUserService;

        public UserService(IConfiguration config, IUserModelService userModelService, ICurrentUserService currentUserService, IContextUserService principalService, IAzureUserService azureUserService)
        {
            _config = config;            
            _userModelService = userModelService;
            _currentUserService = currentUserService;
            _principalService = principalService;
            _azureUserService = azureUserService;
        }

        public async Task<UserDto> GetByIdAsync(int userId)
        {
            return await _userModelService.GetByIdAsync(userId);
        }       

        public async Task<UserDto> GetCurrentUserAsync()
        {
            if (_cachedUser == null)
            {
                _cachedUser = await EnsureDbUserExists();
            }

            return _cachedUser;
        }

        async Task<UserDto> EnsureDbUserExists()
        {
            var loggedInUserObjectId = _currentUserService.GetUserId();

            var userFromDb = await _userModelService.GetByObjectIdAsync(loggedInUserObjectId);

            if (userFromDb == null)
            {

                var cypressMockUser = _config[ConfigConstants.CYPRESS_MOCK_USER];

                AzureUserDto userFromAzure;

                if (!String.IsNullOrWhiteSpace(cypressMockUser) && loggedInUserObjectId.Equals(cypressMockUser))
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
                    userFromAzure = await _azureUserService.GetUserAsync(loggedInUserObjectId);

                    if (userFromAzure == null)
                    {
                        throw new Exception($"Unable to get info on logged in user from Azure. User id: {loggedInUserObjectId}");
                    }
                }

                await _userModelService.TryCreate(loggedInUserObjectId, userFromAzure.UserPrincipalName, userFromAzure.Mail, userFromAzure.DisplayName, userFromAzure.UserPrincipalName);

                userFromDb = await _userModelService.GetByObjectIdAsync(loggedInUserObjectId);

            }

            ApplyExtendedProps(userFromDb);
            return userFromDb;
        }

        void ApplyExtendedProps(UserDto user)
        {
            if (_principalService.IsAdmin())
            {
                user.Admin = true;
                user.AppRoles.Add(AppRoles.Admin);
            }

            if (_principalService.IsSponsor())
            {
                user.Sponsor = true;
                user.AppRoles.Add(AppRoles.Sponsor);
            }

            if (_principalService.IsDatasetAdmin())
            {
                user.DatasetAdmin = true;
                user.AppRoles.Add(AppRoles.DatasetAdmin);
            }

            if (_principalService.IsEmployee())
            {
                user.Employee = true;
            }
        }

        public async Task<bool> IsMockUser()
        {
            var currentUser = await GetCurrentUserAsync();
            var cypressMockUser = _config[ConfigConstants.CYPRESS_MOCK_USER];

            if (currentUser.ObjectId.ToLowerInvariant() == cypressMockUser.ToLowerInvariant())
            {
                return true;
            }

            return false;
        }
    }
}
