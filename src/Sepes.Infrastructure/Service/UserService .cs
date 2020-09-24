using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Interface;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class UserService : IUserService
    {
        readonly IConfiguration _config;
        readonly IHasPrincipal _principalService;
        readonly SepesDbContext _db;
        readonly IMapper _mapper;
        UserDto _cachedUser;
        bool userIsLoadedFromDb;
        bool userIsLoadedFromDbWithStudyParticipants;

        public UserService(IConfiguration config, SepesDbContext db, IHasPrincipal principalService, IMapper mapper)
        {
            _config = config;
            _db = db;
            _principalService = principalService;
            _mapper = mapper;

            _cachedUser = CreateUserFromPrincipal();
        }

        public UserDto GetCurrentUser()
        {
            return _cachedUser;
        }

        public async Task<UserDto> GetCurrentUserFromDbAsync()
        {
            if (userIsLoadedFromDb == false)
            {
                var userFromDb = await EnsureDbUserExists(false);
                _cachedUser = MapToDtoAndAttachPrincipal(userFromDb);
                userIsLoadedFromDb = true;
            }
          
            return _cachedUser;
        }

        public async Task<UserDto> GetCurrentUserWithStudyParticipantsAsync()
        {
            if (userIsLoadedFromDb == false || userIsLoadedFromDbWithStudyParticipants == false)
            {
                var userFromDb = await EnsureDbUserExists(true);             
                _cachedUser = MapToDtoAndAttachPrincipal(userFromDb);
                userIsLoadedFromDb = true;
                userIsLoadedFromDbWithStudyParticipants = true;
            }
            return _cachedUser;
        }

        public bool CurrentUserIsAdmin()
        {
            return _cachedUser.Principal.IsInRole(AppRoles.Admin);
        }

        UserDto MapToDtoAndAttachPrincipal(User user)
        {
            var mapped = _mapper.Map<UserDto>(user);
            mapped.Principal = _cachedUser.Principal;
            return mapped;
        }

        UserDto CreateUserFromPrincipal()
        {
            var user = UserUtil.CreateSepesUser(_config, _principalService.GetPrincipal());
            return user;
        }

        IQueryable<User> GetUserQueryable(bool includeParticipantInfo = false)
        {
            if (includeParticipantInfo)
            {
                return _db.Users.Include(u => u.StudyParticipants);
            }
            else
            {
                return _db.Users;
            }
        }

        async Task<User> EnsureDbUserExists(bool includeParticipantInfo = false)
        {
            var userFromDb = await GetUserFromDb(includeParticipantInfo);

            if (userFromDb == null)
            {
                userFromDb = _mapper.Map<User>(_cachedUser);
                _db.Users.Add(userFromDb);
                await _db.SaveChangesAsync();
            }
            else
            {
                //Ensure details are correct
                userFromDb.EmailAddress = _cachedUser.EmailAddress;
                userFromDb.FullName = _cachedUser.FullName;
                userFromDb.UserName = _cachedUser.UserName;
                await _db.SaveChangesAsync();
            }

            return userFromDb;
        }

        async Task<User> GetUserFromDb(bool includeParticipantInfo = false)
        {
            var queryable = GetUserQueryable(includeParticipantInfo);
            var userFromDb = await queryable.SingleOrDefaultAsync(u => u.TenantId == _cachedUser.TenantId && u.ObjectId == _cachedUser.ObjectId);
            return userFromDb;
        }       
    }
}
