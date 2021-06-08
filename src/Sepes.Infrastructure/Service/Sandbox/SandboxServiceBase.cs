using AutoMapper;
using Microsoft.Extensions.Logging;
using Sepes.Common.Interface;
using Sepes.Common.Model;
using Sepes.Common.Response.Sandbox;
using Sepes.Common.Util;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class SandboxServiceBase
    {            
        protected readonly IMapper _mapper;
        protected readonly ILogger _logger;
        protected readonly IUserService _userService;
        protected readonly ISandboxModelService _sandboxModelService;
        protected readonly IStudyPermissionService _studyPermissionService;

        public SandboxServiceBase(IMapper mapper, ILogger logger, IUserService userService, IStudyPermissionService studyPermissionService, ISandboxModelService sandboxModelService)
        {
            _logger = logger;
            _mapper = mapper;          
            _userService = userService;
            _sandboxModelService = sandboxModelService;
            _studyPermissionService = studyPermissionService;
        }

        protected async Task<SandboxDetails> GetSandboxDetailsInternalAsync(int sandboxId)
        {
            var sandboxFromDb = await _sandboxModelService.GetDetailsByIdAsync(sandboxId);

            var sandboxDetailsDto = _mapper.Map<SandboxDetails>(sandboxFromDb);

            await DecorateDto(sandboxFromDb.Study, sandboxDetailsDto.Permissions, sandboxDetailsDto.CurrentPhase);           

            DatasetClassificationUtils.SetRestrictionProperties(sandboxDetailsDto);           

            return sandboxDetailsDto;
        }

        async Task DecorateDto(Study studyDb, SandboxPermissions sandboxPermissions, SandboxPhase phase)
        {
            var currentUser = await _userService.GetCurrentUserAsync();
            var studyPermissionDetails = _mapper.Map<IHasStudyPermissionDetails>(studyDb);

            sandboxPermissions.Delete = _studyPermissionService.HasAccessToOperationForStudy(currentUser, studyPermissionDetails, Common.Constants.UserOperation.Study_Crud_Sandbox);
            sandboxPermissions.Update = _studyPermissionService.HasAccessToOperationForStudy(currentUser, studyPermissionDetails, Common.Constants.UserOperation.Study_Crud_Sandbox);
            sandboxPermissions.EditInboundRules = _studyPermissionService.HasAccessToOperationForStudy(currentUser, studyPermissionDetails, Common.Constants.UserOperation.Sandbox_EditInboundRules);
            sandboxPermissions.OpenInternet = phase > SandboxPhase.Open ? currentUser.Admin : _studyPermissionService.HasAccessToOperationForStudy(currentUser, studyPermissionDetails, Common.Constants.UserOperation.Sandbox_OpenInternet); //TODO: was it really only admin who could do this?
            sandboxPermissions.IncreasePhase = _studyPermissionService.HasAccessToOperationForStudy(currentUser, studyPermissionDetails, Common.Constants.UserOperation.Sandbox_IncreasePhase);
        }
    }
}
