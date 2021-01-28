using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Service.Queries;
using Sepes.Infrastructure.Util;
using Sepes.Infrastructure.Util.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class SandboxService : SandboxServiceBase, ISandboxService
    {
        readonly IStudyService _studyService;
        readonly ISandboxResourceCreateService _sandboxResourceCreateService;
        readonly ISandboxResourceDeleteService _sandboxResourceDeleteService;

        public SandboxService(IConfiguration config, SepesDbContext db, IMapper mapper, ILogger<SandboxService> logger,
            IUserService userService, IStudyService studyService, ISandboxResourceCreateService sandboxCloudResourceService, ISandboxResourceDeleteService sandboxResourceDeleteService)
            : base(config, db, mapper, logger, userService)
        {
            _studyService = studyService;
            _sandboxResourceCreateService = sandboxCloudResourceService;
            _sandboxResourceDeleteService = sandboxResourceDeleteService;
        }

        public async Task<SandboxDto> GetAsync(int sandboxId, UserOperation userOperation, bool withIncludes = false)
        {
            return await GetDtoAsync(sandboxId, userOperation);
        }

        public async Task<SandboxDetailsDto> GetSandboxDetailsAsync(int sandboxId)
        {
            return await GetSandboxDetailsInternalAsync(sandboxId);
        }

        public async Task<IEnumerable<SandboxDto>> GetAllForStudy(int studyId)
        {
            var studyFromDb = await StudySingularQueries.GetStudyByIdCheckAccessOrThrow(_db, _userService, studyId, UserOperation.Study_Read, true);

            var sandboxesFromDb = await _db.Sandboxes.Where(s => s.StudyId == studyId && s.Deleted == false).ToListAsync();
            var sandboxDTOs = _mapper.Map<IEnumerable<SandboxDto>>(sandboxesFromDb);

            return sandboxDTOs;
        }

        public async Task<SandboxDetailsDto> CreateAsync(int studyId, SandboxCreateDto sandboxCreateDto)
        {
            _logger.LogInformation(SepesEventId.SandboxCreate, "Sandbox {0}: Starting", studyId);

            Sandbox createdSandbox = null;

            GenericNameValidation.ValidateName(sandboxCreateDto.Name);

            if (String.IsNullOrWhiteSpace(sandboxCreateDto.Region))
            {
                throw new ArgumentException("Region not specified.");
            }

            // Verify that study with that id exists
            var study = await StudySingularQueries.GetStudyByIdCheckAccessOrThrow(_db, _userService, studyId, UserOperation.Study_Crud_Sandbox);

            // Check that study has WbsCode.
            if (String.IsNullOrWhiteSpace(study.WbsCode))
            {
                throw new ArgumentException("WBS code missing in Study. Study requires WBS code before Sandbox can be created.");
            }

            //Check uniqueness of name
            if (await _db.Sandboxes.Where(sb => sb.StudyId == studyId && sb.Name == sandboxCreateDto.Name && sb.Deleted == false).AnyAsync())
            {
                throw new ArgumentException($"A Sandbox called {sandboxCreateDto.Name} allready exists for Study");
            }

            try
            {
                var user = await _userService.GetCurrentUserAsync();

                createdSandbox = _mapper.Map<Sandbox>(sandboxCreateDto);

                InitiatePhaseHistory(createdSandbox, user);

                createdSandbox.CreatedBy = user.UserName;
                createdSandbox.TechnicalContactName = user.FullName;
                createdSandbox.TechnicalContactEmail = user.EmailAddress;

                study.Sandboxes.Add(createdSandbox);

                await _db.SaveChangesAsync();

                try
                {
                    // Get Dtos for arguments to sandboxWorkerService
                    //TODO: Can get on or the other via the other, don't need two?
                    var studyDto = await _studyService.GetStudyDtoByIdAsync(studyId, UserOperation.Study_Crud_Sandbox);
                    var sandboxDto = await GetAsync(createdSandbox.Id, UserOperation.Study_Crud_Sandbox);

                    var tags = AzureResourceTagsFactory.SandboxResourceTags(_configuration, study, createdSandbox);                 

                    //This object gets passed around
                    var creationAndSchedulingDto =
                        new SandboxResourceCreationAndSchedulingDto()
                        {
                            StudyId = studyDto.Id,
                            SandboxId = createdSandbox.Id,
                            StudyName = studyDto.Name,
                            SandboxName = sandboxDto.Name,
                            Region = sandboxCreateDto.Region,
                            Tags = tags,
                            BatchId = Guid.NewGuid().ToString()
                        };

                    await _sandboxResourceCreateService.CreateBasicSandboxResourcesAsync(creationAndSchedulingDto);
                }
                catch (Exception ex)
                {
                    //Deleting sandbox entry and all related from DB
                    if (createdSandbox.Id > 0)
                    {
                        foreach (var curRes in await _db.CloudResources.Include(r => r.Operations).Where(r => r.SandboxId == createdSandbox.Id).ToListAsync())
                        {
                            foreach (var curOp in curRes.Operations)
                            {
                                _db.CloudResourceOperations.Remove(curOp);
                            }

                            _db.CloudResources.Remove(curRes);
                        }

                        study.Sandboxes.Remove(createdSandbox);
                        await _db.SaveChangesAsync();
                    }

                    throw;
                }

                return await GetSandboxDetailsAsync(createdSandbox.Id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Sandbox creation failed: {ex.Message}", ex);
            }
        }

        public async Task DeleteAsync(int sandboxId)
        {
            _logger.LogWarning(SepesEventId.SandboxDelete, "Sandbox {0}: Starting", sandboxId);

            var sandboxFromDb = await GetOrThrowAsync(sandboxId, UserOperation.Study_Crud_Sandbox, true);

            int studyId = sandboxFromDb.StudyId;

            var user = await _userService.GetCurrentUserAsync();

            _logger.LogInformation(SepesEventId.SandboxDelete, "Study {0}, Sandbox {1}: Marking sandbox record for deletion", studyId, sandboxId);

            SoftDeleteUtil.MarkAsDeleted(sandboxFromDb, user);

            await _db.SaveChangesAsync();

            await _sandboxResourceDeleteService.HandleSandboxDeleteAsync(sandboxId);

            _logger.LogInformation(SepesEventId.SandboxDelete, "Study {0}, Sandbox {1}: Done", studyId, sandboxId);
        }
    }
}
