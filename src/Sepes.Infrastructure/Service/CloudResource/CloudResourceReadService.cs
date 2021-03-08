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
using Sepes.Infrastructure.Service.DataModelService.Interface;
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
        public CloudResourceReadService(SepesDbContext db, IConfiguration config, IMapper mapper, ILogger<CloudResourceReadService> logger, IUserService userService, ISandboxModelService sandboxModelService)
         : base(db, config, mapper, logger, userService, sandboxModelService)
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

      
    }
}
