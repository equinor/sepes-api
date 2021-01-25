using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using Sepes.Infrastructure.Util.Auth;
using Sepes.Infrastructure.Util.Provisioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class StudyParticipantBaseService
    {
        protected readonly SepesDbContext _db;
        protected readonly IMapper _mapper;
        protected readonly IUserService _userService;
        protected readonly IProvisioningQueueService _provisioningQueueService;
        protected readonly ICloudResourceOperationCreateService _cloudResourceOperationCreateService;
        protected readonly ICloudResourceOperationUpdateService _cloudResourceOperationUpdateService;

        public StudyParticipantBaseService(SepesDbContext db,
            IMapper mapper,
            IUserService userService,
            IProvisioningQueueService provisioningQueueService,
            ICloudResourceOperationCreateService cloudResourceOperationCreateService,
            ICloudResourceOperationUpdateService cloudResourceOperationUpdateService)
        {
            _db = db;
            _mapper = mapper;
            _userService = userService;
            _provisioningQueueService = provisioningQueueService;
            _cloudResourceOperationCreateService = cloudResourceOperationCreateService;
            _cloudResourceOperationUpdateService = cloudResourceOperationUpdateService;
        }
   
        protected bool RoleAllreadyExistsForUser(Study study, int userId, string roleName)
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

        protected void ValidateRoleNameThrowIfInvalid(string role)
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

        protected async Task<List<int>> CreateDraftRoleUpdateOperationsAsync(Study study)
        {
            return await ThreadSafeUpdateOperationUtil.CreateDraftRoleUpdateOperationAsync(study, _cloudResourceOperationCreateService);
        }

        protected async Task FinalizeAndQueueRoleAssignmentUpdateAsync(int studyId, List<int> existingUpdateOperationIds)
        {
            var study = await GetStudyAsync(studyId, true);

            var desiredRoles = ParticipantRoleToAzureRoleTranslator.CreateDesiredRolesForSandboxResourceGroup(study.StudyParticipants.ToList());
            var desiredRolesSerialized = CloudResourceConfigStringSerializer.Serialize(desiredRoles);

            foreach (var currentOperationId in existingUpdateOperationIds)
            {
                var updateOp = await _cloudResourceOperationUpdateService.SetDesiredStateAsync(currentOperationId, desiredRolesSerialized);
                await ProvisioningQueueUtil.CreateItemAndEnqueue(updateOp, _provisioningQueueService);
            }
        }      

        async Task<Study> GetStudyAsync(int studyId, bool allIncludes)
        {
            var studyQueryable = _db.Studies.AsNoTracking()
              .Include(s => s.Sandboxes)
                  .ThenInclude(sb => sb.Resources)
              .If(allIncludes, x => x.Include(s => s.StudyParticipants)
                  .ThenInclude(sp => sp.User));

            return await studyQueryable.FirstOrDefaultAsync(s => s.Id == studyId);
        }      
    }
}
