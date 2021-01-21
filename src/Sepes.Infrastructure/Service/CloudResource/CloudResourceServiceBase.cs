using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class CloudResourceServiceBase : SandboxServiceBase
    {       

        public CloudResourceServiceBase(SepesDbContext db, IConfiguration config, IMapper mapper, ILogger logger, IUserService userService)
         : base(config, db, mapper, logger, userService)
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

        public async Task<List<CloudResource>> GetActiveResources() => await _db.CloudResources.Include(sr => sr.Sandbox)
                                                                                                   .ThenInclude(sb => sb.Study)
                                                                                                    .Include(sr => sr.Operations)
                                                                                                   .Where(sr => !sr.DeletedAt.HasValue)
                                                                                                   .ToListAsync();

       
        protected async Task<Sandbox> GetSandboxOrThrowAsync(int sandboxId)
        {
            var sandboxFromDb = await _db.Sandboxes
                .Include(sb => sb.Resources)
                    .ThenInclude(r => r.Operations)
                .FirstOrDefaultAsync(sb => sb.Id == sandboxId);

            if (sandboxFromDb == null)
            {
                throw NotFoundException.CreateForEntity("Sandbox", sandboxId);
            }
            return sandboxFromDb;
        }     
       
    }
}
