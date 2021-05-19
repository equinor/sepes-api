using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Common.Constants;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService
{
    public class CloudResourceReadService : CloudResourceServiceBase, ICloudResourceReadService
    {
        public readonly IConfiguration _config;

        public CloudResourceReadService(SepesDbContext db, IConfiguration config, IMapper mapper, ILogger<CloudResourceReadService> logger, IUserService userService, ISandboxModelService sandboxModelService)
         : base(db, config, mapper, logger, userService, sandboxModelService)
        {
            _config = config;
        }

        public async Task<CloudResource> GetByIdNoAccessCheckAsync(int id)
        {
            return await GetInternalWithoutAccessCheckAsync(id);
        }

        public async Task<CloudResource> GetByIdAsync(int id, UserOperation operation)
        {
            return await GetInternalAsync(id, operation, throwIfNotFound: true);
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
