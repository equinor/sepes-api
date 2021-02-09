using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Response.Sandbox;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Service.Queries;
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


        public SandboxServiceBase(IConfiguration configuration, SepesDbContext db, IMapper mapper, ILogger logger, IUserService userService)
        {
            _configuration = configuration;
            _db = db;
            _logger = logger;
            _mapper = mapper;          
            _userService = userService;

        }

        protected async Task<SandboxDetails> GetSandboxDetailsInternalAsync(int sandboxId)
        {
            var sandboxFromDb = await GetOrThrowAsync(sandboxId, UserOperation.Study_Read, true);
            var sandboxDto = _mapper.Map<SandboxDetails>(sandboxFromDb);

            await StudyPermissionsUtil.DecorateDto(_userService, sandboxFromDb.Study, sandboxDto.Permissions, sandboxDto.CurrentPhase);

            DatasetClassificationUtils.SetRestrictionProperties(sandboxDto);           

            return sandboxDto;
        }

        protected async Task<Sandbox> GetWithoutChecks(int sandboxId)
        {
            var sandbox = await SandboxSingularQueries.GetSandboxByIdNoChecks(_db, sandboxId);

            if (sandbox == null)
            {
                throw NotFoundException.CreateForEntity("Sandbox", sandboxId);
            }

            return sandbox;
        }

        protected async Task<bool> SandboxWithNameAllreadyExists(string sandboxName)
        {
            return await SandboxSingularQueries.SandboxWithNameAllreadyExists(_db, sandboxName);           
        }

        protected async Task<Sandbox> GetOrThrowAsync(int sandboxId, UserOperation userOperation, bool withIncludes)
        {
            var sandbox = await SandboxSingularQueries.GetSandboxByIdCheckAccessOrThrow(_db, _userService, sandboxId, userOperation, withIncludes);

            if (sandbox == null)
            {
                throw NotFoundException.CreateForEntity("Sandbox", sandboxId);
            }

            return sandbox;
        }

        protected async Task<SandboxDto> GetDtoAsync(int sandboxId, UserOperation userOperation, bool withIncludes = false)
        {
            var sandboxFromDb = await GetOrThrowAsync(sandboxId, userOperation, withIncludes);
            var sandboxDto = _mapper.Map<SandboxDto>(sandboxFromDb);
            return sandboxDto;
        }

        protected void InitiatePhaseHistory(Sandbox sandbox, UserDto currentUser)
        {
            sandbox.PhaseHistory = new List<SandboxPhaseHistory>();
            sandbox.PhaseHistory.Add(new SandboxPhaseHistory { Counter = 0, Phase = SandboxPhase.Open, CreatedBy = currentUser.UserName });
        }
    }
}
