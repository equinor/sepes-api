using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class CloudResourceServiceBase : SandboxServiceBase
    {       

        public CloudResourceServiceBase(SepesDbContext db, IConfiguration config, IMapper mapper, ILogger logger, IUserService userService, ISandboxModelService sandboxModelService)
         : base(config, db, mapper, logger, userService, sandboxModelService)
        {           
          
        }

        protected async Task<CloudResource> GetInternalAsync(int id)
        {
           return await _db.CloudResources
                    .Include(r => r.Sandbox)
                    .ThenInclude(s => s.Resources)
                    .Include(r => r.Operations)
                    .FirstOrDefaultAsync(s => s.Id == id);         

        }

        protected async Task<CloudResource> GetOrThrowInternalAsync(int id)
        {
            var entityFromDb = await GetInternalAsync(id);

            if (entityFromDb == null)
            {
                throw NotFoundException.CreateForEntity("SandboxResource", id);
            }

            return entityFromDb;
        }

        public async Task<CloudResourceDto> GetDtoByIdAsync(int id)
        {
            var entityFromDb = await GetOrThrowInternalAsync(id);
            var dto = MapEntityToDto(entityFromDb);

            return dto;
        }

        protected CloudResourceDto MapEntityToDto(CloudResource entity) => _mapper.Map<CloudResourceDto>(entity);         
       
    }
}
