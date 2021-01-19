using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Constants.CloudResource;
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
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class StudyParticipantRemoveService : StudyParticipantBaseService, IStudyParticipantRemoveService
    {
        public StudyParticipantRemoveService(SepesDbContext db,
            IMapper mapper,
            IUserService userService,
            IProvisioningQueueService provisioningQueueService,
            ICloudResourceOperationCreateService cloudResourceOperationCreateService
            )
            : base(db, mapper, userService, provisioningQueueService, cloudResourceOperationCreateService)
        {

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

            await ScheduleRoleAssignmentUpdateAsync(studyFromDb.Id);

            return _mapper.Map<StudyParticipantDto>(studyParticipantFromDb);
        }      
    }
}
