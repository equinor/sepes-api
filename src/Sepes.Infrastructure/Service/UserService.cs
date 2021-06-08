using Microsoft.Extensions.Configuration;
using Sepes.Common.Constants;
using Sepes.Common.Dto;
using Sepes.Common.Interface;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util.Auth;
using System;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class UserService : IUserService
    {
        UserDto _cachedUser;

        readonly IConfiguration _configuration;
        readonly IUserModelService _userModelService;
        readonly IContextUserService _contextUserService;

        public UserService(IConfiguration configuration, IUserModelService userModelService, IContextUserService contextUserService)
        {
            _configuration = configuration;
            _userModelService = userModelService;
            _contextUserService = contextUserService;
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
            await EnsureUserHasDbEntryAndSetDbIdOnDto(user);

            return user;
        }

        void VerifyDbUserCreatedOrThrow(UserDto user)
        {
            if (user.Id == 0)
            {
                throw new Exception($"Unable to ensure user {user.ObjectId} exists in DB");
            }
        }

        public async Task<UserDto> GetCurrentUserAsync(bool includeDbId = true)
        {
            if (_cachedUser == null)
            {
                _cachedUser = _contextUserService.GetCurrentUser();

                if (includeDbId)
                {
                    await EnsureUserHasDbEntryAndSetDbIdOnDto(_cachedUser);
                }

            }
            else if (includeDbId && _cachedUser.Id == 0)
            {
                await EnsureUserHasDbEntryAndSetDbIdOnDto(_cachedUser);
            }

            return _cachedUser;
        } 

        async Task EnsureUserHasDbEntryAndSetDbIdOnDto(UserDto user)
        {
            var userFromDb = await _userModelService.GetByObjectIdAsync(user.ObjectId);

            if (userFromDb == null)
            {
                await _userModelService.TryCreate(user.ObjectId, user.UserName, user.EmailAddress, user.FullName, user.UserName);

                userFromDb = await _userModelService.GetByObjectIdAsync(user.ObjectId);
            }

            user.Id = userFromDb.Id;

            VerifyDbUserCreatedOrThrow(user);
        }

        public bool IsMockUser()
        {
            var cypressMockUserIdFromConfig = _configuration[ConfigConstants.CYPRESS_MOCK_USER];

            if (string.IsNullOrWhiteSpace(cypressMockUserIdFromConfig))
            {
                return false;
            }

            var currentUserObjectId = _contextUserService.GetCurrentUserObjectId();

            return currentUserObjectId.Equals(cypressMockUserIdFromConfig);
        }

        //public bool IsMockUser(out UserDto mockUser)
        //{
        //    if (IsMockUser())
        //    {
        //        mockUser = GetCurrentUserAsync(true);
        //        return true;
        //    }

        //    mockUser = null;
        //    return false;
        //}

        //UserDto CreateMockUser()
        //{
        //    var cypressMockUserIdFromConfig = _configuration[ConfigConstants.CYPRESS_MOCK_USER];
        //    return MockUserFactory.CreateMockUser(cypressMockUserIdFromConfig);
        //}
    }
}
