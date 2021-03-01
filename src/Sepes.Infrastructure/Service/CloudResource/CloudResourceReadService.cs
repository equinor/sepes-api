using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Query;
using Sepes.Infrastructure.Response.Sandbox;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class CloudResourceReadService : CloudResourceServiceBase, ICloudResourceReadService
    {
        public readonly IConfiguration _config;
        public CloudResourceReadService(SepesDbContext db, IConfiguration config, IMapper mapper, ILogger<CloudResourceReadService> logger, IUserService userService)
         : base(db, config, mapper, logger, userService)
        {
            _config = config;
        } 
        
        public async Task<CloudResource> GetByIdAsync(int id)
        {
            var entityFromDb = await GetOrThrowAsync(id);
            return entityFromDb;
        }  

        public async Task<CloudResource> GetOrThrowAsync(int id)
        {
            return await GetOrThrowInternalAsync(id);
        }

        public async Task<List<SandboxResourceLight>> GetSandboxResourcesLight(int sandboxId)
        {
            var sandboxFromDb = await GetOrThrowAsync(sandboxId, UserOperation.Study_Read, true);

            //Filter out deleted resources
            var resourcesFiltered = sandboxFromDb.Resources
                .Where(r => !SoftDeleteUtil.IsMarkedAsDeleted(r)
                    || (
                    SoftDeleteUtil.IsMarkedAsDeleted(r)
                    && !r.Operations.Where(o => o.OperationType == CloudResourceOperationType.DELETE && o.Status == CloudResourceOperationState.DONE_SUCCESSFUL).Any())

                ).ToList();

            var resourcesMapped = _mapper.Map<List<SandboxResourceLight>>(resourcesFiltered);

            return resourcesMapped;
        }      

        public async Task<List<CloudResource>> GetAllActiveResources() => await _db.CloudResources.Include(sr => sr.Sandbox)
                                                                                                   .ThenInclude(sb => sb.Study)
                                                                                                    .Include(sr => sr.Operations)
                                                                                                   .Where(sr => !sr.Deleted)
                                                                                                   .ToListAsync();

       
       

        public async Task<IEnumerable<CloudResource>> GetDeletedResourcesAsync() => await _db.CloudResources.Include(sr => sr.Operations).Where(sr => sr.Deleted && sr.DeletedAt.HasValue && sr.DeletedAt.Value.AddMinutes(15) < DateTime.UtcNow)
                                                                                                                .ToListAsync();

        public async Task<bool> ResourceIsDeleted(int resourceId)
        {
            var resource = await _db.CloudResources.AsNoTracking().FirstOrDefaultAsync(r => r.Id == resourceId);

            if(resource == null)
            {
                return true;
            }

            return SoftDeleteUtil.IsMarkedAsDeleted(resource);
        }

        public async Task<List<CloudResourceDto>> GetSandboxResources(int sandboxId, CancellationToken cancellation = default)
        {
            var queryable = CloudResourceQueries.GetSandboxResourcesQueryable(_db, sandboxId);

            var resources = await queryable.ToListAsync(cancellation);

            return _mapper.Map<List<CloudResourceDto>>(resources);
        }

        public async Task<string> GetSandboxCostanlysis(int sandboxId, CancellationToken cancellation = default)
        {
            var sandboxFromDb = await GetOrThrowAsync(sandboxId, UserOperation.Study_Read, true);
            return AzureResourceUtil.CreateResourceCostLink(_config, sandboxFromDb);
        }
    }
}
