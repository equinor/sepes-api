﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sepes.Azure.Dto;
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
            IStudyEfModelService studyModelService,
            IAzureUserService azureADUsersService,
            IProvisioningQueueService provisioningQueueService,
            ICloudResourceReadService cloudResourceReadService,
            ICloudResourceOperationCreateService cloudResourceOperationCreateService,
            ICloudResourceOperationUpdateService cloudResourceOperationUpdateService)

            : base(db, mapper, logger, userService, studyModelService, provisioningQueueService, cloudResourceReadService, cloudResourceOperationCreateService, cloudResourceOperationUpdateService)
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

                StudyParticipantDto newlyAddedParticipant = null;

                if (user.Source == ParticipantSource.Db)
                {
                    newlyAddedParticipant = await AddDbUserAsync(studyFromDb, user.DatabaseId.Value, role);
                }
                else if (user.Source == ParticipantSource.Azure)
                {
                    newlyAddedParticipant = await AddAzureUserAsync(studyFromDb, user, role);
                }
                else
                {
                    throw new ArgumentException($"Unknown source for user {user.UserName}");
                }

                await CreateRoleUpdateOperationsAsync(newlyAddedParticipant.StudyId);

                return newlyAddedParticipant;
            }
            catch (Exception ex)
            {
                if (updateOperations != null)
                {
                    foreach (var curOperation in updateOperations)
                    {
                        await _cloudResourceOperationUpdateService.AbortAndAllowDependentOperationsToRun(curOperation.Id, ex.Message);
                    }
                }

                if (ex is ForbiddenException)
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

            var userFromDb = await _userService.GetByDbIdAsync(userId);

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
                    
                return ConvertToDto(createdStudyParticipant, userFromDb);
            }
            catch (Exception)
            {
                await RemoveIfExist(createdStudyParticipant);
                throw;
            }
        }      

        async Task<StudyParticipantDto> AddAzureUserAsync(Study studyFromDb, ParticipantLookupDto user, string role)
        {
            AzureUserDto userFromAzure;

            if (await _userService.IsMockUser())
            {
                userFromAzure = new AzureUserDto { DisplayName = "Mock user", Mail = "Mock@User.com", UserPrincipalName = "Mock user" };
            }
            else
            {
                userFromAzure = await _azureADUsersService.GetUserAsync(user.ObjectId);
            }

            if (userFromAzure == null)
            {
                throw new NotFoundException($"AD User with id {user.ObjectId} not found!");
            }

            var userDb = await _userService.EnsureExists(new UserDto(user.ObjectId, userFromAzure.UserPrincipalName, userFromAzure.DisplayName, userFromAzure.Mail));

            if (RoleAllreadyExistsForUser(studyFromDb, userDb.Id, role))
            {
                throw new ArgumentException($"Role {role} allready granted for user {user.DatabaseId.Value} on study {studyFromDb.Id}");
            }

            StudyParticipant createdStudyParticipant = null;

            try
            {
                createdStudyParticipant = new StudyParticipant { StudyId = studyFromDb.Id, RoleName = role };
                studyFromDb.StudyParticipants = new List<StudyParticipant> { createdStudyParticipant };

                await _db.SaveChangesAsync();

                return _mapper.Map<StudyParticipantDto>(createdStudyParticipant);
            }
            catch (Exception)
            {
                await RemoveIfExist(createdStudyParticipant);

                throw;
            }
        }

        async Task RemoveIfExist(StudyParticipant studyParticipant)
        {
            if (studyParticipant != null)
            {
                await RemoveIfExist(studyParticipant.StudyId, studyParticipant.UserId, studyParticipant.RoleName);
            }
        }


        async Task RemoveIfExist(int studyId, int userId, string role)
        {
            var sameParticipantFromDb = await _db.StudyParticipants
                .Where(sp => sp.StudyId == studyId
                && sp.UserId == userId
                && sp.RoleName == role)
                .FirstOrDefaultAsync();

            if (sameParticipantFromDb != null)
            {
                _db.StudyParticipants.Remove(sameParticipantFromDb);
                await _db.SaveChangesAsync();
            }
        }
    }
}
