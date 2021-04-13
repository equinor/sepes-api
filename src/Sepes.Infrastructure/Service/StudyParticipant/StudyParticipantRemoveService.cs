using AutoMapper;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Study;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class StudyParticipantRemoveService : StudyParticipantBaseService, IStudyParticipantRemoveService
    {
        public StudyParticipantRemoveService(SepesDbContext db,
            IMapper mapper,
            ILogger<StudyParticipantRemoveService> logger,
            IUserService userService,
            IStudyModelService studyModelService,
            IProvisioningQueueService provisioningQueueService,
            ICloudResourceOperationCreateService cloudResourceOperationCreateService,
            ICloudResourceOperationUpdateService cloudResourceOperationUpdateService
            )
            : base(db, mapper, logger, userService, studyModelService, provisioningQueueService, cloudResourceOperationCreateService, cloudResourceOperationUpdateService)
        {

        }

        public async Task<StudyParticipantDto> RemoveAsync(int studyId, int userId, string roleName)
        {
            List<CloudResourceOperationDto> updateOperations = null;

            try
            {
                var studyFromDb = await GetStudyForParticipantOperation(studyId, roleName);            

                if (roleName == StudyRoles.StudyOwner)
                {
                    throw new ArgumentException($"The Study Owner role cannot be deleted");
                }

                updateOperations = await CreateDraftRoleUpdateOperationsAsync(studyFromDb);

                var studyParticipantFromDb = studyFromDb.StudyParticipants.FirstOrDefault(p => p.UserId == userId && p.RoleName == roleName);

                if (studyParticipantFromDb == null)
                {
                    throw NotFoundException.CreateForEntityCustomDescr("StudyParticipant", $"studyId: {studyId}, userId: {userId}, roleName: {roleName}");
                }

                studyFromDb.StudyParticipants.Remove(studyParticipantFromDb);

                await _db.SaveChangesAsync();

                await FinalizeAndQueueRoleAssignmentUpdateAsync(studyId, updateOperations);

                return _mapper.Map<StudyParticipantDto>(studyParticipantFromDb);
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

                throw new Exception($"Remove participant failed: {ex.Message}", ex);
            }
        }
    }
}
