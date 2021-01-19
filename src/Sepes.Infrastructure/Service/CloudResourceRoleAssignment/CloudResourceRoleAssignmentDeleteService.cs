using AutoMapper;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class CloudResourceRoleAssignmentDeleteService : CloudResourceRoleAssignmentServiceBase, ICloudResourceRoleAssignmentDeleteService
    {
        public CloudResourceRoleAssignmentDeleteService(
            SepesDbContext db,
            IMapper mapper,
            IUserService userService,
            ILogger<CloudResourceRoleAssignmentDeleteService> logger)
              : base(db, mapper, userService, logger)
        {
        }

        public async Task RemoveBySignatureAsync(int resourceId, string principalId, string roleId, bool failOnNotExist = false)
        {           
            var existing = await GetBySignature(resourceId, principalId, roleId);

            if (existing == null)
            {
                if (failOnNotExist)
                {
                    throw new Exception($"Role assignment does not exists for resource {resourceId}, principal: {principalId}, roleDefinitionId: {roleId}");
                }
            }
            else
            {               
                await SoftDeleteUtil.MarkAsDeleted(existing, _userService);
                await _db.SaveChangesAsync();
            } 
        }     
    }
}
