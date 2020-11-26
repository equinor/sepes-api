using AutoMapper;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Azure;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Interface;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class SandboxResourceService : ISandboxResourceService
    {
        readonly SepesDbContext _db;
        readonly IConfiguration _config;
        readonly ILogger<SandboxResourceService> _logger;
        readonly IMapper _mapper;
        readonly IUserService _userService;
        readonly IRequestIdService _requestIdService;
        readonly IAzureResourceGroupService _resourceGroupService;
        readonly ISandboxResourceOperationService _sandboxResourceOperationService;
        readonly IProvisioningQueueService _provisioningQueueService;

        public SandboxResourceService(SepesDbContext db, IConfiguration config, IMapper mapper, ILogger<SandboxResourceService> logger, IUserService userService, IRequestIdService requestIdService, IAzureResourceGroupService resourceGroupService, ISandboxResourceOperationService sandboxResourceOperationService, IProvisioningQueueService provisioningQueueService)
        {
            _db = db;
            _config = config;
            _logger = logger;
            _mapper = mapper;
            _userService = userService;
            _requestIdService = requestIdService;
            _resourceGroupService = resourceGroupService;
            _sandboxResourceOperationService = sandboxResourceOperationService ?? throw new ArgumentNullException(nameof(sandboxResourceOperationService));
            _provisioningQueueService = provisioningQueueService;
        }

        public async Task CreateSandboxResourceGroup(SandboxResourceCreationAndSchedulingDto dto)
        {
            var resourceGroupName = AzureResourceNameUtil.ResourceGroup(dto.StudyName, dto.SandboxName);
            var resourceEntity = await AddInternal(dto.BatchId, dto.SandboxId, "not created", resourceGroupName, AzureResourceType.ResourceGroup, dto.Region.Name, resourceGroupName, dto.Tags);

            var resourceCreateOperation = resourceEntity.Operations.FirstOrDefault();
            await _sandboxResourceOperationService.SetInProgressAsync(resourceCreateOperation.Id, _requestIdService.GetRequestId(), CloudResourceOperationState.IN_PROGRESS);

            dto.ResourceGroup = MapEntityToDto(resourceEntity);

            var azureResourceGroup = await _resourceGroupService.Create(resourceEntity.ResourceName, dto.Region, dto.Tags);
            ApplyPropertiesFromResourceGroup(azureResourceGroup, dto.ResourceGroup);

            _ = await UpdateResourceGroup(dto.ResourceGroup.Id, dto.ResourceGroup);
            _ = await _sandboxResourceOperationService.UpdateStatusAsync(dto.ResourceGroup.Operations.FirstOrDefault().Id, CloudResourceOperationState.DONE_SUCCESSFUL);
        }

        public async Task<SandboxResourceDto> CreateVmEntryAsync(int sandboxId, SandboxResource resourceGroup, Microsoft.Azure.Management.ResourceManager.Fluent.Core.Region region, Dictionary<string, string> tags, string vmName, int dependsOn, string configString)
        {
            try
            {
                var resourceEntity = await AddInternal(Guid.NewGuid().ToString(),
                    sandboxId,
                    resourceGroup.ResourceGroupId, resourceGroup.ResourceGroupName, AzureResourceType.VirtualMachine, region.Name, vmName, tags, false, dependentOn: dependsOn, configString: configString);

                return MapEntityToDto(resourceEntity);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to create database resource entry for Virtual Machine for Sandbox {sandboxId}. See inner Exception for details", ex);
            }
        }

        public void ApplyPropertiesFromResourceGroup(AzureResourceGroupDto source, SandboxResourceDto target)
        {
            target.ResourceId = source.Id;
            target.ResourceName = source.Name;
            target.ResourceGroupId = source.Id;
            target.ResourceGroupName = source.Name;
            target.ProvisioningState = source.ProvisioningState;
            target.ResourceKey = source.Key;
        }

        public async Task<SandboxResourceDto> Create(SandboxResourceCreationAndSchedulingDto dto, string type, string resourceName, bool sandboxControlled = true, string configString = null, int dependsOn = 0)
        {
            var newResource = await AddInternal(dto.BatchId, dto.SandboxId, dto.ResourceGroupId, dto.ResourceGroupName, type, dto.Region.Name, resourceName, dto.Tags, sandboxControlled: sandboxControlled, dependentOn: dependsOn, configString: configString);

            var mappedToDto = MapEntityToDto(newResource);

            return mappedToDto;
        }

        public async Task ValidateNameThrowIfInvalid(string resourceName)
        {
            if(await _db.SandboxResources.Where(r=> r.ResourceName == resourceName && !r.Deleted.HasValue).AnyAsync())
            {
                throw new Exception($"Resource with name {resourceName} allready exists!");
            }
        }

        async Task<SandboxResource> AddInternal(string batchId, int sandboxId, string resourceGroupId, string resourceGroupName, string type, string region, string resourceName, Dictionary<string, string> tags, bool sandboxControlled = true, int dependentOn = 0, string configString = null)
        {
           await ValidateNameThrowIfInvalid(resourceName);

            var sandboxFromDb = await GetSandboxOrThrowAsync(sandboxId);

            var tagsString = AzureResourceTagsFactory.TagDictionaryToString(tags);

            var currentUser = await _userService.GetCurrentUserFromDbAsync();

            var newResource = new SandboxResource()
            {
                ResourceGroupId = resourceGroupId,
                ResourceGroupName = resourceGroupName,
                ResourceType = type,
                ResourceKey = AzureResourceNameUtil.AZURE_RESOURCE_INITIAL_ID_OR_NAME,
                ResourceName = resourceName,
                ResourceId = AzureResourceNameUtil.AZURE_RESOURCE_INITIAL_ID_OR_NAME,
                SandboxControlled = sandboxControlled,
                Region = region,
                Tags = tagsString,
                ConfigString = configString,

                Operations = new List<SandboxResourceOperation> {
                    new SandboxResourceOperation()
                    {
                    Description = AzureResourceUtil.CreateDescriptionForResourceOperation(type, CloudResourceOperationType.CREATE, sandboxId),
                    BatchId = batchId,
                    OperationType = CloudResourceOperationType.CREATE,
                    CreatedBy = currentUser.UserName,
                    CreatedBySessionId = _requestIdService.GetRequestId(),
                    DependsOnOperationId = dependentOn != 0 ? dependentOn: default(int?),
                    MaxTryCount = CloudResourceConstants.RESOURCE_MAX_TRY_COUNT
                    }
                },
                CreatedBy = currentUser.UserName,
                Created = DateTime.UtcNow
            };

            sandboxFromDb.Resources.Add(newResource);

            await _db.SaveChangesAsync();

            return newResource;
        }

        public async Task<SandboxResourceDto> UpdateResourceGroup(int resourceId, SandboxResourceDto updated)
        {
            var currentUser = await _userService.GetCurrentUserFromDbAsync();

            var resource = await GetOrThrowAsync(resourceId);
            resource.ResourceGroupId = updated.ResourceId;
            resource.ResourceGroupName = updated.ResourceName;
            resource.ResourceId = updated.ResourceId;
            resource.ResourceKey = updated.ResourceKey;
            resource.ResourceName = updated.ResourceName;
            resource.LastKnownProvisioningState = updated.ProvisioningState;
            resource.Updated = DateTime.UtcNow;
            resource.UpdatedBy = currentUser.UserName;
            await _db.SaveChangesAsync();

            var retVal = await GetDtoByIdAsync(resourceId);
            return retVal;
        }

        public async Task<SandboxResourceDto> Update(int resourceId, SandboxResourceDto updated)
        {
            var currentUser = await _userService.GetCurrentUserFromDbAsync();

            var resource = await GetOrThrowAsync(resourceId);
            resource.ResourceId = updated.ResourceId;
            resource.ResourceKey = updated.ResourceKey;
            resource.ResourceName = updated.ResourceName;
            resource.ResourceType = updated.ResourceType;
            resource.LastKnownProvisioningState = updated.ProvisioningState;
            resource.ConfigString = updated.ConfigString;        
            resource.Updated = DateTime.UtcNow;
            resource.UpdatedBy = currentUser.UserName;
            await _db.SaveChangesAsync();

            var retVal = await GetDtoByIdAsync(resourceId);
            return retVal;
        }

        public async Task<SandboxResource> GetByIdAsync(int id)
        {
            var entityFromDb = await GetOrThrowAsync(id);
            return entityFromDb;
        }

        public async Task<SandboxResourceDto> GetDtoByIdAsync(int id)
        {
            var entityFromDb = await GetByIdAsync(id);
            var dto = MapEntityToDto(entityFromDb);

            return dto;
        }

        SandboxResourceDto MapEntityToDto(SandboxResource entity) => _mapper.Map<SandboxResourceDto>(entity);

        public async Task<SandboxResource> GetOrThrowAsync(int id)
        {
            var entityFromDb = await _db.SandboxResources
                    .Include(r => r.Sandbox)
                    .ThenInclude(s=> s.Resources)
                    .Include(r => r.Operations)
                    .FirstOrDefaultAsync(s => s.Id == id);

            if (entityFromDb == null)
            {
                throw NotFoundException.CreateForEntity("AzureResource", id);
            }

            return entityFromDb;
        }

        public async Task<SandboxResourceDto> MarkAsDeletedAndScheduleDeletion(int id)
        {
            var user = _userService.GetCurrentUser();

            var resourceFromDb = await GetOrThrowAsync(id);

            var deleteOperationDescription = $"Delete resource {id} ({resourceFromDb.ResourceType})";

            _logger.LogInformation($"{deleteOperationDescription}: Abort all other operations for Resource");

            await _sandboxResourceOperationService.AbortAllUnfinishedCreateOrUpdateOperations(id);

            var deleteOperation = await _sandboxResourceOperationService.GetUnfinishedDeleteOperation(id);

            if (deleteOperation == null)
            {
                _logger.LogInformation($"{deleteOperationDescription}: Creating delete operation");

                deleteOperation = new SandboxResourceOperation()
                {
                    Description = AzureResourceUtil.CreateDescriptionForResourceOperation(resourceFromDb.ResourceType, CloudResourceOperationType.DELETE, resourceFromDb.SandboxId, id),
                    CreatedBy = user.UserName,
                    BatchId = Guid.NewGuid().ToString(),                   
                    CreatedBySessionId = _requestIdService.GetRequestId(),
                    OperationType = CloudResourceOperationType.DELETE,
                    SandboxResourceId = resourceFromDb.Id,                   
                    MaxTryCount = CloudResourceConstants.RESOURCE_MAX_TRY_COUNT
                };

                resourceFromDb.Operations.Add(deleteOperation);
            }
            else
            {
                _logger.LogInformation($"{deleteOperationDescription}: Existing delete operation found, re-queueing that");
                deleteOperation.TryCount = 0;
            }

            MarkAsDeletedInternal(resourceFromDb, user.UserName);

            await _db.SaveChangesAsync();

            _logger.LogInformation($"{deleteOperationDescription}: Enqueing delete operation");

            //Create queue item
            var queueParentItem = new ProvisioningQueueParentDto();
            queueParentItem.SandboxId = resourceFromDb.SandboxId;
            queueParentItem.Description = deleteOperationDescription;
            queueParentItem.Children.Add(new ProvisioningQueueChildDto() { SandboxResourceOperationId = deleteOperation.Id });
            await _provisioningQueueService.SendMessageAsync(queueParentItem);

            _logger.LogInformation($"{deleteOperationDescription}: Done!");

            return MapEntityToDto(resourceFromDb);
        }

        async Task<SandboxResource> MarkAsDeletedByIdInternalAsync(int id)
        {
            var resourceEntity = await _db.SandboxResources.FirstOrDefaultAsync(s => s.Id == id);

            if (resourceEntity == null)
            {
                throw NotFoundException.CreateForEntity("SandboxResource", id);
            }

            var user = _userService.GetCurrentUser();

            MarkAsDeletedInternal(resourceEntity, user.UserName);

            await _db.SaveChangesAsync();

            return resourceEntity;
        }

        SandboxResource MarkAsDeletedInternal(SandboxResource resource, string deletedBy)
        {
            resource.DeletedBy = deletedBy;
            resource.Deleted = DateTime.UtcNow;

            return resource;
        }

        public async Task<List<SandboxResource>> GetActiveResources() => await _db.SandboxResources.Include(sr => sr.Sandbox)
                                                                                                   .ThenInclude(sb => sb.Study)
                                                                                                    .Include(sr => sr.Operations)
                                                                                                   .Where(sr => !sr.Deleted.HasValue)
                                                                                                   .ToListAsync();

        public async Task UpdateProvisioningState(int resourceId, string newProvisioningState)
        {
            var resource = await GetOrThrowAsync(resourceId);

            if (resource.LastKnownProvisioningState != newProvisioningState)
            {
                var currentUser = await _userService.GetCurrentUserFromDbAsync();

                resource.LastKnownProvisioningState = newProvisioningState;
                resource.Updated = DateTime.UtcNow;
                resource.UpdatedBy = currentUser.UserName;
                await _db.SaveChangesAsync();
            }

        }

        public async Task<SandboxResourceDto> UpdateResourceIdAndName(int resourceId, string resourceIdInForeignSystem, string resourceNameInForeignSystem)
        {
            if (String.IsNullOrWhiteSpace(resourceIdInForeignSystem))
            {
                throw new ArgumentNullException("azureId", $"Provided empty foreign system resource id for resource {resourceId} ");
            }

            if (String.IsNullOrWhiteSpace(resourceNameInForeignSystem))
            {
                throw new ArgumentNullException("azureId", $"Provided empty foreign system resource name for resource {resourceId} ");
            }

            var resourceFromDb = await GetOrThrowAsync(resourceId);

            //if (String.IsNullOrWhiteSpace(resourceFromDb.ResourceId) == false && resourceFromDb.ResourceId != AzureResourceNameUtil.AZURE_RESOURCE_INITIAL_NAME)
            //{
            //    throw new Exception($"Resource {resourceId} allredy has a foreign system id. This should not have occured ");
            //}

            resourceFromDb.ResourceId = resourceIdInForeignSystem;

            if (resourceFromDb.ResourceName != resourceNameInForeignSystem)
            {
                resourceFromDb.ResourceName = resourceNameInForeignSystem;
            }

            var currentUser = _userService.GetCurrentUser();

            resourceFromDb.Updated = DateTime.UtcNow;
            resourceFromDb.UpdatedBy = currentUser.UserName;

            await _db.SaveChangesAsync();

            return MapEntityToDto(resourceFromDb);

        }
        private async Task<Sandbox> GetSandboxOrThrowAsync(int sandboxId)
        {
            var sandboxFromDb = await _db.Sandboxes
                .Include(sb => sb.Resources)
                    .ThenInclude(r => r.Operations)
                .FirstOrDefaultAsync(sb => sb.Id == sandboxId);

            if (sandboxFromDb == null)
            {
                throw NotFoundException.CreateForEntity("Sandbox", sandboxId);
            }
            return sandboxFromDb;
        }

        public async Task<IEnumerable<SandboxResource>> GetDeletedResourcesAsync() => await _db.SandboxResources.Include(sr => sr.Operations).Where(sr => sr.Deleted.HasValue && sr.Deleted.Value.AddMinutes(10) < DateTime.UtcNow)
                                                                                                                .ToListAsync();

        public async Task<bool> ResourceIsDeleted(int resourceId)
        {
            var resource = await _db.SandboxResources.AsNoTracking().FirstOrDefaultAsync(r => r.Id == resourceId);

            if(resource == null)
            {
                return true;
            }

            if (resource.Deleted.HasValue || !String.IsNullOrWhiteSpace(resource.DeletedBy) )
            {
                return true;
            } 
            
            return false;
        }
    }
}
