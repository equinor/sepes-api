using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Interface;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util.Auth;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class UserService : IUserService
    {
        UserDto _cachedUser;

        readonly IConfiguration _config;
        readonly SepesDbContext _db;
        readonly IMapper _mapper;

        ICurrentUserService _currentUserService;
        IPrincipalService _principalService;
        IAzureUserService _azureUserService;

        public UserService(IConfiguration config, SepesDbContext db, IMapper mapper, ICurrentUserService currentUserService, IPrincipalService principalService, IAzureUserService azureUserService)
        {
            _config = config;
            _db = db;           
            _mapper = mapper;
            _currentUserService = currentUserService;
            _principalService = principalService;
            _azureUserService = azureUserService;
        }
        public async Task<UserDto> GetCurrentUserAsync()
        {
            if (_cachedUser == null)
            {
                var userFromDb = await EnsureDbUserExists(false);
                _cachedUser = MapToDtoAndPersistRelevantProperties(userFromDb);
            }

            return _cachedUser;
        }     

        public async Task<UserDto> GetCurrentUserWithStudyParticipantsAsync()
        {
            if (_cachedUser == null || (_cachedUser != null && _cachedUser.StudyParticipants == null))
            {
                var userFromDb = await EnsureDbUserExists(true);
                _cachedUser = MapToDtoAndPersistRelevantProperties(userFromDb);
            }

            return _cachedUser;
        }

        async Task<User> EnsureDbUserExists(bool includeParticipantInfo = false)
        {
            var loggedInUserId = _currentUserService.GetUserId();

            var userFromDb = await GetUserFromDb(loggedInUserId, includeParticipantInfo);

            if (userFromDb == null)
            {
                var userFromAzure = await _azureUserService.GetUserAsync(loggedInUserId);

                if(userFromAzure == null)
                {
                    throw new Exception($"Unable to get info on logged in user from Azure. User id: {loggedInUserId}");
                }

                var newDbUser = UserUtil.CreateDbUserFromAzureUser(loggedInUserId, userFromAzure);
                _db.Users.Add(newDbUser);
                await _db.SaveChangesAsync();  
            }

            return userFromDb;
        }

        async Task<User> GetUserFromDb(string userId, bool includeParticipantInfo = false)
        {
            var queryable = GetUserQueryable(includeParticipantInfo);
            var userFromDb = await queryable.SingleOrDefaultAsync(u => u.ObjectId == userId);
            return userFromDb;
        }

        UserDto MapToDtoAndPersistRelevantProperties(User user)
        {
            var userDto = _mapper.Map<UserDto>(user);  
            UserUtil.ApplyExtendedProps(_config, _principalService, userDto);
            return userDto;
        }      

        IQueryable<User> GetUserQueryable(bool includeParticipantInfo = false)
        {
            if (includeParticipantInfo)
            {
                return _db.Users.Include(u => u.StudyParticipants).ThenInclude(sp=> sp.Study);
            }
            else
            {
                return _db.Users;
            }
        }

        public async Task<UserDto> GetUserByIdAsync(int userId)
        {
            var userFromDb = await _db.Users.FirstOrDefaultAsync(p => p.Id == userId);
            var userDto = _mapper.Map<UserDto>(userFromDb);
            return userDto;
        }
    }
}
