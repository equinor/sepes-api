using AutoMapper;
using Microsoft.Extensions.Logging;
using Sepes.Common.Response.Sandbox;
using Sepes.Common.Util;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class SandboxServiceBase
    {            
        protected readonly IMapper _mapper;
        protected readonly ILogger _logger;
        protected readonly IUserService _userService;
        protected readonly ISandboxModelService _sandboxModelService;

        public SandboxServiceBase(IMapper mapper, ILogger logger, IUserService userService, ISandboxModelService sandboxModelService)
        {
                  
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
    }
}
