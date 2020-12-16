using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Interface;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Query;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class SandboxResourceService : SandboxResourceServiceBase, ISandboxResourceService
    { 
        public SandboxResourceService(SepesDbContext db, IConfiguration config, IMapper mapper, ILogger<SandboxResourceService> logger, IUserService userService)
         : base(db, config, mapper, logger, userService)
        { 
        } 
        
        public async Task<SandboxResource> GetByIdAsync(int id)
        {
            var entityFromDb = await GetOrThrowAsync(id);
            return entityFromDb;
        }  

        public async Task<SandboxResource> GetOrThrowAsync(int id)
        {
            return await GetOrThrowInternalAsync(id);
        }

        public async Task<List<SandboxResourceLightDto>> GetSandboxResourcesLight(int sandboxId)
        {
            var sandboxFromDb = await GetOrThrowAsync(sandboxId, UserOperation.Study_Read, true);

            //Filter out deleted resources
            var resourcesFiltered = sandboxFromDb.Resources
                .Where(r => !r.Deleted.HasValue
                || (r.Deleted.HasValue && r.Operations.Where(o => o.OperationType == CloudResourceOperationType.DELETE && o.Status == CloudResourceOperationState.DONE_SUCCESSFUL).Any() == false)

                ).ToList();

            var resourcesMapped = _mapper.Map<List<SandboxResourceLightDto>>(resourcesFiltered);

            return resourcesMapped;
        }

      

        public async Task<List<SandboxResource>> GetAllActiveResources() => await _db.SandboxResources.Include(sr => sr.Sandbox)
                                                                                                   .ThenInclude(sb => sb.Study)
                                                                                                    .Include(sr => sr.Operations)
                                                                                                   .Where(sr => !sr.Deleted.HasValue)
                                                                                                   .ToListAsync();

       
       

        public async Task<IEnumerable<SandboxResource>> GetDeletedResourcesAsync() => await _db.SandboxResources.Include(sr => sr.Operations).Where(sr => sr.Deleted.HasValue && sr.Deleted.Value.AddMinutes(10) < DateTime.UtcNow)
                                                                                                                .ToListAsync();

        public async Task<bool> ResourceIsDeleted(int resourceId)
        {
            var resource = await _db.SandboxResources.AsNoTracking().FirstOrDefaultAsync(r => r.Id == resourceId);

            if(resource == null)
            {
                return true;
            }

            if (resource.Deleted.HasValue || !String.IsNullOrWhiteSpace(resource.DeletedBy) )
            {
                return true;
            } 
            
            return false;
        }

        public async Task<List<SandboxResourceDto>> GetSandboxResources(int sandboxId, CancellationToken cancellation = default)
        {
            var queryable = SandboxResourceQueries.GetSandboxResourcesQueryable(_db, sandboxId);

            var resources = await queryable.ToListAsync(cancellation);

            return _mapper.Map<List<SandboxResourceDto>>(resources);
        }
    }
}
