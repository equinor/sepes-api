using AutoMapper;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Threading.Tasks;


namespace Sepes.Infrastructure.Service
{
    public class CloudResourceResourceRoleAssignmentCreateService : CloudResourceRoleAssignmentServiceBase, ICloudResourceRoleAssignmentCreateService
    {
        public CloudResourceResourceRoleAssignmentCreateService(
            SepesDbContext db,
            IMapper mapper,
            IUserService userService,
            ILogger<SandboxResourceDeleteService> logger)
              : base(db, mapper, userService, logger)
        {



        }

        public async Task<CloudResourceRoleAssignmentDto> AddAsync(int resourceDbId, string principalId, string roleId)
        {           
            var existing = await GetBySignature(resourceDbId, principalId, roleId);

            if (existing != null)
            {
                throw new Exception($"Role assignment allready exists for resource {resourceDbId}, principal: {principalId}, roleDefinitionId: {roleId}");
            }

            var currentUser = await _userService.GetCurrentUserAsync();

            var newAssignment = new Model.CloudResourceRoleAssignment()
            {
                CloudResourceId = resourceDbId,
                UserOjectId = principalId,
                RoleId = roleId,
                CreatedBy = currentUser.UserName,
                UpdatedBy = currentUser.UserName,
            };

            _db.CloudResourceRoleAssignments.Add(newAssignment);

            await _db.SaveChangesAsync();

            return MapEntityToDto(newAssignment);
        }
    }
}
