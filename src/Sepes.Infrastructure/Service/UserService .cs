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
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class UserService : IUserService
    {
        UserDto _cachedUser;

        bool userIsLoadedFromDb;
        bool userIsLoadedFromDbWithStudyParticipants;

        readonly IConfiguration _config;
        readonly IPrincipalService _principalService;
        readonly SepesDbContext _db;
        readonly IMapper _mapper; 
        
        public UserService(IConfiguration config, SepesDbContext db, IPrincipalService principalService, IMapper mapper)
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
                _cachedUser = MapToDtoAndPersistRelevantProperties(userFromDb);
                userIsLoadedFromDb = true;
            }
          
            return _cachedUser;
        }

        public async Task<UserDto> GetCurrentUserWithStudyParticipantsAsync()
        {
            if (userIsLoadedFromDb == false || userIsLoadedFromDbWithStudyParticipants == false)
            {
                var userFromDb = await EnsureDbUserExists(true);             
                _cachedUser = MapToDtoAndPersistRelevantProperties(userFromDb);
                userIsLoadedFromDb = true;
                userIsLoadedFromDbWithStudyParticipants = true;
            }
            return _cachedUser;
        }

        public bool CurrentUserIsAdmin()
        {
            return _principalService.GetPrincipal().IsInRole(AppRoles.Admin);
        }

        public bool CurrentUserIsSponsor()
        {
            return _principalService.GetPrincipal().IsInRole(AppRoles.Sponsor);
        }

        public bool CurrentUserIsDatasetAdmin()
        {
            return _principalService.GetPrincipal().IsInRole(AppRoles.DatasetAdmin);
        }

        public  async Task<UserPermissionDto> GetUserPermissionsAsync()
        {
            var userFromDb = await GetCurrentUserFromDbAsync();

            var result = new UserPermissionDto();
            result.Admin = userFromDb.Admin;
            result.Sponsor = userFromDb.Sponsor;
            result.DatasetAdmin = userFromDb.DatasetAdmin;

            result.CanCreateStudy = userFromDb.Admin || userFromDb.Sponsor;
            result.CanAdministerDatasets = userFromDb.Admin || userFromDb.DatasetAdmin;

            return result;
        }

        UserDto MapToDtoAndPersistRelevantProperties(User user)
        {
            var userDto = _mapper.Map<UserDto>(user);

            if(userDto.ObjectId != _cachedUser.ObjectId)
            {
                throw new Exception($"Error mapping user DTO from DB entry. ObjectId was not equal");
            }

            if (userDto.EmailAddress != _cachedUser.EmailAddress)
            {
                throw new Exception($"Error mapping user DTO from DB entry. EmailAddress was not equal");
            }

            if (userDto.UserName != _cachedUser.UserName)
            {
                throw new Exception($"Error mapping user DTO from DB entry. UserName was not equal");
            }

            //Reload information that comes from principal;
            var principal = _principalService.GetPrincipal();

            userDto.Admin = principal.IsInRole(AppRoles.Admin);
            userDto.Sponsor = principal.IsInRole(AppRoles.Sponsor);
            userDto.DatasetAdmin = principal.IsInRole(AppRoles.DatasetAdmin);
     
            return userDto;
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
            var userFromDb = await queryable.SingleOrDefaultAsync(u => u.ObjectId == _cachedUser.ObjectId);
            return userFromDb;
        }

  
    }
}
