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

        protected CloudResourceRoleAssignmentDto MapEntityToDto(CloudResourceRoleAssignment entity) => _mapper.Map<CloudResourceRoleAssignmentDto>(entity);
               
       
    }
}
