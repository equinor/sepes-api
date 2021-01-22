using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Study;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Service.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class StudyParticipantCreateService : StudyParticipantBaseService, IStudyParticipantCreateService
    {
        readonly IAzureUserService _azureADUsersService;

        public StudyParticipantCreateService(SepesDbContext db,
            IMapper mapper,
            IUserService userService,
            IAzureUserService azureADUsersService,
            IProvisioningQueueService provisioningQueueService,
            ICloudResourceOperationCreateService cloudResourceOperationCreateService,
            ICloudResourceOperationUpdateService cloudResourceOperationUpdateService)

            : base(db, mapper, userService, provisioningQueueService, cloudResourceOperationCreateService,cloudResourceOperationUpdateService)
        {
            _azureADUsersService = azureADUsersService;
        }

        public async Task<StudyParticipantDto> AddAsync(int studyId, ParticipantLookupDto user, string role)
        {
            List<int> updateOperationIds = null;

            try
            {
                ValidateRoleNameThrowIfInvalid(role);

                var studyFromDb = await StudySingularQueries.GetStudyByIdCheckAccessOrThrow(_db, _userService, studyId, UserOperation.Study_AddRemove_Participant, true, false, role);

                updateOperationIds = await CreateDraftRoleUpdateOperationsAsync(studyFromDb);

                StudyParticipantDto participantDto = null;

                if (user.Source == ParticipantSource.Db)
                {
                    participantDto = await AddDbUserAsync(studyFromDb, user.DatabaseId.Value, role);
                }
                else if (user.Source == ParticipantSource.Azure)
                {
                    participantDto = await AddAzureUserAsync(studyFromDb, user, role);
                }
                else
                {
                    throw new ArgumentException($"Unknown source for user {user.UserName}");
                }

                await FinalizeAndQueueRoleAssignmentUpdateAsync(studyId, updateOperationIds);

                return participantDto;
            }
            catch (Exception ex)
            {
                if(updateOperationIds != null)
                {
                    foreach(var curOperationId in updateOperationIds)
                    {
                        await _cloudResourceOperationUpdateService.AbortAndAllowDependentOperationsToRun(curOperationId, ex.Message);
                    }
                }

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

                return _mapper.Map<StudyParticipantDto>(createdStudyParticipant);

            }
            catch (Exception)
            {
                await RemoveIfExist(createdStudyParticipant);

                throw;
            }
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

        //async Task AddNewRoleAssignmentToSandboxes(StudyParticipant studyParticipant)
        //{
        //    if (ParticipantRoleToAzureRoleTranslator.Translate(studyParticipant.RoleName, out string translatedRole))
        //    {
        //        var sandboxes = await _db.Sandboxes.Include(s => s.Resources).ThenInclude(r => r.RoleAssignments).Where(s => s.StudyId == studyParticipant.StudyId).ToListAsync();

        //        foreach (var curSb in sandboxes)
        //        {
        //            if (curSb.Deleted.HasValue && curSb.Deleted.Value)
        //            {
        //                continue;
        //            }

        //            var resourceGroup = CloudResourceUtil.GetSandboxResourceGroupEntry(curSb.Resources);

        //            //Create list of desired roles

        //            await _cloudResourceRoleAssignmentCreateService.AddAsync(resourceGroup.Id, studyParticipant.User.ObjectId, translatedRole);
        //        }
        //    }
        //}
    }
}
