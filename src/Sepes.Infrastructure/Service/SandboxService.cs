using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Sandbox;
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
        readonly ISandboxResourceService _sandboxResourceService;


        public SandboxService(SepesDbContext db, IMapper mapper, ILogger<SandboxService> logger, IUserService userService, IStudyService studyService, ISandboxResourceService sandboxResourceService)
        {
            _db = db;
            _mapper = mapper;
            _logger = logger;
            _userService = userService;
            _studyService = studyService;
            _sandboxResourceService = sandboxResourceService;

        }

        public async Task<SandboxDto> GetSandbox(int studyId, int sandboxId)
        {
            var sandboxFromDb = await GetSandboxOrThrowAsync(sandboxId);

            if (sandboxFromDb.StudyId != studyId)
            {
                throw new ArgumentException($"Sandbox with id {sandboxId} does not belong to study with id {studyId}");
            }

            //TODO: Check that user can access study 

            return _mapper.Map<SandboxDto>(sandboxFromDb);
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
            var azureSandbox = new SandboxWithCloudResourcesDto() { SandboxId = sandbox.Id, StudyName = studyFromDb.Name, SandboxName = AzureResourceNameUtil.Sandbox(studyFromDb.Name), Region = region, Tags = tags };
            await CreateBasicSandboxResourcesAsync(azureSandbox);

            return await GetSandboxDtoAsync(sandbox.Id);
        }

        async Task<SandboxWithCloudResourcesDto> CreateBasicSandboxResourcesAsync(SandboxWithCloudResourcesDto dto)
        {
            _logger.LogInformation($"Ordering creation of basic sandbox resources for sandbox: {dto.SandboxName}");

            await _sandboxResourceService.CreateSandboxResourceGroup(dto);

            _logger.LogInformation($"Done creating Resource Group for sandbox: {dto.SandboxName}");

            //FOREACH resource to create
            //Create record in CloudResource (remember resourece group)
            //Create record in CloudResourceOperations
            //Add to queue
            //Send notification to worker?

            //Create storage account resource, add to dto
            await ScheduleCreationOfDiagStorageAccount(dto);    
            await ScheduleCreationOfNetworkSecurityGroup(dto);
            await ScheduleCreationOfVirtualNetwork(dto);
            await ScheduleCreationOfVirtualNetwork(dto);
            await ScheduleCreationOfBastion(dto);

            //await ScheduleCreationOfResource(dto, AzureResourceType.NetworkSecurityGroup);
            //await ScheduleCreationOfResource(dto, AzureResourceType.VirtualNetwork);
            //azureSandbox = await CreateDiagStorageAccount(sandboxId, azureSandbox, region, tags);
            //azureSandbox = await CreateNetworkSecurityGroup(sandboxId, azureSandbox, region, tags);
            //azureSandbox = await CreateVirtualNetwork(sandboxId, azureSandbox, region, tags);
            //TODO: Order bastion

            _logger.LogInformation($"Done ordering creation of basic resources for sandbox: {dto.SandboxName}");

            return dto;
        }

        async Task ScheduleCreationOfDiagStorageAccount(SandboxWithCloudResourcesDto dto)
        {
            var resourceEntry = await CreateResource(dto, AzureResourceType.StorageAccount);
            dto.DiagnosticsStorage = resourceEntry;
        }

        async Task ScheduleCreationOfNetworkSecurityGroup(SandboxWithCloudResourcesDto dto)
        {
            var resourceEntry = await CreateResource(dto, AzureResourceType.NetworkSecurityGroup);
            dto.DiagnosticsStorage = resourceEntry;
        }

        async Task ScheduleCreationOfVirtualNetwork(SandboxWithCloudResourcesDto dto)
        {
            var resourceEntry = await CreateResource(dto, AzureResourceType.VirtualNetwork);
            dto.DiagnosticsStorage = resourceEntry;
        }

        async Task ScheduleCreationOfBastion(SandboxWithCloudResourcesDto dto)
        {
            var resourceEntry = await CreateResource(dto, AzureResourceType.Bastion);
            dto.DiagnosticsStorage = resourceEntry;
        }

        async Task<SandboxResourceDto> CreateResource(SandboxWithCloudResourcesDto dto, string resourceType)
        {
            return await _sandboxResourceService.Create(dto, resourceType);
        }


        //Handles all the stuff around creating the resource
        async Task ScheduleCreationOfResource(SandboxWithCloudResourcesDto dto, string resourceType)
        {
            _logger.LogInformation($"Scheduling a resource create for sandbox: {dto.SandboxName}");

            // Create resource-entry
            var sandboxResourceEntry = await _sandboxResourceService.Create(dto, resourceType);

            //Add to dto

            //TODO: Add to queue

            //TODO: Actual creation. Not handled here
            //TODO: Update relevant entries. Not handled here
        }       

        //TODO: Get status for specific resource
        //TODO: Get status for resources for sandbox

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

            if (resourceGroupForSandbox == null)
            {
                throw new Exception($"Unable to find ResourceGroup record in DB for Sandbox {sandboxId}, StudyId: {studyId}");
            }

            _logger.LogInformation($"Terminating sandbox for study {studyFromDb.Name}. Sandbox name: { sandboxFromDb.Name}. Deleting Resource Group {resourceGroupForSandbox.ResourceGroupName} and all it's contents");
            //TODO: Order instead of performing
            //await _resourceGroupService.Delete(sandboxFromDb.Name, resourceGroupForSandbox.ResourceGroupName);

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
