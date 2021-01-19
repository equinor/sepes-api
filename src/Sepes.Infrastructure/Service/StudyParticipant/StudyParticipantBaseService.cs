using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Constants.CloudResource;
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

        public StudyParticipantBaseService(SepesDbContext db,
            IMapper mapper,
            IUserService userService,
            IProvisioningQueueService provisioningQueueService,
            ICloudResourceOperationCreateService cloudResourceOperationCreateService)
        {
            _db = db;
            _mapper = mapper;
            _userService = userService;
            _provisioningQueueService = provisioningQueueService;
            _cloudResourceOperationCreateService = cloudResourceOperationCreateService;
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

        //protected async Task<List<int>> CreateDraftRoleUpdateOperation(int studyId)
        //{
        //    var study = await GetStudyAsync(studyId, true);

        //    var result = new List<int>();

        //    foreach (var resourceGroup in GetResourceGroups(study))
        //    {
        //        var updateOp = await _cloudResourceOperationCreateService.CreateUpdateOperationAsync(resourceGroup.Id, CloudResourceOperationType.ENSURE_ROLES);
        //        result.Add(updateOp.Id);
        //    }
        //}     

        protected async Task ScheduleRoleAssignmentUpdateAsync(int studyId)
        {
            var study = await GetStudyAsync(studyId, true);
          
            var desiredRoles = ParticipantRoleToAzureRoleTranslator.CreateListOfDesiredRoles(study.StudyParticipants.ToList());
            var desiredRolesSerialized = CloudResourceConfigStringSerializer.Serialize(desiredRoles);  
            
            foreach(var resourceGroup in GetResourceGroups(study))
            {                
                var updateOp = await _cloudResourceOperationCreateService.CreateUpdateOperationAsync(resourceGroup.Id, CloudResourceOperationType.ENSURE_ROLES, desiredState: desiredRolesSerialized);
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

        List<CloudResource> GetResourceGroups(Study study)
        {
            return study.Sandboxes
                .Where(sb => !SoftDeleteUtil.IsMarkedAsDeleted(sb))
                .Select(sb => CloudResourceUtil.GetSandboxResourceGroupEntry(sb.Resources))
                .Where(r => !r.Deleted.HasValue)
                .ToList();
        }
    }
}
