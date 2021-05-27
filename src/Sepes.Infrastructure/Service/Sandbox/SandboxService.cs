using AutoMapper;
using Microsoft.Extensions.Logging;
using Sepes.Common.Constants;
using Sepes.Common.Dto.Sandbox;
using Sepes.Common.Response.Sandbox;
using Sepes.Common.Util;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class SandboxService : SandboxServiceBase, ISandboxService
    {
        readonly IStudyModelService _studyModelService;
        readonly IStudyWbsValidationService _studyWbsValidationService;
        readonly ISandboxResourceCreateService _sandboxResourceCreateService;
        readonly ISandboxResourceDeleteService _sandboxResourceDeleteService;

        readonly EventId _sandboxCreateEventId = new EventId(30, "Sepes-Event-Sandbox-Create");
        readonly EventId _sandboxDeleteEventId = new EventId(32, "Sepes-Event-Sandbox-Delete");

        public const string SandboxDelete = "Sandbox-Delete";

        public SandboxService(IMapper mapper, ILogger<SandboxService> logger,
            IUserService userService, ISandboxModelService sandboxModelService, IStudyModelService studyModelService, IStudyWbsValidationService studyWbsValidationService ,ISandboxResourceCreateService sandboxCloudResourceService, ISandboxResourceDeleteService sandboxResourceDeleteService)
            : base(mapper, logger, userService, sandboxModelService)
        {
            _studyModelService = studyModelService;
            _sandboxResourceCreateService = sandboxCloudResourceService;
            _sandboxResourceDeleteService = sandboxResourceDeleteService;
            _studyWbsValidationService = studyWbsValidationService;
        }

        public async Task<Sandbox> CreateAsync(int studyId, SandboxCreateDto sandboxCreateDto)
        {
            _logger.LogInformation(_sandboxCreateEventId, "Sandbox {0}: Starting", studyId);

            Sandbox createdSandbox;

            GenericNameValidation.ValidateName(sandboxCreateDto.Name);

            if (String.IsNullOrWhiteSpace(sandboxCreateDto.Region))
            {
                throw new ArgumentException("Region not specified.");
            }

            // Verify that study with that id exists
            var study = await _studyModelService.GetForSandboxCreateAndDeleteAsync(studyId, UserOperation.Study_Crud_Sandbox);

            await  _studyWbsValidationService.ValidateForSandboxCreationOrThrow(study);

            //Check uniqueness of name
            if (await _sandboxModelService.NameIsTaken(studyId, sandboxCreateDto.Name))
            {
                throw new ArgumentException($"A Sandbox called {sandboxCreateDto.Name} allready exists for Study");
            }

            try
            {
                createdSandbox = await _sandboxModelService.AddAsync(study, _mapper.Map<Sandbox>(sandboxCreateDto));

                try
                {
                    await _sandboxResourceCreateService.CreateBasicSandboxResourcesAsync(createdSandbox);
                }
                catch (Exception)
                {
                    //Deleting sandbox entry and all related from DB
                    if (createdSandbox.Id > 0)
                    {
                        await _sandboxResourceDeleteService.UndoResourceCreationAsync(createdSandbox.Id);
                        await _sandboxModelService.HardDeleteAsync(createdSandbox.Id);
                    }

                    throw;
                }

                return createdSandbox;
            }
            catch (Exception ex)
            {
                throw new Exception($"Sandbox creation failed: {ex.Message}", ex);
            }
        }

        public async Task<SandboxDetails> GetSandboxDetailsAsync(int sandboxId)
        {
            return await GetSandboxDetailsInternalAsync(sandboxId);
        }

        public async Task DeleteAsync(int sandboxId)
        {
            _logger.LogWarning(_sandboxDeleteEventId, "Sandbox {0}: Starting", sandboxId);

            await _sandboxModelService.SoftDeleteAsync(sandboxId);
            
            await _sandboxResourceDeleteService.HandleSandboxDeleteAsync(sandboxId, _sandboxDeleteEventId);

            _logger.LogInformation(_sandboxDeleteEventId, "Sandbox {0}: Done", sandboxId);
        }
    }
}
