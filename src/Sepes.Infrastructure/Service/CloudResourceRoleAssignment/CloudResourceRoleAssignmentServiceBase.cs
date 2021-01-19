using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using System.Linq;
using System.Threading.Tasks;
using Sepes.Infrastructure;
using System.Collections.Generic;
using System;

namespace Sepes.Infrastructure.Service
{
    public class CloudResourceRoleAssignmentServiceBase : ServiceBase<CloudResourceRoleAssignment>
    {       

        public CloudResourceRoleAssignmentServiceBase(SepesDbContext db, IMapper mapper, IUserService userService, ILogger logger)
         : base(db, mapper, userService)
        {           
          
        }

        IQueryable<CloudResourceRoleAssignment> GetBasicQueryable(int id = default, bool onlyActive = true)
        {
            return _db.CloudResourceRoleAssignments
                                .Include(r => r.Resource)
                                .ThenInclude(s => s.Sandbox)
                                .Include(r => r.Resource)
                                .ThenInclude(s => s.Operations)
                                .If(id > 0, x => x.Where(ra => ra.Id == id))
                                .If(onlyActive, x => x.Where(ra => ra.Deleted.HasValue == false || ra.Deleted.Value == false));
        }      

        protected async Task<CloudResourceRoleAssignment> GetInternalAsync(int id)
        {
           return await GetBasicQueryable(id)
                    .FirstOrDefaultAsync();         

        }

        protected async Task<CloudResourceRoleAssignment> GetBySignature(int resourceId, string principalId, string roleDefinitionId)
        {
            return await GetBasicQueryable()
                     .FirstOrDefaultAsync(ra => ra.CloudResourceId == resourceId && ra.UserOjectId == principalId && ra.RoleId == roleDefinitionId);

        }

        protected async Task<List<CloudResourceRoleAssignment>> GetByResourceAndPrincipalAsync(int resourceId, string principalId)
        {
            return await GetBasicQueryable()
                     .Where(ra => ra.CloudResourceId == resourceId && ra.UserOjectId == principalId).ToListAsync();

        }     

        protected async Task<CloudResourceRoleAssignment> GetOrThrowInternalAsync(int id)
        {
            var entityFromDb = await GetInternalAsync(id);

            if (entityFromDb == null)
            {
                throw NotFoundException.CreateForEntity("CloudResourceRoleAssignment", id);
            }

            return entityFromDb;
        }

        public async Task<CloudResourceRoleAssignmentDto> GetDtoByIdAsync(int id)
        {
            var entityFromDb = await GetOrThrowInternalAsync(id);
            var dto = MapEntityToDto(entityFromDb);

            return dto;
        }

        public async Task<CloudResourceRoleAssignmentDto> AddInternalAsync(int resourceDbId, string principalId, string roleId, bool failOnDuplicate = false)
        {
            var existing = await GetBySignature(resourceDbId, principalId, roleId);

            if (existing != null)
            {
                if (failOnDuplicate)
                {
                    throw new Exception($"Role assignment allready exists for resource {resourceDbId}, principal: {principalId}, roleDefinitionId: {roleId}");
                }
                else
                {
                    return MapEntityToDto(existing);
                }
            }

            var currentUser = await _userService.GetCurrentUserAsync();

            var newAssignment = new CloudResourceRoleAssignment()
            {
                CloudResourceId = resourceDbId,
                UserOjectId = principalId,
                RoleId = roleId,
                CreatedBy = currentUser.UserName,
                UpdatedBy = currentUser.UserName,
            };
            var resourceFromDb = await _db.CloudResources.Include(r => r.RoleAssignments).Where(r => r.Id == resourceDbId).FirstOrDefaultAsync();
            resourceFromDb.RoleAssignments.Add(newAssignment);

            await _db.SaveChangesAsync();

            return MapEntityToDto(newAssignment);
        }

        protected CloudResourceRoleAssignmentDto MapEntityToDto(CloudResourceRoleAssignment entity) => _mapper.Map<CloudResourceRoleAssignmentDto>(entity);
                    
    }
}
