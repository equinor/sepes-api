using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Interface;
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
        readonly IConfiguration _config;
        readonly SepesDbContext _db;
        readonly IMapper _mapper;
        readonly ILogger _logger;
        readonly IUserService _userService;
        readonly IRequestIdService _requestIdService;
        readonly IStudyService _studyService;
        readonly ISandboxResourceService _sandboxResourceService;
        readonly IProvisioningQueueService _provisioningQueueService;


        public SandboxService(IConfiguration config, SepesDbContext db, IMapper mapper, ILogger<SandboxService> logger, IUserService userService, IRequestIdService requestIdService, IStudyService studyService, ISandboxResourceService sandboxResourceService, IProvisioningQueueService provisioningQueueService)
        {
            _db = db;
            _mapper = mapper;
            _logger = logger;
            _userService = userService;
            _requestIdService = requestIdService;
            _studyService = studyService;
            _sandboxResourceService = sandboxResourceService;
            _provisioningQueueService = provisioningQueueService;
            _config = config;
        }

        public async Task<SandboxDto> GetSandboxAsync(int sandboxId)
        {
            var sandboxFromDb = await GetSandboxOrThrowAsync(sandboxId, UserOperations.SandboxEdit);

            return _mapper.Map<SandboxDto>(sandboxFromDb);
        }

        public async Task<IEnumerable<SandboxDto>> GetSandboxesForStudyAsync(int studyId)
        {
            var studyFromDb = await StudyAccessUtil.GetStudyByIdCheckAccessOrThrow(_db, _userService, studyId, UserOperations.StudyAddRemoveSandbox);

            var sandboxesFromDb = await _db.Sandboxes.Where(s => s.StudyId == studyId && (!s.Deleted.HasValue || s.Deleted.Value == false)).ToListAsync();
            var sandboxDTOs = _mapper.Map<IEnumerable<SandboxDto>>(sandboxesFromDb);

            return sandboxDTOs;
        }

        public async Task<SandboxDto> CreateAsync(int studyId, SandboxCreateDto sandboxCreateDto)
        {
            Sandbox createdSandbox = null;

            if (String.IsNullOrWhiteSpace(sandboxCreateDto.Region))
            {
                throw new ArgumentException("Region not specified.");
            }

            // Verify that study with that id exists
            var study = await StudyAccessUtil.GetStudyByIdCheckAccessOrThrow(_db, _userService, studyId, UserOperations.StudyAddRemoveSandbox);

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
                createdSandbox = _mapper.Map<Sandbox>(sandboxCreateDto);

                var user = _userService.GetCurrentUser();
                createdSandbox.CreatedBy = user.UserName;
                createdSandbox.TechnicalContactName = user.FullName;
                createdSandbox.TechnicalContactEmail = user.EmailAddress;

                study.Sandboxes.Add(createdSandbox);
                await _db.SaveChangesAsync();

                try
                {
                    // Get Dtos for arguments to sandboxWorkerService
                    var studyDto = await _studyService.GetStudyDtoByIdAsync(studyId, UserOperations.StudyAddRemoveSandbox);
                    var sandboxDto = await GetSandboxDtoAsync(createdSandbox.Id);

                    var tags = AzureResourceTagsFactory.CreateTags(_config, studyDto, sandboxDto);

                    var region = RegionStringConverter.Convert(sandboxCreateDto.Region);

                    //This objects gets passed around
                    var creationAndSchedulingDto = new SandboxResourceCreationAndSchedulingDto() { SandboxId = createdSandbox.Id, StudyName = studyDto.Name, SandboxName = sandboxDto.Name, Region = region, Tags = tags, BatchId = Guid.NewGuid().ToString() };

                    await CreateBasicSandboxResourcesAsync(creationAndSchedulingDto);
                }
                catch (Exception ex)
                {
                    //Deleting sandbox entry from DB
                    study.Sandboxes.Remove(createdSandbox);
                    await _db.SaveChangesAsync();
                    throw;
                }

                return await GetSandboxDtoAsync(createdSandbox.Id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Sandbox creation failed: {ex.Message}", ex);
            }
        }


        async Task<Sandbox> GetSandboxOrThrowAsync(int sandboxId, UserOperations userOperation = UserOperations.SandboxEdit)
        {
            var sandboxFromDb = await _db.Sandboxes
                .Include(sb => sb.SandboxDatasets)
                    .ThenInclude(sd => sd.Dataset)
                .Include(sb => sb.Resources)
                    .ThenInclude(r => r.Operations)
                .FirstOrDefaultAsync(sb => sb.Id == sandboxId && (!sb.Deleted.HasValue || !sb.Deleted.Value));

            if (sandboxFromDb == null)
            {
                throw NotFoundException.CreateForEntity("Sandbox", sandboxId);
            }

            //Ensure user is allowed to perform this action
            _ = await StudyAccessUtil.GetStudyByIdCheckAccessOrThrow(_db, _userService, sandboxFromDb.StudyId, userOperation);

            return sandboxFromDb;
        }

        async Task<SandboxDto> GetSandboxDtoAsync(int sandboxId)
        {
            var sandboxFromDb = await GetSandboxOrThrowAsync(sandboxId);
            return _mapper.Map<SandboxDto>(sandboxFromDb);
        }

        async Task<SandboxResourceCreationAndSchedulingDto> CreateBasicSandboxResourcesAsync(SandboxResourceCreationAndSchedulingDto dto)
        {
            _logger.LogInformation($"Creating basic sandbox resources for sandbox: {dto.SandboxName}. First creating Resource Group, other resources are created by worker");

            try
            {               
                await _sandboxResourceService.CreateSandboxResourceGroup(dto);

                _logger.LogInformation($"Done creating Resource Group for sandbox: {dto.SandboxName}. Scheduling creation of other resources");

                var queueParentItem = new ProvisioningQueueParentDto
                {
                    SandboxId = dto.SandboxId,
                    Description = $"Create basic resources for Sandbox: {dto.SandboxId}"
                };

                await ScheduleCreationOfDiagStorageAccount(dto, queueParentItem);
                await ScheduleCreationOfNetworkSecurityGroup(dto, queueParentItem);
                await ScheduleCreationOfVirtualNetwork(dto, queueParentItem);
                await ScheduleCreationOfBastion(dto, queueParentItem);

                await _provisioningQueueService.SendMessageAsync(queueParentItem);

                _logger.LogInformation($"Done ordering creation of basic resources for sandbox: {dto.SandboxName}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to create basic sandbox resources.", ex);
            }

            return dto;
        }

        async Task ScheduleCreationOfDiagStorageAccount(SandboxResourceCreationAndSchedulingDto dto, ProvisioningQueueParentDto queueParentItem)
        {
            var resourceName = AzureResourceNameUtil.DiagnosticsStorageAccount(dto.StudyName, dto.SandboxName);
            var resourceGroupCreateOperation = dto.ResourceGroup.Operations.FirstOrDefault().Id.Value;
            var resourceEntry = await CreateResource(dto, queueParentItem, AzureResourceType.StorageAccount, sandboxControlled: true, resourceName: resourceName, dependsOn: resourceGroupCreateOperation);
            dto.DiagnosticsStorage = resourceEntry;

        }

        async Task ScheduleCreationOfNetworkSecurityGroup(SandboxResourceCreationAndSchedulingDto dto, ProvisioningQueueParentDto queueParentItem)
        {
            var nsgName = AzureResourceNameUtil.NetworkSecGroupSubnet(dto.StudyName, dto.SandboxName);
            var diagStorageAccountCreateOperation = dto.DiagnosticsStorage.Operations.FirstOrDefault().Id.Value;
            var resourceEntry = await CreateResource(dto, queueParentItem, AzureResourceType.NetworkSecurityGroup, sandboxControlled: true, resourceName: nsgName, dependsOn: diagStorageAccountCreateOperation);
            dto.NetworkSecurityGroup = resourceEntry;
        }

        async Task ScheduleCreationOfVirtualNetwork(SandboxResourceCreationAndSchedulingDto dto, ProvisioningQueueParentDto queueParentItem)
        {
            //TODO: Add special network rules to resource
            var networkName = AzureResourceNameUtil.VNet(dto.StudyName, dto.SandboxName);
            var sandboxSubnetName = AzureResourceNameUtil.SubNet(dto.StudyName, dto.SandboxName);

            var networkSettings = new NetworkSettingsDto() { SandboxSubnetName = sandboxSubnetName };
            var networkSettingsString = SandboxResourceConfigStringSerializer.Serialize(networkSettings);

            var nsgCreateOperation = dto.NetworkSecurityGroup.Operations.FirstOrDefault().Id.Value;

            var resourceEntry = await CreateResource(dto, queueParentItem, AzureResourceType.VirtualNetwork, sandboxControlled: true, resourceName: networkName, configString: networkSettingsString, dependsOn: nsgCreateOperation);
            dto.Network = resourceEntry;
        }

        async Task ScheduleCreationOfBastion(SandboxResourceCreationAndSchedulingDto dto, ProvisioningQueueParentDto queueParentItem, string configString = null)
        {
            var vNetCreateOperation = dto.Network.Operations.FirstOrDefault().Id.Value;

            var bastionName = AzureResourceNameUtil.Bastion(dto.StudyName, dto.SandboxName);

            var resourceEntry = await CreateResource(dto, queueParentItem, AzureResourceType.Bastion, sandboxControlled: true, resourceName: bastionName, configString: configString, dependsOn: vNetCreateOperation);
            dto.Bastion = resourceEntry;
        }

        async Task<SandboxResourceDto> CreateResource(SandboxResourceCreationAndSchedulingDto dto, ProvisioningQueueParentDto queueParentItem, string resourceType, bool sandboxControlled = true, string resourceName = AzureResourceNameUtil.AZURE_RESOURCE_INITIAL_ID_OR_NAME, string configString = null, int dependsOn = 0)
        {
            var resourceEntry = await _sandboxResourceService.Create(dto, resourceType, sandboxControlled: sandboxControlled, resourceName: resourceName, configString: configString, dependsOn: dependsOn);
            queueParentItem.Children.Add(new ProvisioningQueueChildDto() { SandboxResourceOperationId = resourceEntry.Operations.FirstOrDefault().Id.Value });

            return resourceEntry;
        }

        public async Task<List<SandboxResourceLightDto>> GetSandboxResources(int studyId, int sandboxId)
        {
            var sandboxFromDb = await GetSandboxOrThrowAsync(sandboxId, UserOperations.SandboxEdit);

            //Filter out deleted resources
            var resourcesFiltered = sandboxFromDb.Resources
                .Where(r => !r.Deleted.HasValue
                || (r.Deleted.HasValue && r.Operations.Where(o => o.OperationType == CloudResourceOperationType.DELETE && o.Status == CloudResourceOperationState.DONE_SUCCESSFUL).Any() == false)

                ).ToList();

            var resourcesMapped = _mapper.Map<List<SandboxResourceLightDto>>(resourcesFiltered);


            return resourcesMapped;
        }

        public async Task<SandboxDto> DeleteAsync(int studyId, int sandboxId)
        {
            _logger.LogWarning(SepesEventId.SandboxDelete, "Study {0}, Sandbox {1}: Starting", studyId, sandboxId);

            var studyFromDb = await StudyAccessUtil.GetStudyByIdCheckAccessOrThrow(_db, _userService, studyId, UserOperations.StudyAddRemoveSandbox);
            var sandboxFromDb = await _db.Sandboxes.Include(sb => sb.Resources).ThenInclude(r => r.Operations).FirstOrDefaultAsync(sb => sb.Id == sandboxId && (!sb.Deleted.HasValue || !sb.Deleted.Value));

            if (sandboxFromDb == null)
            {
                throw NotFoundException.CreateForEntity("Sandbox", sandboxId);
            }

            var user = _userService.GetCurrentUser();

            _logger.LogInformation(SepesEventId.SandboxDelete, "Study {0}, Sandbox {1}: Marking sandbox record for deletion", studyId, sandboxId);

            //Mark sandbox object as deleted
            sandboxFromDb.Deleted = true;
            sandboxFromDb.DeletedAt = DateTime.UtcNow;
            sandboxFromDb.DeletedBy = user.UserName;

            SandboxResource sandboxResourceGroup = null;

            if (sandboxFromDb.Resources.Count > 0)
            {
                //Mark all resources as deleted
                foreach (var curResource in sandboxFromDb.Resources)
                {
                    if (curResource.ResourceType == AzureResourceType.ResourceGroup)
                    {
                        sandboxResourceGroup = curResource;
                    }

                    curResource.Deleted = DateTime.UtcNow;
                    curResource.DeletedBy = user.UserName;

                    _logger.LogInformation(SepesEventId.SandboxDelete, "Study {0}, Sandbox {1}: Marking resource {2} for deletion", studyId, sandboxId, curResource.Id);
                }

                if (sandboxResourceGroup == null)
                {
                    throw new Exception($"Unable to find ResourceGroup record in DB for Sandbox {sandboxId}, StudyId: {studyId}.");
                }

                _logger.LogInformation(SepesEventId.SandboxDelete, $"Creating delete operation for resource group {sandboxResourceGroup.ResourceGroupName}");

                var deleteOperation = new SandboxResourceOperation()
                {
                    CreatedBy = user.UserName,
                    BatchId = Guid.NewGuid().ToString(),
                    CreatedBySessionId = _requestIdService.GetRequestId(),
                    OperationType = CloudResourceOperationType.DELETE,
                    SandboxResourceId = sandboxResourceGroup.Id,
                    Description = AzureResourceUtil.CreateDescriptionForResourceOperation(sandboxResourceGroup.ResourceType, CloudResourceOperationType.DELETE, sandboxResourceGroup.SandboxId) + ". (Delete of SandBox resource group and all resources within)",
                    MaxTryCount = CloudResourceConstants.RESOURCE_MAX_TRY_COUNT
                };

                sandboxResourceGroup.Operations.Add(deleteOperation);

                await _db.SaveChangesAsync();

                _logger.LogInformation(SepesEventId.SandboxDelete, "Study {0}, Sandbox {1}: Queuing operation", studyId, sandboxId);

                //Create queue item
                var queueParentItem = new ProvisioningQueueParentDto();
                queueParentItem.SandboxId = sandboxId;
                queueParentItem.Description = $"Delete resources for Sandbox: {sandboxId}";
                queueParentItem.Children.Add(new ProvisioningQueueChildDto() { SandboxResourceOperationId = deleteOperation.Id });
                await _provisioningQueueService.SendMessageAsync(queueParentItem, visibilityTimeout: TimeSpan.FromSeconds(10));
            }
            else
            {
                _logger.LogCritical(SepesEventId.SandboxDelete, "Study {0}, Sandbox {1}: Unable to find any resources for Sandbox", studyId, sandboxId);
                await _db.SaveChangesAsync();
            }

            _logger.LogInformation(SepesEventId.SandboxDelete, "Study {0}, Sandbox {1}: Done", studyId, sandboxId);

            return _mapper.Map<SandboxDto>(sandboxFromDb);
        }

        public async Task ReScheduleSandboxCreation(int sandboxId)
        {
            var sandboxFromDb = await GetSandboxOrThrowAsync(sandboxId, UserOperations.SandboxEdit);

            var queueParentItem = new ProvisioningQueueParentDto();
            queueParentItem.SandboxId = sandboxFromDb.Id;
            queueParentItem.Description = $"Create basic resources for Sandbox (re-scheduled): {sandboxFromDb.Id}";

            //Check state of sandbox resource creation: Resource group shold be success, rest should be not started or failed

            var resourceGroupResource = sandboxFromDb.Resources.SingleOrDefault(r => r.ResourceType == AzureResourceType.ResourceGroup);

            if (resourceGroupResource == null)
            {
                throw new NullReferenceException(ReScheduleLogPrefix(sandboxFromDb.StudyId, sandboxId, "Could not locate database entry for ResourceGroup"));
            }

            var resourceGroupResourceOperation = resourceGroupResource.Operations.OrderByDescending(o => o.Created).FirstOrDefault();

            if (resourceGroupResourceOperation == null)
            {
                throw new NullReferenceException(ReScheduleLogPrefix(sandboxFromDb.StudyId, sandboxId, "Could not locate ANY database entry for ResourceGroupOperation"));
            }
            else if (resourceGroupResourceOperation.OperationType != CloudResourceOperationType.CREATE && resourceGroupResourceOperation.Status == CloudResourceOperationState.DONE_SUCCESSFUL)
            {
                throw new Exception(ReScheduleLogPrefix(sandboxFromDb.StudyId, sandboxId, "Could not locate RELEVANT database entry for ResourceGroupOperation"));
            }

            //Rest of resources must have failed, cannot handle partial creation yet

            var operations = new List<SandboxResourceOperation>();

            foreach (var curResource in sandboxFromDb.Resources)
            {
                if (curResource.Id == resourceGroupResource.Id)
                {
                    //allready covered this above
                    continue;
                }

                //Last operation must be a create
                var relevantOperation = curResource.Operations.OrderByDescending(o => o.Created).FirstOrDefault();

                if (relevantOperation == null)
                {
                    throw new NullReferenceException(ReScheduleLogPrefix(sandboxFromDb.StudyId, sandboxId, "Could not locate ANY database entry for ResourceGroupOperation", curResource.Id));
                }
                else if (String.IsNullOrWhiteSpace(relevantOperation.Status) || relevantOperation.Status == CloudResourceOperationState.NEW || relevantOperation.Status == CloudResourceOperationState.IN_PROGRESS || relevantOperation.Status == CloudResourceOperationState.DONE_SUCCESSFUL)
                {
                    _logger.LogInformation(ReScheduleLogPrefix(sandboxFromDb.StudyId, sandboxId, $"Re-queing item. Previous status was {relevantOperation.Status}", curResource.Id));
                    queueParentItem.Children.Add(new ProvisioningQueueChildDto() { SandboxResourceOperationId = relevantOperation.Id });
                }
                else if (relevantOperation.Status == CloudResourceOperationState.FAILED)
                {
                    _logger.LogInformation(ReScheduleLogPrefix(sandboxFromDb.StudyId, sandboxId, $"Increasing retry count and re-queing item. Previous status was {relevantOperation.Status}", curResource.Id));
                    relevantOperation.MaxTryCount += 3;
                    await _db.SaveChangesAsync();
                    queueParentItem.Children.Add(new ProvisioningQueueChildDto() { SandboxResourceOperationId = relevantOperation.Id });
                }
                else
                {
                    throw new Exception(ReScheduleLogPrefix(sandboxFromDb.StudyId, sandboxId, $"Could not locate RELEVANT database entry for ResourceGroupOperation", curResource.Id));
                }
            }

            if (queueParentItem.Children.Count == 0)
            {
                throw new Exception(ReScheduleLogPrefix(sandboxFromDb.StudyId, sandboxId, $"Could not re-shedule creation. No relevant resource items found"));
            }
            else
            {
                await _provisioningQueueService.SendMessageAsync(queueParentItem);
            }
        }

        string ReScheduleLogPrefix(int studyId, int sandboxId, string logText, int resourceId = 0)
        {
            var logMessage = $"ReScheduleSandboxCreation | Study {studyId} | Sandbox {sandboxId}";

            if (resourceId > 0)
            {
                logMessage += $" | Resource: {resourceId}";
            }

            logMessage += $" | {logText}";

            return logMessage;
        }

        //public Task<IEnumerable<SandboxTemplateDto>> GetTemplatesAsync()
        //{
        //    return templates;          
        //}
    }
}
