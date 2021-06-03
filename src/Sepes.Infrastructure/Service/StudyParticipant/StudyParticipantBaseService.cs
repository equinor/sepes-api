using AutoMapper;
using Microsoft.Extensions.Logging;
using Sepes.Common.Constants;
using Sepes.Common.Constants.CloudResource;
using Sepes.Common.Dto;
using Sepes.Common.Util;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class StudyParticipantBaseService
    { 
        protected readonly SepesDbContext _db;
        protected readonly IMapper _mapper;
        protected readonly ILogger _logger;
        protected readonly IUserService _userService;
        protected readonly IStudyEfModelService _studyModelService;
        protected readonly IProvisioningQueueService _provisioningQueueService;
        protected readonly ICloudResourceReadService _cloudResourceReadService;
        protected readonly ICloudResourceOperationCreateService _cloudResourceOperationCreateService;
        protected readonly ICloudResourceOperationUpdateService _cloudResourceOperationUpdateService;

        public StudyParticipantBaseService(SepesDbContext db,
            IMapper mapper,
            ILogger logger,
            IUserService userService,
            IStudyEfModelService studyModelService,
            IProvisioningQueueService provisioningQueueService,
            ICloudResourceReadService cloudResourceReadService,
            ICloudResourceOperationCreateService cloudResourceOperationCreateService,
            ICloudResourceOperationUpdateService cloudResourceOperationUpdateService)
        {
            _db = db;
            _mapper = mapper;
            _logger = logger;
            _userService = userService;
            _studyModelService = studyModelService;
            _provisioningQueueService = provisioningQueueService;
            _cloudResourceReadService = cloudResourceReadService;
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
            if (!(role.Equals(StudyRoles.SponsorRep) ||
                role.Equals(StudyRoles.StudyOwner) ||
                role.Equals(StudyRoles.StudyViewer) ||
                role.Equals(StudyRoles.VendorAdmin) ||
                role.Equals(StudyRoles.VendorContributor)))
            {
                throw new ArgumentException($"Invalid Role supplied: {role}");
            }
        }

        protected async Task<Study> GetStudyForParticipantOperation(int studyId, string roleBeingAddedOrRemoved = null)
        {         
            var studyFromDb = await _studyModelService.GetForParticpantOperationsAsync(studyId, UserOperation.Study_AddRemove_Participant, roleBeingAddedOrRemoved);
            return studyFromDb;
        }

        protected async Task CreateRoleUpdateOperationsAsync(int studyId)
        {
            var resourcesToUpdate = await _cloudResourceReadService.GetDatasetResourceGroupIdsForStudy(studyId);
            resourcesToUpdate.AddRange(await _cloudResourceReadService.GetSandboxResourceGroupIdsForStudy(studyId));

            foreach (var currentResourceId in resourcesToUpdate)
            {
                var desiredState = CloudResourceConfigStringSerializer.Serialize(new CloudResourceOperationStateForRoleUpdate(studyId));
                var updateOperation = await _cloudResourceOperationCreateService.CreateUpdateOperationAsync(currentResourceId, CloudResourceOperationType.ENSURE_ROLES, desiredState: desiredState);
                await _provisioningQueueService.CreateItemAndEnqueue(updateOperation);
            }
        }             
    }
}
