using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
        readonly IHasPrincipal _principalService;
        readonly SepesDbContext _db;
        readonly IMapper _mapper;
        readonly UserDto _userBasedOnPrincipal;

        public UserService(SepesDbContext db, IHasPrincipal principalService, IMapper mapper)
        {
            _db = db;
            _principalService = principalService;
            _mapper = mapper;

            _userBasedOnPrincipal = CreateUserFromPrincipal();
        }

        public UserDto GetCurrentUser()
        {
            return _userBasedOnPrincipal;
        }

        public async Task<UserDto> GetCurrentUserFromDbAsync()
        {
            var userFromDb = await EnsureDbUserExists(false);
            return MapToDtoAndAttachPrincipal(userFromDb);
        }

        public async Task<UserDto> GetCurrentUserWithStudyParticipantsAsync()
        {
            var userFromDb = await EnsureDbUserExists(true);
            return MapToDtoAndAttachPrincipal(userFromDb);
        }

        public bool CurrentUserIsAdmin()
        {
            return _userBasedOnPrincipal.Principal.IsInRole(AppRoles.Admin);
        }

        UserDto MapToDtoAndAttachPrincipal(User user)
        {
            var mapped = _mapper.Map<UserDto>(user);
            mapped.Principal = _userBasedOnPrincipal.Principal;
            return mapped;
        }

        UserDto CreateUserFromPrincipal()
        {
            var user = UserUtil.CreateSepesUser(_principalService.GetPrincipal());
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
                userFromDb = _mapper.Map<User>(_userBasedOnPrincipal);
                _db.Users.Add(userFromDb);
                await _db.SaveChangesAsync();
            }
            else
            {
                //Ensure details are correct
                userFromDb.EmailAddress = _userBasedOnPrincipal.Email;
                userFromDb.FullName = _userBasedOnPrincipal.FullName;
                userFromDb.UserName = _userBasedOnPrincipal.UserName;
                await _db.SaveChangesAsync();
            }

            return userFromDb;
        }

        async Task<User> GetUserFromDb(bool includeParticipantInfo = false)
        {
            var queryable = GetUserQueryable(includeParticipantInfo);
            var userFromDb = await queryable.SingleOrDefaultAsync(u => u.TenantId == _userBasedOnPrincipal.TenantId && u.ObjectId == _userBasedOnPrincipal.ObjectId);
            return userFromDb;
        }

       
    }
}
