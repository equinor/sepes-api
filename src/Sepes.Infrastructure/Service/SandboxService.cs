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
        readonly IResourceProvisioningQueueService _provisioningQueueService;

        public SandboxService(SepesDbContext db, IMapper mapper, ILogger<SandboxService> logger, IUserService userService, IStudyService studyService, ISandboxResourceService sandboxResourceService, IResourceProvisioningQueueService provisioningQueueService)
        {
            _db = db;
            _mapper = mapper;
            _logger = logger;
            _userService = userService;
            _studyService = studyService;
            _sandboxResourceService = sandboxResourceService;
            _provisioningQueueService = provisioningQueueService;

        }

        public async Task<SandboxDto> GetSandbox(int studyId, int sandboxId)
        {
            var sandboxFromDb = await GetSandboxOrThrowAsync(sandboxId, AccessType.SANDBOX_READ);

            if (sandboxFromDb.StudyId != studyId)
            {
                throw new ArgumentException($"Sandbox with id {sandboxId} does not belong to study with id {studyId}");
            }

            //TODO: Check that user can access study 

            return _mapper.Map<SandboxDto>(sandboxFromDb);
        }

        public async Task<IEnumerable<SandboxDto>> GetSandboxesForStudyAsync(int studyId)
        {
            var studyFromDb = await StudyAccessUtil.GetStudyAndCheckAccessOrThrow(_db, _userService, studyId, AccessType.SANDBOX_READ);
   
            var sandboxesFromDb = await _db.Sandboxes.Where(s => s.StudyId == studyId && (!s.Deleted.HasValue || s.Deleted.Value == false)).ToListAsync();
            var sandboxDTOs = _mapper.Map<IEnumerable<SandboxDto>>(sandboxesFromDb);

            return sandboxDTOs;
        }


        // TODO Validate azure things
        public async Task<StudyDto> ValidateSandboxAsync(int studyId, SandboxDto newSandbox)
        {
            var studyFromDb = await StudyAccessUtil.GetStudyAndCheckAccessOrThrow(_db, _userService, studyId, AccessType.SANDBOX_READ);
            return await ValidateSandboxAsync(studyFromDb, newSandbox);
        }

        Task<StudyDto> ValidateSandboxAsync(Study study, SandboxDto newSandbox)
        {
            throw new NotImplementedException();
        }

        public async Task<SandboxDto> CreateAsync(int studyId, SandboxCreateDto sandboxCreateDto)
        {
            // Verify that study with that id exists
            var studyFromDb = await StudyAccessUtil.GetStudyAndCheckAccessOrThrow(_db, _userService, studyId, AccessType.SANDBOX_READ);

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


        async Task<Sandbox> GetSandboxOrThrowAsync(int sandboxId, string accessType = AccessType.SANDBOX_READ)
        {
            var sandboxFromDb = await _db.Sandboxes
                .Include(sb => sb.Resources)
                    .ThenInclude(r => r.Operations)
                .FirstOrDefaultAsync(sb => sb.Id == sandboxId && (!sb.Deleted.HasValue || !sb.Deleted.Value));

            if (sandboxFromDb == null)
            {
                throw NotFoundException.CreateForEntity("Sandbox", sandboxId);
            }

            //Check that relevant access to Study is present
            var studyFromDb = await StudyAccessUtil.GetStudyAndCheckAccessOrThrow(_db, _userService, sandboxFromDb.StudyId, accessType);

            return sandboxFromDb;
        }

        async Task<SandboxDto> GetSandboxDtoAsync(int sandboxId)
        {
            var sandboxFromDb = await GetSandboxOrThrowAsync(sandboxId);
            return _mapper.Map<SandboxDto>(sandboxFromDb);
        }

        async Task<SandboxWithCloudResourcesDto> CreateBasicSandboxResourcesAsync(SandboxWithCloudResourcesDto dto)
        {
            _logger.LogInformation($"Ordering creation of basic sandbox resources for sandbox: {dto.SandboxName}");

            await _sandboxResourceService.CreateSandboxResourceGroup(dto);

            _logger.LogInformation($"Done creating Resource Group for sandbox: {dto.SandboxName}");

            var queueParentItem = new ProvisioningQueueParentDto();
            queueParentItem.SandboxId = dto.SandboxId;
            queueParentItem.Description = $"Create basic resources for Sandbox: {dto.SandboxId}";

            //Create storage account resource, add to dto
            await ScheduleCreationOfDiagStorageAccount(dto, queueParentItem);
            await ScheduleCreationOfNetworkSecurityGroup(dto, queueParentItem);
            await ScheduleCreationOfVirtualNetwork(dto, queueParentItem);
            await ScheduleCreationOfBastion(dto, queueParentItem);

            await _provisioningQueueService.SendMessageAsync(queueParentItem);

            //Send notification to worker and tell it that dinner is ready?

            _logger.LogInformation($"Done ordering creation of basic resources for sandbox: {dto.SandboxName}");

            return dto;
        }

        async Task ScheduleCreationOfDiagStorageAccount(SandboxWithCloudResourcesDto dto, ProvisioningQueueParentDto queueParentItem)
        {
            var resourceEntry = await CreateResource(dto, queueParentItem, AzureResourceType.StorageAccount);
            dto.DiagnosticsStorage = resourceEntry;
        }

        async Task ScheduleCreationOfNetworkSecurityGroup(SandboxWithCloudResourcesDto dto, ProvisioningQueueParentDto queueParentItem)
        {
            var resourceEntry = await CreateResource(dto, queueParentItem, AzureResourceType.NetworkSecurityGroup);
            dto.NetworkSecurityGroup = resourceEntry;
        }

        async Task ScheduleCreationOfVirtualNetwork(SandboxWithCloudResourcesDto dto, ProvisioningQueueParentDto queueParentItem)
        {
            //TODO: Add special network rules to resource
            var resourceEntry = await CreateResource(dto, queueParentItem, AzureResourceType.VirtualNetwork);
            dto.Network = resourceEntry;
        }

        async Task ScheduleCreationOfBastion(SandboxWithCloudResourcesDto dto, ProvisioningQueueParentDto queueParentItem)
        {
            var resourceEntry = await CreateResource(dto, queueParentItem, AzureResourceType.Bastion);
            dto.Bastion = resourceEntry;
        }

        async Task<SandboxResourceDto> CreateResource(SandboxWithCloudResourcesDto dto, ProvisioningQueueParentDto queueParentItem, string resourceType)
        {
            var resourceEntry = await _sandboxResourceService.Create(dto, resourceType);
            queueParentItem.Children.Add(new ProvisioningQueueChildDto() { SandboxResourceId = resourceEntry.Id.Value, SandboxResourceOperationId = resourceEntry.Operations.FirstOrDefault().Id.Value });

            return resourceEntry;
        }

        public async Task<List<SandboxResourceLightDto>> GetSandboxResources(int studyId, int sandboxId)
        {
            var sandboxFromDb = await GetSandboxOrThrowAsync(sandboxId, AccessType.SANDBOX_READ);
            var resources = _mapper.Map<List<SandboxResourceLightDto>>(sandboxFromDb.Resources);
            return resources;
        }

        public async Task<SandboxDto> DeleteAsync(int studyId, int sandboxId)
        {
            _logger.LogWarning(SepesEventId.SandboxDelete, "Deleting sandbox with id {0}, for study {1}", studyId, sandboxId);

            // Run validations: (Check if ID is valid)
            var studyFromDb = await StudyAccessUtil.GetStudyAndCheckAccessOrThrow(_db, _userService, studyId, AccessType.SANDBOX_READ);
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
