using AutoMapper;
using Microsoft.Extensions.Configuration;
using Sepes.Common.Constants;
using Sepes.Common.Constants.CloudResource;
using Sepes.Common.Response.Sandbox;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class SandboxResourceReadService : ISandboxResourceReadService
    {
        protected readonly IMapper _mapper;
        protected readonly IConfiguration _configuration;        
        protected readonly ISandboxModelService _sandboxModelService;

        public SandboxResourceReadService(IMapper mapper, IConfiguration config, ISandboxModelService sandboxModelService)
             
        {
            _mapper = mapper;
            _configuration = config;            
            _sandboxModelService = sandboxModelService;
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
      

        public async Task<string> GetSandboxCostanlysis(int sandboxId, CancellationToken cancellation = default)
        {
            var sandboxFromDb = await _sandboxModelService.GetByIdForCostAnalysisLinkAsync(sandboxId, UserOperation.Study_Read);
            return CloudResourceUtil.CreateResourceCostLink(_configuration, sandboxFromDb);
        }
    }
}
