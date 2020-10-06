using AutoMapper;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Azure;
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
    public class SandboxResourceService : ISandboxResourceService
    {
        readonly SepesDbContext _db;
        readonly ILogger<SandboxResourceService> _logger;
        readonly IMapper _mapper;
        readonly IUserService _userService;
        readonly IRequestIdService _requestIdService;
        readonly IAzureQueueService _azureQueueService;
        readonly IAzureResourceGroupService _resourceGroupService;
        readonly ISandboxResourceOperationService _sandboxResourceOperationService;

        public SandboxResourceService(SepesDbContext db, IMapper mapper, ILogger<SandboxResourceService> logger, IUserService userService, IRequestIdService requestIdService, IAzureQueueService azureQueueService, IAzureResourceGroupService resourceGroupService, ISandboxResourceOperationService sandboxResourceOperationService)
        {
            _db = db;
            _logger = logger;
            _mapper = mapper;
            _userService = userService;
            _requestIdService = requestIdService;
            _azureQueueService = azureQueueService;
            _resourceGroupService = resourceGroupService;
            _sandboxResourceOperationService = sandboxResourceOperationService ?? throw new ArgumentNullException(nameof(sandboxResourceOperationService));
        }

        public async Task CreateSandboxResourceGroup(SandboxWithCloudResourcesDto dto)
        {
            var sandboxResource = await AddInternal(dto.SandboxId, "not created", "not created", AzureResourceType.ResourceGroup, dto.Region.Name, dto.Tags);
           
            dto.ResourceGroup = MapEntityToDto(sandboxResource);

            await CreateResourceGroupForSandbox(dto);         

        }

        public async Task CreateResourceGroupForSandbox(SandboxWithCloudResourcesDto dto)
        {
            var resourceGroupName = AzureResourceNameUtil.ResourceGroup(dto.StudyName, dto.SandboxName);
         
            var azureResourceGroup = await _resourceGroupService.Create(resourceGroupName, dto.Region, dto.Tags);
            ApplyPropertiesFromResourceGroup(azureResourceGroup, dto.ResourceGroup);

            // After Resource is created, mark entry in SandboxResourceOperations-table as "created/successful" and update Id in resource-table.
            _ = await UpdateResourceGroup(dto.ResourceGroup.Id.Value, dto.ResourceGroup);
            _ = await _sandboxResourceOperationService.UpdateStatus(dto.ResourceGroup.Operations.FirstOrDefault().Id.Value, CloudResourceOperationState.DONE_SUCCESSFUL);
            _logger.LogInformation($"Resource group created for sandbox with Id: {dto.SandboxId}! Id: {dto.ResourceGroupId}, name: {dto.ResourceGroupName}");
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

        public async Task<SandboxResourceDto> Create(SandboxWithCloudResourcesDto dto, string type, string resourceName)
        {      
            var newResource = await AddInternal(dto.SandboxId, dto.ResourceGroupId, dto.ResourceGroupName, type, dto.Region.Name, dto.Tags, resourceName);
         
            return await GetByIdAsync(newResource.Id);
        }      

        async Task<SandboxResource> AddInternal(int sandboxId, string resourceGroupId, string resourceGroupName, string type, string region, Dictionary<string,string> tags, string resourceName = AzureResourceNameUtil.AZURE_RESOURCE_INITIAL_NAME)
        {
            var sandboxFromDb = await GetSandboxOrThrowAsync(sandboxId);

            var tagsString = AzureResourceTagsFactory.TagDictionaryToString(tags);

            var currentUser = await _userService.GetCurrentUserFromDbAsync();

            var newResource = new SandboxResource()
            {
                ResourceGroupId = resourceGroupId,
                ResourceGroupName = resourceGroupName,
                ResourceType = type,
                ResourceKey = "n/a",
                ResourceName = resourceName,
                ResourceId = "n/a",
                Region = region,
                Tags = tagsString,               
                Operations = new List<SandboxResourceOperation> {
                    new SandboxResourceOperation()
                    {
                    OperationType = CloudResourceOperationType.CREATE,
                    CreatedBySessionId = _requestIdService.GetRequestId()
                    }
                },
                CreatedBy = currentUser.UserName,
                Created = DateTime.UtcNow
            };

            sandboxFromDb.Resources.Add(newResource);

            await _db.SaveChangesAsync();

            return newResource;
        }

        public async Task<SandboxResourceDto> Add(int sandboxId, string resourceGroupId, string resourceGroupName, string type, string resourceId, string resourceName)
        {
            var sandboxFromDb = await GetSandboxOrThrowAsync(sandboxId);

            var newResource = new SandboxResource()
            {
                ResourceGroupId = resourceGroupId,
                ResourceGroupName = resourceGroupName,
                ResourceType = type,
                ResourceName = resourceName,
                Status = ""
            };

            sandboxFromDb.Resources.Add(newResource);
            await _db.SaveChangesAsync();

            return await GetByIdAsync(newResource.Id);
        }

        public async Task<SandboxResourceDto> AddResourceGroup(int sandboxId, string resourceGroupId, string resourceGroupName, string type) =>
            await Add(sandboxId, resourceGroupId, resourceGroupName, type, resourceGroupId, resourceGroupName);

        public async Task<SandboxResourceDto> Add(int sandboxId, string resourceGroupId, string resourceGroupName, Microsoft.Azure.Management.Network.Models.Resource resource) =>
            await Add(sandboxId, resourceGroupId, resourceGroupName, resource.Type, resource.Id, resource.Name);

        public async Task<SandboxResourceDto> Add(int sandboxId, string resourceGroupId, string resourceGroupName, IResource resource) =>
            await Add(sandboxId, resourceGroupId, resourceGroupName, resource.Type, resource.Id, resource.Name);

        //ResourceGroup
        //Nsg
        //VNet
        //Bastion

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

            var retVal = await GetByIdAsync(resourceId);
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
            resource.Updated = DateTime.UtcNow;
            resource.UpdatedBy = currentUser.UserName;
            await _db.SaveChangesAsync();

            var retVal = await GetByIdAsync(resourceId);
            return retVal;
        }

        public async Task<SandboxResourceDto> GetByIdAsync(int id)
        {
            var entityFromDb = await GetOrThrowAsync(id);

            var dto = MapEntityToDto(entityFromDb);

            return dto;
        }

        SandboxResourceDto MapEntityToDto(SandboxResource entity) => _mapper.Map<SandboxResourceDto>(entity);

        public async Task<SandboxResource> GetOrThrowAsync(int id)
        {
            var entityFromDb = await _db.SandboxResources.FirstOrDefaultAsync(s => s.Id == id);

            if (entityFromDb == null)
            {
                throw NotFoundException.CreateForEntity("AzureResource", id);
            }

            return entityFromDb;
        }

        public async Task<SandboxResourceDto> MarkAsDeletedByIdAsync(int id)
        {
            var resourceFromDb = await MarkAsDeletedByIdInternalAsync(id);
            return MapEntityToDto(resourceFromDb);
        }

        async Task<SandboxResource> MarkAsDeletedByIdInternalAsync(int id)
        {
            //WE DONT REALLY DELETE FROM THIS TABLE, WE "MARK AS DELETED" AND KEEP THE RECORDS FOR FUTURE REFERENCE

            var entityFromDb = await _db.SandboxResources.FirstOrDefaultAsync(s => s.Id == id);

            if (entityFromDb == null)
            {
                throw NotFoundException.CreateForEntity("AzureResource", id);
            }

            var user = _userService.GetCurrentUser();

            entityFromDb.DeletedBy = user.UserName;
            entityFromDb.Deleted = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return entityFromDb;
        }

        public async Task<List<SandboxResource>> GetActiveResources() => await _db.SandboxResources.Include(sr => sr.Sandbox)
                                                                                                   .ThenInclude(sb => sb.Study)
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

        public async Task<IEnumerable<SandboxResource>> GetDeletedResourcesAsync() => await _db.SandboxResources.Where(sr => sr.Deleted.HasValue)
                                                                                                                .ToListAsync();
    }
}
