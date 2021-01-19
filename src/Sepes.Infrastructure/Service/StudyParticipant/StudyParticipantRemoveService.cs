using AutoMapper;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Study;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Service.Queries;
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
            IUserService userService,
            IProvisioningQueueService provisioningQueueService,
            ICloudResourceOperationCreateService cloudResourceOperationCreateService,
            ICloudResourceOperationUpdateService cloudResourceOperationUpdateService
            )
            : base(db, mapper, userService, provisioningQueueService, cloudResourceOperationCreateService, cloudResourceOperationUpdateService)
        {

        }

        public async Task<StudyParticipantDto> RemoveAsync(int studyId, int userId, string roleName)
        {
            List<int> updateOperationIds = null;

            try
            {
                var studyFromDb = await StudySingularQueries.GetStudyByIdCheckAccessOrThrow(_db, _userService, studyId, UserOperation.Study_AddRemove_Participant, true, roleName);

                if (roleName == StudyRoles.StudyOwner)
                {
                    throw new ArgumentException($"The Study Owner role cannot be deleted");
                }

                updateOperationIds = await CreateDraftRoleUpdateOperationsAsync(studyFromDb);

                var studyParticipantFromDb = studyFromDb.StudyParticipants.FirstOrDefault(p => p.UserId == userId && p.RoleName == roleName);

                if (studyParticipantFromDb == null)
                {
                    throw NotFoundException.CreateForEntityCustomDescr("StudyParticipant", $"studyId: {studyId}, userId: {userId}, roleName: {roleName}");
                }

                studyFromDb.StudyParticipants.Remove(studyParticipantFromDb);

                await _db.SaveChangesAsync();

                await FinalizeAndQueueRoleAssignmentUpdateAsync(studyId, updateOperationIds);

                return _mapper.Map<StudyParticipantDto>(studyParticipantFromDb);
            }
            catch (Exception ex)
            {
                if (updateOperationIds != null)
                {
                    foreach (var curOperationId in updateOperationIds)
                    {
                        await _cloudResourceOperationUpdateService.AbortAndAllowDependentOperationsToRun(curOperationId, ex.Message);
                    }
                }

                throw new Exception($"Remove participant failed: {ex.Message}", ex);
            }
        }
    }
}
