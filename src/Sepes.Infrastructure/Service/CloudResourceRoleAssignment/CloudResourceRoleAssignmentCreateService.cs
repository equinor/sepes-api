using AutoMapper;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util.Provisioning;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class CloudResourceRoleAssignmentCreateService : CloudResourceRoleAssignmentServiceBase, ICloudResourceRoleAssignmentCreateService
    {
        readonly ICloudResourceOperationCreateService _cloudResourceOperationCreateService;
        readonly IProvisioningQueueService _workQueue;

        public CloudResourceRoleAssignmentCreateService(
            SepesDbContext db,
            IMapper mapper,
            IUserService userService,
            ILogger<CloudResourceRoleAssignmentCreateService> logger,
            ICloudResourceOperationCreateService cloudResourceOperationCreateService,
            IProvisioningQueueService workQueue
              )
              : base(db, mapper, userService, logger)
        {
            _cloudResourceOperationCreateService = cloudResourceOperationCreateService;
            _workQueue = workQueue;
        }

        public async Task<CloudResourceRoleAssignmentDto> AddAsync(int resourceDbId, string principalId, string roleId, bool failOnDuplicate = false)
        {
            var roleassignmentDto = await AddInternalAsync(resourceDbId, principalId, roleId, failOnDuplicate);
            var updateOp = await _cloudResourceOperationCreateService.CreateUpdateOperationAsync(resourceDbId, CloudResourceOperationType.ENSURE_ROLES);
            await ProvisioningQueueUtil.CreateItemAndEnqueue(updateOp, _workQueue);

            return roleassignmentDto;
        }
    }
}
