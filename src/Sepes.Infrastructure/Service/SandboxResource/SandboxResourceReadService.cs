using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Query;
using Sepes.Infrastructure.Response.Sandbox;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class SandboxResourceReadService : SandboxServiceBase, ISandboxResourceReadService
    {       
        

        public SandboxResourceReadService(IConfiguration config, SepesDbContext db, IMapper mapper, ILogger<SandboxResourceDeleteService> logger, IUserService userService, ISandboxModelService sandboxModelService)
              : base(config, db, mapper, logger, userService, sandboxModelService)
        {          
       
        }


        public async Task<List<SandboxResourceLight>> GetSandboxResourcesLight(int sandboxId)
        {
            var sandboxFromDb = await _sandboxModelService.GetByIdForResourcesAsync(sandboxId);

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

        public async Task<List<CloudResourceDto>> GetSandboxResources(int sandboxId, CancellationToken cancellation = default)
        {
            var queryable = CloudResourceQueries.GetSandboxResourcesQueryable(_db, sandboxId);

            var resources = await queryable.ToListAsync(cancellation);

            return _mapper.Map<List<CloudResourceDto>>(resources);
        }

        public async Task<string> GetSandboxCostanlysis(int sandboxId, CancellationToken cancellation = default)
        {
            var sandboxFromDb = await GetOrThrowAsync(sandboxId, UserOperation.Study_Read, true);
            return AzureResourceUtil.CreateResourceCostLink(_configuration, sandboxFromDb);
        }
    }
}
