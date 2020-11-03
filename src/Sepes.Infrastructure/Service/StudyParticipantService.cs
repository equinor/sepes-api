using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class StudyParticipantService : IStudyParticipantService
    {
        readonly SepesDbContext _db;
        readonly IMapper _mapper;
        readonly IUserService _userService;
        readonly IAzureUserService _azureADUsersService;

        public StudyParticipantService(SepesDbContext db, IMapper mapper, IUserService userService, IAzureUserService azureADUsersService)
        {
            _db = db;
            _mapper = mapper;
            _userService = userService;
            _azureADUsersService = azureADUsersService;
        }

        public async Task<IEnumerable<ParticipantLookupDto>> GetLookupAsync(string searchText, int limit = 30)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return new List<ParticipantLookupDto>();
            }

            var usersFromAzureAdTask = _azureADUsersService.SearchUsersAsync(searchText, limit);
            var usersFromDbTask = _db.Users.Where(u => u.EmailAddress.StartsWith(searchText) || u.FullName.StartsWith(searchText) || u.ObjectId.Equals(searchText)).ToListAsync();

            await Task.WhenAll(usersFromAzureAdTask, usersFromDbTask);

            var usersFromDb = _mapper.Map<IEnumerable<ParticipantLookupDto>>(usersFromDbTask.Result);
            var usersFromDbAsDictionary = new Dictionary<string, ParticipantLookupDto>();

            foreach (var curUserFromDb in usersFromDb)
            {
                if (string.IsNullOrWhiteSpace(curUserFromDb.ObjectId))
                {
                    continue;
                }

                if (!usersFromDbAsDictionary.ContainsKey(curUserFromDb.ObjectId))
                {
                    usersFromDbAsDictionary.Add(curUserFromDb.ObjectId, curUserFromDb);
                }
            }

            var usersFromAzureAd = _mapper.Map<IEnumerable<ParticipantLookupDto>>(usersFromAzureAdTask.Result).ToList();

            foreach (var curAzureUser in usersFromAzureAd)
            {
                if (usersFromDbAsDictionary.ContainsKey(curAzureUser.ObjectId) == false)
                {
                    usersFromDbAsDictionary.Add(curAzureUser.ObjectId, curAzureUser);
                }
            }

            return usersFromDbAsDictionary.OrderBy(o => o.Value.FullName).Select(o => o.Value);
        }

        public async Task<StudyParticipantDto> RemoveParticipantFromStudyAsync(int studyId, int userId, string roleName)
        {
            if (roleName == StudyRoles.StudyOwner)
            {
                throw new ArgumentException($"The Study Owner role cannot be deleted");
            }

            var studyFromDb = await StudyAccessUtil.GetStudyAndCheckAccessOrThrow(_db, _userService, studyId, UserOperations.StudyAddRemoveParticipant);
            var studyParticipantFromDb = studyFromDb.StudyParticipants.FirstOrDefault(p => p.UserId == userId && p.RoleName == roleName);

            if (studyParticipantFromDb == null)
            {
                throw NotFoundException.CreateForEntityCustomDescr("StudyParticipant", $"studyId: {studyId}, userId: {userId}, roleName: {roleName}");
            }

            studyFromDb.StudyParticipants.Remove(studyParticipantFromDb);
            await _db.SaveChangesAsync();

            return _mapper.Map<StudyParticipantDto>(studyParticipantFromDb);
        }

        public async Task<StudyParticipantDto> HandleAddParticipantAsync(int studyId, ParticipantLookupDto user, string role)
        {
            ValidateRoleNameThrowIfInvalid(role);

            if (user.Source == ParticipantSource.Db)
            {
                return await AddDbUserAsParticipantAsync(studyId, user.DatabaseId.Value, role);
            }
            else if (user.Source == ParticipantSource.Azure)
            {
                return await AddAzureUserAsParticipantAsync(studyId, user, role);
            }

            throw new ArgumentException($"Unknown source for user {user.UserName}");
        }


        async Task<StudyParticipantDto> AddDbUserAsParticipantAsync(int studyId, int userId, string role)
        {
            var studyFromDb = await StudyAccessUtil.GetStudyAndCheckAccessOrThrow(_db, _userService, studyId, UserOperations.StudyAddRemoveParticipant);

            if (RoleAllreadyExistsForUser(studyFromDb, userId, role))
            {
                throw new ArgumentException($"Role {role} allready granted for user {userId} on study {studyId}");
            }

            var userFromDb = await _db.Users.FirstOrDefaultAsync(p => p.Id == userId);

            if (userFromDb == null)
            {
                throw NotFoundException.CreateForEntity("User", userId);
            }

            var studyParticipant = new StudyParticipant { StudyId = studyFromDb.Id, UserId = userId, RoleName = role };
            await _db.StudyParticipants.AddAsync(studyParticipant);
            await _db.SaveChangesAsync();

            return _mapper.Map<StudyParticipantDto>(studyParticipant);
        }

        async Task<StudyParticipantDto> AddAzureUserAsParticipantAsync(int studyId, ParticipantLookupDto user, string role)
        {
            // Run validations: (Check if both id's are valid)
            var studyFromDb = await StudyQueries.GetStudyOrThrowAsync(studyId, _db);

            var userFromAzure = await _azureADUsersService.GetUser(user.ObjectId);

            if (userFromAzure == null)
            {
                throw new NotFoundException($"AD User with id {user.ObjectId} not found!");
            }

            var userDb = await _db.Users.FirstOrDefaultAsync(p => p.ObjectId == user.ObjectId);

            if (userDb == null)
            {
                userDb = new User { ObjectId = user.ObjectId, UserName = userFromAzure.Mail, EmailAddress = userFromAzure.Mail, FullName = userFromAzure.DisplayName };
                _db.Users.Add(userDb);
            }
            else
            {
                return await AddDbUserAsParticipantAsync(studyId, userDb.Id, role);
            }

            if (RoleAllreadyExistsForUser(studyFromDb, userDb.Id, role))
            {
                throw new ArgumentException($"Role {role} allready granted for user {user.DatabaseId.Value} on study {studyId}");
            }

            var newStudyParticipant = new StudyParticipant { StudyId = studyFromDb.Id, RoleName = role };
            userDb.StudyParticipants = new List<StudyParticipant> { newStudyParticipant };

            await _db.SaveChangesAsync();
            return _mapper.Map<StudyParticipantDto>(newStudyParticipant);
        }


        bool RoleAllreadyExistsForUser(Study study, int userId, string roleName)
        {
            if (study.StudyParticipants == null)
            {
                throw new ArgumentNullException($"Study not loaded properly. Probably missing include for \"StudyParticipants\"");
            }

            if (study.StudyParticipants.Count == 0)
            {
                return false;
            }

            if (study.StudyParticipants.Where(sp => sp.UserId == userId && sp.RoleName == roleName).Any())
            {
                return true;
            }

            return false;
        }

        void ValidateRoleNameThrowIfInvalid(string role)
        {
            if ((role.Equals(StudyRoles.SponsorRep) ||
                role.Equals(StudyRoles.StudyOwner) ||
                role.Equals(StudyRoles.StudyViewer) ||
                role.Equals(StudyRoles.VendorAdmin) ||
                role.Equals(StudyRoles.VendorContributor) == false))
            {
                throw new ArgumentException($"Invalid Role supplied: {role}");
            }
          
        }
    }
}
