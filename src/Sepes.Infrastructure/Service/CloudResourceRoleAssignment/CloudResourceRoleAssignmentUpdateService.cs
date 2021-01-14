using AutoMapper;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Sepes.Infrastructure.Service
{
    public class CloudResourceRoleAssignmentUpdateService : CloudResourceRoleAssignmentServiceBase, ICloudResourceRoleAssignmentUpdateService
    {
        public CloudResourceRoleAssignmentUpdateService(
            SepesDbContext db,
            IMapper mapper,
            IUserService userService,
            ILogger<CloudResourceRoleAssignmentUpdateService> logger)
              : base(db, mapper, userService, logger)
        {
        }        

        public async Task<CloudResourceRoleAssignmentDto> SetForeignIdAsync(int roleAssignmentId, string foreignSystemId)
        {
            var roleAssignment = await GetOrThrowInternalAsync(roleAssignmentId);
            roleAssignment.ForeignSystemId = foreignSystemId;
            await _db.SaveChangesAsync();
            return MapEntityToDto(roleAssignment);
        }

        public async Task ReviseRoleAssignments(int resourceId, string principalId, HashSet<string> rolesUserShouldHave)
        {
            var currentUser = await _userService.GetCurrentUserAsync();
            //Delete those not relevant
            var existing = await GetByResourceAndPrincipalAsync(resourceId, principalId);

            foreach(var curExisting in existing)
            {
                if(rolesUserShouldHave.Contains(curExisting.RoleId) == false)
                {
                    SoftDeleteUtil.MarkAsDeleted(curExisting, currentUser);
                    await _db.SaveChangesAsync();
                }
            }

            foreach(var curShouldExist in rolesUserShouldHave)
            {
                await AddInternalAsync(resourceId, principalId, curShouldExist);
            }           
        }
    }
}
