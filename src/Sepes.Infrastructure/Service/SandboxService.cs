using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Interface;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Service.Queries;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class SandboxService : SandboxServiceBase, ISandboxService
    {
        readonly IRequestIdService _requestIdService;
        readonly IStudyService _studyService;
        readonly ISandboxCloudResourceService _sandboxCloudResourceService;     


        public SandboxService(IConfiguration config, SepesDbContext db, IMapper mapper, ILogger<SandboxService> logger,
            IUserService userService,
            IRequestIdService requestIdService, IStudyService studyService, ISandboxCloudResourceService sandboxCloudResourceService)
            :base (config, db, mapper, logger, userService)
        {
        
            _requestIdService = requestIdService;
            _studyService = studyService;       
            _sandboxCloudResourceService = sandboxCloudResourceService;
        }

        public async Task<SandboxDto> GetAsync(int sandboxId, UserOperation userOperation)
        {
            return await GetDtoAsync(sandboxId, userOperation);
        }

        public async Task<SandboxDetailsDto> GetSandboxDetailsAsync(int sandboxId)
        {
            var sandboxFromDb = await GetOrThrowAsync(sandboxId, UserOperation.Study_Read, true);
            var sandboxDto = _mapper.Map<SandboxDetailsDto>(sandboxFromDb);

            await StudyPermissionsUtil.DecorateDto(_userService, sandboxFromDb.Study, sandboxDto.Permissions);

            return sandboxDto;
        }

        public async Task<IEnumerable<SandboxDto>> GetAllForStudy(int studyId)
        {
            var studyFromDb = await StudySingularQueries.GetStudyByIdCheckAccessOrThrow(_db, _userService, studyId, UserOperation.Study_Read, true);

            var sandboxesFromDb = await _db.Sandboxes.Where(s => s.StudyId == studyId && (!s.Deleted.HasValue || s.Deleted.Value == false)).ToListAsync();
            var sandboxDTOs = _mapper.Map<IEnumerable<SandboxDto>>(sandboxesFromDb);

            return sandboxDTOs;
        }

        public async Task<SandboxDetailsDto> CreateAsync(int studyId, SandboxCreateDto sandboxCreateDto)
        {
            _logger.LogInformation(SepesEventId.SandboxCreate, "Sandbox {0}: Starting", studyId);

            Sandbox createdSandbox = null;

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
            if (await _db.Sandboxes.Where(sb => sb.StudyId == studyId && sb.Name == sandboxCreateDto.Name && !sb.Deleted.HasValue).AnyAsync())
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

                    var region = RegionStringConverter.Convert(sandboxCreateDto.Region);

                    //This objects gets passed around
                    var creationAndSchedulingDto = new SandboxResourceCreationAndSchedulingDto() { SandboxId = createdSandbox.Id, StudyName = studyDto.Name, SandboxName = sandboxDto.Name, Region = region, Tags = tags, BatchId = Guid.NewGuid().ToString() };

                    await _sandboxCloudResourceService.CreateBasicSandboxResourcesAsync(creationAndSchedulingDto);
                }
                catch (Exception ex)
                {
                    //Deleting sandbox entry from DB
                    study.Sandboxes.Remove(createdSandbox);
                    await _db.SaveChangesAsync();
                    throw;
                }

                return await GetSandboxDetailsAsync(createdSandbox.Id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Sandbox creation failed: {ex.Message}", ex);
            }
        } 
        
        public void InitiatePhaseHistory(Sandbox sandbox, UserDto currentUser)
        {
            sandbox.PhaseHistory = new List<SandboxPhaseHistory>();
            sandbox.PhaseHistory.Add(new SandboxPhaseHistory { Counter = 0, Phase = SandboxPhase.Open, CreatedBy = currentUser.UserName });
        }

        public async Task MoveToNextPhase(int sandboxId)
        {
            _logger.LogInformation(SepesEventId.SandboxNextPhase, "Sandbox {0}: Starting", sandboxId);

            try
            {
                var user = await _userService.GetCurrentUserAsync();

                var sandboxFromDb = await GetOrThrowAsync(sandboxId, UserOperation.SandboxLock, true);

                var currentPhaseItem = SandboxPhaseUtil.GetCurrentPhaseHistoryItem(sandboxFromDb);

                var nextPhase = SandboxPhaseUtil.GetNextPhase(sandboxFromDb);

                _logger.LogInformation(SepesEventId.SandboxNextPhase, "Sandbox {0}: Moving from {1} to {2}", sandboxId, currentPhaseItem.Phase, nextPhase);

                sandboxFromDb.PhaseHistory.Add(new SandboxPhaseHistory() { Counter = currentPhaseItem.Counter + 1, Phase = nextPhase, CreatedBy = user.UserName });
                await _db.SaveChangesAsync();
                _logger.LogInformation(SepesEventId.SandboxNextPhase, "Sandbox {0}: Phase added to db. Proceeding to make data available", sandboxId);

                //Make data available


                _logger.LogInformation(SepesEventId.SandboxNextPhase, "Sandbox {0}: Done", sandboxId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Moving to next phase failed", ex);
            }
        }

        public async Task DeleteAsync(int sandboxId)
        {
            _logger.LogWarning(SepesEventId.SandboxDelete, "Sandbox {0}: Starting", sandboxId);

            var sandboxFromDb = await GetOrThrowAsync(sandboxId, UserOperation.Study_Crud_Sandbox, true);         

            int studyId = sandboxFromDb.StudyId;

            var user = await _userService.GetCurrentUserAsync();

            _logger.LogInformation(SepesEventId.SandboxDelete, "Study {0}, Sandbox {1}: Marking sandbox record for deletion", studyId, sandboxId);

            //Mark sandbox object as deleted
            sandboxFromDb.Deleted = true;
            sandboxFromDb.DeletedAt = DateTime.UtcNow;
            sandboxFromDb.DeletedBy = user.UserName;

            await _db.SaveChangesAsync();

            await _sandboxCloudResourceService.HandleSandboxDeleteAsync(sandboxId);           

            _logger.LogInformation(SepesEventId.SandboxDelete, "Study {0}, Sandbox {1}: Done", studyId, sandboxId);
        }     
    }
}
