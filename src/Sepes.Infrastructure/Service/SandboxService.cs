using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class SandboxService : ISandboxService
    {
        readonly SepesDbContext _db;
        readonly IMapper _mapper;
        readonly ILogger _logger;
        readonly IUserService _userService;
        readonly IStudyService _studyService;
        readonly ISandboxWorkerService _sandboxWorkerService;

        public SandboxService(SepesDbContext db, IMapper mapper, ILogger<SandboxService> logger, IUserService userService, IStudyService studyService, ISandboxWorkerService sandboxWorkerService)
        {
            _db = db;
            _mapper = mapper;
            _logger = logger;
            _userService = userService;
            _studyService = studyService;
            _sandboxWorkerService = sandboxWorkerService;
        }

        public async Task<IEnumerable<SandboxDto>> GetSandboxesForStudyAsync(int studyId)
        {
            var studyFromDb = await StudyQueries.GetStudyOrThrowAsync(studyId, _db);
            var sandboxesFromDb = await _db.Sandboxes.Where(s => s.StudyId == studyId && (!s.Deleted.HasValue || s.Deleted.Value == false)).ToListAsync();
            var sandboxDTOs = _mapper.Map<IEnumerable<SandboxDto>>(sandboxesFromDb);

            return sandboxDTOs;
        }

        async Task<Sandbox> GetSandboxOrThrowAsync(int sandboxId)
        {
            var sandboxFromDb = await _db.Sandboxes
                .Include(sb => sb.Resources)
                    .ThenInclude(r => r.Operations)
                .FirstOrDefaultAsync(sb => sb.Id == sandboxId && (!sb.Deleted.HasValue || !sb.Deleted.Value));

            if (sandboxFromDb == null)
            {
                throw NotFoundException.CreateForEntity("Sandbox", sandboxId);
            }
            return sandboxFromDb;
        }

        async Task<SandboxDto> GetSandboxDtoAsync(int sandboxId)
        {
            var sandboxFromDb = await GetSandboxOrThrowAsync(sandboxId);
            return _mapper.Map<SandboxDto>(sandboxFromDb);
        }

        // TODO Validate azure things
        public async Task<StudyDto> ValidateSandboxAsync(int studyId, SandboxDto newSandbox)
        {
            var studyFromDb = await StudyQueries.GetStudyOrThrowAsync(studyId, _db);
            return await ValidateSandboxAsync(studyFromDb, newSandbox);

        }

        Task<StudyDto> ValidateSandboxAsync(Study study, SandboxDto newSandbox)
        {
            throw new NotImplementedException();
        }

        public async Task<SandboxDto> CreateAsync(int studyId, SandboxCreateDto sandboxCreateDto)
        {

            // Verify that study with that id exists
            var studyFromDb = await StudyQueries.GetStudyOrThrowAsync(studyId, _db);

            //TODO: Verify that this user can create sandbox for study

            // Check that study has WbsCode.
            if (String.IsNullOrWhiteSpace(studyFromDb.WbsCode))
            {
                throw new ArgumentException("WBS code missing in Study. Study requires WBS code before sandbox can be created.");
            }

            if (String.IsNullOrWhiteSpace(sandboxCreateDto.Region))
            {
                throw new ArgumentException("Region not specified.");
            }

            var sandbox = _mapper.Map<Sandbox>(sandboxCreateDto);

            var user = _userService.GetCurrentUser();

            sandbox.TechnicalContactName = user.FullName;
            sandbox.TechnicalContactEmail = user.Email;      

            studyFromDb.Sandboxes.Add(sandbox);
            await _db.SaveChangesAsync();

            // Get Dtos for arguments to sandboxWorkerService
            var studyDto = await _studyService.GetStudyByIdAsync(studyId);
            var sandboxDto = await GetSandboxDtoAsync(sandbox.Id);
            //TODO: Remember to consider templates specifed as argument


            var tags = AzureResourceTagsFactory.CreateTags(studyFromDb.Name, studyDto, sandboxDto);

            var region = RegionStringConverter.Convert(sandboxCreateDto.Region);

            //Her har vi mye info om sandboxen i Azure, men den har for mye info
           await _sandboxWorkerService.CreateBasicSandboxResourcesAsync(sandbox.Id, region, studyFromDb.Name, tags);

            return await GetSandboxDtoAsync(sandbox.Id);
        }

        // TODO: Implement deletion of Azure resources. Only deletes from SEPES db as of now
        //Todo, add a deleted flag instead of actually deleting, so that we keep history
        public async Task<SandboxDto> DeleteAsync(int studyId, int sandboxId)
        {
            _logger.LogWarning(SepesEventId.SandboxDelete, "Deleting sandbox with id {0}, for study {1}", studyId, sandboxId);

            // Run validations: (Check if ID is valid)
            var studyFromDb = await StudyQueries.GetStudyOrThrowAsync(studyId, _db);
            var sandboxFromDb = await _db.Sandboxes.FirstOrDefaultAsync(sb => sb.Id == sandboxId && (!sb.Deleted.HasValue || !sb.Deleted.Value));

            if (sandboxFromDb == null)
            {
                throw NotFoundException.CreateForEntity("Sandbox", sandboxId);
            }

            SetSandboxAsDeleted(sandboxFromDb);

            await _db.SaveChangesAsync();

            //Find resource group name
            var resourceGroupForSandbox = sandboxFromDb.Resources.SingleOrDefault(r => r.ResourceType == AzureResourceType.ResourceGroup);

            if(resourceGroupForSandbox == null)
            {
                throw new Exception($"Unable to find ResourceGroup record in DB for Sandbox {sandboxId}, StudyId: {studyId}");
            }

            await _sandboxWorkerService.NukeSandbox(studyFromDb.Name, sandboxFromDb.Name, resourceGroupForSandbox.ResourceGroupName); 
            
            _logger.LogWarning(SepesEventId.SandboxDelete, "Sandbox with id {0} deleted", studyId);
            return _mapper.Map<SandboxDto>(sandboxFromDb);
        }

        void SetSandboxAsDeleted(Sandbox sandbox)
        {
            var user = _userService.GetCurrentUser();

            //Mark sandbox object as deleted
            sandbox.Deleted = true;
            sandbox.DeletedAt = DateTime.UtcNow;
            sandbox.DeletedBy = user.UserName;

            //Mark all resources as deleted
            foreach (var curResource in sandbox.Resources)
            {
                curResource.Deleted = DateTime.UtcNow;
                curResource.DeletedBy = user.UserName;               
            }
        }

        //public Task<IEnumerable<SandboxTemplateDto>> GetTemplatesAsync()
        //{
        //    return templates;          
        //}
    }
}
