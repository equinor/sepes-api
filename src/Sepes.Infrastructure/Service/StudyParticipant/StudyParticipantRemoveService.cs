using AutoMapper;
using Microsoft.Extensions.Logging;
using Sepes.Common.Constants;
using Sepes.Common.Dto;
using Sepes.Common.Dto.Study;
using Sepes.Common.Exceptions;
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
            ICloudResourceReadService cloudResourceReadService,
            ICloudResourceOperationCreateService cloudResourceOperationCreateService,
            ICloudResourceOperationUpdateService cloudResourceOperationUpdateService
            )
            : base(db, mapper, logger, userService, studyModelService, provisioningQueueService, cloudResourceReadService, cloudResourceOperationCreateService, cloudResourceOperationUpdateService)
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

                var studyParticipantFromDb = studyFromDb.StudyParticipants.FirstOrDefault(p => p.UserId == userId && p.RoleName == roleName);

                if (studyParticipantFromDb == null)
                {
                    throw NotFoundException.CreateForEntityCustomDescr("StudyParticipant", $"studyId: {studyId}, userId: {userId}, roleName: {roleName}");
                }

                studyFromDb.StudyParticipants.Remove(studyParticipantFromDb);

                await CreateRoleUpdateOperationsAsync(studyParticipantFromDb);            

                await _db.SaveChangesAsync();

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
