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
        readonly ICloudResourceRoleAssignmentUpdateService _cloudResourceRoleAssignmentUpdateService;
        readonly ICloudResourceOperationCreateService _cloudResourceOperationCreateService;
        readonly IProvisioningQueueService _workQueue;

        public StudyParticipantRemoveService(SepesDbContext db,
            IMapper mapper,
            IUserService userService,
            ICloudResourceOperationCreateService cloudResourceOperationCreateService,
            ICloudResourceRoleAssignmentUpdateService cloudResourceRoleAssignmentUpdateService,
            IProvisioningQueueService workQueue
            )
            : base (db, mapper, userService)
        {
            _cloudResourceOperationCreateService = cloudResourceOperationCreateService;
            _cloudResourceRoleAssignmentUpdateService = cloudResourceRoleAssignmentUpdateService;
            _workQueue = workQueue;
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
    }
}
