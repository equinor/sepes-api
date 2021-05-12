using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Common.Constants;
using Sepes.Common.Dto;
using Sepes.Common.Exceptions;
using Sepes.Common.Model;
using Sepes.Common.Response.Sandbox;
using Sepes.Common.Util;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class SandboxServiceBase
    {
        protected readonly IConfiguration _configuration;
        protected readonly SepesDbContext _db;
        protected readonly IMapper _mapper;
        protected readonly ILogger _logger;
        protected readonly IUserService _userService;
        protected readonly ISandboxModelService _sandboxModelService;

        public SandboxServiceBase(IConfiguration configuration, SepesDbContext db, IMapper mapper, ILogger logger, IUserService userService, ISandboxModelService sandboxModelService)
        {
            _configuration = configuration;
            _db = db;
            _logger = logger;
            _mapper = mapper;          
            _userService = userService;
            _sandboxModelService = sandboxModelService;
        }

        protected async Task<SandboxDetails> GetSandboxDetailsInternalAsync(int sandboxId)
        {
            var sandboxFromDb = await _sandboxModelService.GetDetailsByIdAsync(sandboxId);

            var sandboxDetailsDto = _mapper.Map<SandboxDetails>(sandboxFromDb);

            await StudyPermissionsUtil.DecorateDto(_userService, sandboxFromDb.Study, sandboxDetailsDto.Permissions, sandboxDetailsDto.CurrentPhase);           

            DatasetClassificationUtils.SetRestrictionProperties(sandboxDetailsDto);           

            return sandboxDetailsDto;
        }
      

        protected async Task<Sandbox> GetWithoutChecks(int sandboxId)
        {
            var sandbox = await _sandboxModelService.GetByIdWithoutPermissionCheckAsync(sandboxId);

            if (sandbox == null)
            {
                throw NotFoundException.CreateForEntity("Sandbox", sandboxId);
            }

            return sandbox;
        }

        protected async Task<Sandbox> GetOrThrowAsync(int sandboxId, UserOperation userOperation, bool withIncludes, bool disableTracking = false)
        {
           return await _sandboxModelService.GetByIdAsync(sandboxId, userOperation, withIncludes, disableTracking);         
        }       

        protected void InitiatePhaseHistory(Sandbox sandbox, UserDto currentUser)
        {
            sandbox.PhaseHistory = new List<SandboxPhaseHistory>
            {
                new SandboxPhaseHistory { Counter = 0, Phase = SandboxPhase.Open, CreatedBy = currentUser.UserName }
            };
        }
    }
}
