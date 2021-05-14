using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sepes.Azure.Service.Interface;
using Sepes.Common.Constants;
using Sepes.Common.Dto;
using Sepes.Common.Dto.Study;
using Sepes.Common.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
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
            ILogger<StudyParticipantCreateService> logger,    
            IUserService userService,
            IStudyModelService studyModelService,
            IAzureUserService azureADUsersService,
            IProvisioningQueueService provisioningQueueService,
            ICloudResourceOperationCreateService cloudResourceOperationCreateService,
            ICloudResourceOperationUpdateService cloudResourceOperationUpdateService)

            : base(db, mapper, logger, userService, studyModelService, provisioningQueueService, cloudResourceOperationCreateService,cloudResourceOperationUpdateService)
        {
            _azureADUsersService = azureADUsersService;            
        }

        public async Task<StudyParticipantDto> AddAsync(int studyId, ParticipantLookupDto user, string role)
        {
            List<CloudResourceOperationDto> updateOperations = null;

            try
            {
                ValidateRoleNameThrowIfInvalid(role);              

                var studyFromDb = await GetStudyForParticipantOperation(studyId, role);
                
                updateOperations = await CreateDraftRoleUpdateOperationsAsync(studyFromDb);           

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
                
                await FinalizeAndQueueRoleAssignmentUpdateAsync(studyId, updateOperations);                   

                return participantDto;
            }
            catch (Exception ex)
            {
                if(updateOperations != null)
                {
                    foreach(var curOperation in updateOperations)
                    {
                        await _cloudResourceOperationUpdateService.AbortAndAllowDependentOperationsToRun(curOperation.Id, ex.Message);
                    }
                }

                if(ex is ForbiddenException)
                {
                    throw;
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
    }
}
