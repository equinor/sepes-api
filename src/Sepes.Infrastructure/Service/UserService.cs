using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Interface;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util.Auth;
using System.Diagnostics;
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

        public async Task<UserDto> GetUserByIdAsync(int userId)
        {
            var userFromDb = await _db.Users.FirstOrDefaultAsync(p => p.Id == userId);
            var userDto = _mapper.Map<UserDto>(userFromDb);
            return userDto;
        }

        async Task<User> EnsureDbUserExists(bool includeParticipantInfo = false)
        {
           var sp = Stopwatch.StartNew();
            return await ThreadSafeUserCreatorUtil.EnsureDbUserExistsAsync(_db, _currentUserService, _azureUserService, includeParticipantInfo);
            var elapsed = sp.ElapsedMilliseconds;
        }     

        UserDto MapToDtoAndPersistRelevantProperties(User user)
        {
            var userDto = _mapper.Map<UserDto>(user);  
            UserUtil.ApplyExtendedProps(_config, _principalService, userDto);
            return userDto;
        }     
    }
}
