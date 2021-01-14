using AutoMapper;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class CloudResourceRoleAssignmentCreateService : CloudResourceRoleAssignmentServiceBase, ICloudResourceRoleAssignmentCreateService
    {
        public CloudResourceRoleAssignmentCreateService(
            SepesDbContext db,
            IMapper mapper,
            IUserService userService,
            ILogger<CloudResourceRoleAssignmentCreateService> logger)
              : base(db, mapper, userService, logger)
        {

        }

        public async Task<CloudResourceRoleAssignmentDto> AddAsync(int resourceDbId, string principalId, string roleId, bool failOnDuplicate = false)
        {
            return await AddInternalAsync(resourceDbId, principalId, roleId, failOnDuplicate);
        }
    }
}
