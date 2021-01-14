using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Study;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Service.Queries;
using Sepes.Infrastructure.Util;
using Sepes.Infrastructure.Util.Auth;
using Sepes.Infrastructure.Util.Provisioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class StudyParticipantService : IStudyParticipantService
    {
        readonly SepesDbContext _db;
        readonly IMapper _mapper;
        readonly IUserService _userService;
        readonly IAzureUserService _azureADUsersService;
        readonly ICloudResourceRoleAssignmentCreateService _cloudResourceRoleAssignmentCreateService;
        readonly ICloudResourceRoleAssignmentUpdateService _cloudResourceRoleAssignmentUpdateService;
        readonly ICloudResourceOperationCreateService _cloudResourceOperationCreateService;

        readonly IProvisioningQueueService _workQueue;

        public StudyParticipantService(SepesDbContext db,
            IMapper mapper,
            IUserService userService,
            IAzureUserService azureADUsersService,
            ICloudResourceOperationCreateService cloudResourceOperationCreateService,
            ICloudResourceRoleAssignmentCreateService cloudResourceRoleAssignmentCreateService,
            ICloudResourceRoleAssignmentUpdateService cloudResourceRoleAssignmentUpdateService,
            IProvisioningQueueService workQueue)
        {
            _db = db;
            _mapper = mapper;
            _userService = userService;
            _azureADUsersService = azureADUsersService;
            _cloudResourceOperationCreateService = cloudResourceOperationCreateService;
            _cloudResourceRoleAssignmentCreateService = cloudResourceRoleAssignmentCreateService;
            _cloudResourceRoleAssignmentUpdateService = cloudResourceRoleAssignmentUpdateService;
            _workQueue = workQueue;
        }

        public async Task<IEnumerable<ParticipantLookupDto>> GetLookupAsync(string searchText, int limit = 30, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return new List<ParticipantLookupDto>();
            }

            var usersFromAzureAdTask = _azureADUsersService.SearchUsersAsync(searchText, limit, cancellationToken);
            var usersFromDbTask = _db.Users.Where(u => u.EmailAddress.StartsWith(searchText) || u.FullName.StartsWith(searchText) || u.ObjectId.Equals(searchText)).ToListAsync(cancellationToken);

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

        public async Task<StudyParticipantDto> AddAsync(int studyId, ParticipantLookupDto user, string role)
        {
            try
            {
                ValidateRoleNameThrowIfInvalid(role);

                var studyFromDb = await StudySingularQueries.GetStudyByIdCheckAccessOrThrow(_db, _userService, studyId, UserOperation.Study_AddRemove_Participant, false, role);

                if (user.Source == ParticipantSource.Db)
                {
                    return await AddDbUserAsync(studyFromDb, user.DatabaseId.Value, role);
                }
                else if (user.Source == ParticipantSource.Azure)
                {
                    return await AddAzureUserAsync(studyFromDb, user, role);
                }

                throw new ArgumentException($"Unknown source for user {user.UserName}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Add participant failed: {ex.Message}", ex);
            }
        }

        async Task<StudyParticipantDto> AddDbUserAsync(Study studyFromDb, int userId, string role)
        {
            if (RoleAllreadyExistsForUser(studyFromDb, userId, role))
            {
                throw new ArgumentException($"Role {role} allready granted for user {userId} on study {studyFromDb.Id}");
            }

            var userFromDb = await _userService.GetUserByIdAsync(userId);

            if (userFromDb == null)
            {
                throw NotFoundException.CreateForEntity("User", userId);
            }

            StudyParticipant createdStudyParticipant = null;

            try
            {
                createdStudyParticipant = new StudyParticipant { StudyId = studyFromDb.Id, UserId = userId, RoleName = role };
                await _db.StudyParticipants.AddAsync(createdStudyParticipant);
                await _db.SaveChangesAsync();

                await AddNewRoleAssignmentToSandboxes(createdStudyParticipant);

                return _mapper.Map<StudyParticipantDto>(createdStudyParticipant);
            }
            catch (Exception)
            {
                await RemoveIfExist(createdStudyParticipant);
                throw;
            }
        }

        async Task<StudyParticipantDto> AddAzureUserAsync(Study studyFromDb, ParticipantLookupDto user, string role)
        {
            var userFromAzure = await _azureADUsersService.GetUserAsync(user.ObjectId);

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
                return await AddDbUserAsync(studyFromDb, userDb.Id, role);
            }

            if (RoleAllreadyExistsForUser(studyFromDb, userDb.Id, role))
            {
                throw new ArgumentException($"Role {role} allready granted for user {user.DatabaseId.Value} on study {studyFromDb.Id}");
            }

            StudyParticipant createdStudyParticipant = null;

            try
            {
                createdStudyParticipant = new StudyParticipant { StudyId = studyFromDb.Id, RoleName = role };
                userDb.StudyParticipants = new List<StudyParticipant> { createdStudyParticipant };

                await _db.SaveChangesAsync();

                await AddNewRoleAssignmentToSandboxes(createdStudyParticipant);

                return _mapper.Map<StudyParticipantDto>(createdStudyParticipant);

            }
            catch (Exception)
            {
                await RemoveIfExist(createdStudyParticipant);

                throw;
            }
        }

        public async Task<StudyParticipantDto> RemoveAsync(int studyId, int userId, string roleName)
        {
            var studyFromDb = await StudySingularQueries.GetStudyByIdCheckAccessOrThrow(_db, _userService, studyId, UserOperation.Study_AddRemove_Participant, true, roleName);

            if (roleName == StudyRoles.StudyOwner)
            {
                throw new ArgumentException($"The Study Owner role cannot be deleted");
            }

            var studyParticipantFromDb = studyFromDb.StudyParticipants.FirstOrDefault(p => p.UserId == userId && p.RoleName == roleName);

            if (studyParticipantFromDb == null)
            {
                throw NotFoundException.CreateForEntityCustomDescr("StudyParticipant", $"studyId: {studyId}, userId: {userId}, roleName: {roleName}");
            }

            studyFromDb.StudyParticipants.Remove(studyParticipantFromDb);
            await _db.SaveChangesAsync();
                       

            await ReviseRoleAssignmentsForUser(studyParticipantFromDb);

            return _mapper.Map<StudyParticipantDto>(studyParticipantFromDb);
        }

        async Task RemoveIfExist(StudyParticipant participant)
        {
            if (participant != null)
            {
                var sameParticipantFromDb = await _db.StudyParticipants
                    .Where(sp => sp.StudyId == participant.StudyId
                    && sp.UserId == participant.UserId
                    && sp.RoleName == participant.RoleName)
                    .FirstOrDefaultAsync();

                if (sameParticipantFromDb != null)
                {
                    _db.StudyParticipants.Remove(participant);
                    await _db.SaveChangesAsync();
                }
            }
        }

        async Task AddNewRoleAssignmentToSandboxes(StudyParticipant studyParticipant)
        {
            if (ParticipantRoleToAzureRoleTranslator.Translate(studyParticipant.RoleName, out string translatedRole)){
                var sandboxes = await _db.Sandboxes.Include(s => s.Resources).ThenInclude(r => r.RoleAssignments).Where(s => s.StudyId == studyParticipant.StudyId).ToListAsync();

                foreach (var curSb in sandboxes)
                {
                    if (curSb.Deleted.HasValue && curSb.Deleted.Value)
                    {
                        continue;
                    }

                    var resourceGroup = CloudResourceUtil.GetSandboxResourceGroupEntry(curSb.Resources);
                    await _cloudResourceRoleAssignmentCreateService.AddAsync(resourceGroup.Id, studyParticipant.User.ObjectId, translatedRole);
                    var updateOp = await _cloudResourceOperationCreateService.CreateUpdateOperationAsync(resourceGroup.Id, CloudResourceOperationType.ENSURE_ROLES);
                    await ProvisioningQueueUtil.CreateQueueItem(updateOp, _workQueue);
                }
            }
        }

        async Task ReviseRoleAssignmentsForUser(StudyParticipant studyParticipant)
        {
            var roleAssignmentUserShouldHave = new HashSet<string>();

            foreach (var curParticipant in await _db.StudyParticipants.Where(sp => sp.StudyId == studyParticipant.StudyId && sp.UserId == studyParticipant.UserId).ToListAsync())
            {
                if(ParticipantRoleToAzureRoleTranslator.Translate(studyParticipant.RoleName, out string translatedRole))
                {
                    if (roleAssignmentUserShouldHave.Contains(translatedRole))
                    {
                        continue;
                    }
                    else
                    {
                        roleAssignmentUserShouldHave.Add(translatedRole);
                    }
                }               
            }

            var sandboxes = await _db.Sandboxes.Include(s => s.Resources).ThenInclude(r => r.RoleAssignments).Where(s => s.StudyId == studyParticipant.StudyId).ToListAsync();

            foreach (var curSb in sandboxes)
            {
                if (curSb.Deleted.HasValue && curSb.Deleted.Value)
                {
                    continue;
                }

                var resourceGroup = CloudResourceUtil.GetSandboxResourceGroupEntry(curSb.Resources);
                await _cloudResourceRoleAssignmentUpdateService.ReviseRoleAssignments(resourceGroup.Id, studyParticipant.User.ObjectId, roleAssignmentUserShouldHave);
                var updateOp = await _cloudResourceOperationCreateService.CreateUpdateOperationAsync(resourceGroup.Id, CloudResourceOperationType.ENSURE_ROLES);
                await ProvisioningQueueUtil.CreateQueueItem(updateOp, _workQueue);
            }
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
                role.Equals(StudyRoles.VendorContributor)) == false)
            {
                throw new ArgumentException($"Invalid Role supplied: {role}");
            }
        }
    }
}
