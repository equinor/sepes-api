using AutoMapper;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class SandboxResourceService : ISandboxResourceService
    {
        readonly SepesDbContext _db;
        readonly IMapper _mapper;
        readonly IUserService _userService;

        public SandboxResourceService(SepesDbContext db, IMapper mapper, IUserService userService)
        {
            _db = db;
            _mapper = mapper;
            _userService = userService;
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

        public async Task<SandboxResourceDto> Update(int resourceId, IResourceGroup updated)
        {
            var resource = await GetOrThrowAsync(resourceId);
            resource.ResourceGroupId = updated.Id;
            resource.ResourceGroupName = updated.Name;
            resource.ResourceId = updated.Id;
            resource.ResourceKey = updated.Key;
            resource.ResourceName = updated.Name;
            resource.Status = updated.ProvisioningState;
            resource.Updated = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            var retVal = await GetByIdAsync(resourceId);
            return retVal;
        }

        public async Task<SandboxResourceDto> Update(int resourceId, IResource updated)
        {
            var resource = await GetOrThrowAsync(resourceId);
            resource.ResourceId = updated.Id;
            resource.ResourceKey = updated.Key;
            resource.ResourceName = updated.Name;
            resource.ResourceType = updated.Type;
            resource.Updated = DateTime.UtcNow;
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
            
            if(resource.LastKnownProvisioningState != newProvisioningState)
            {
                resource.LastKnownProvisioningState = newProvisioningState;
                resource.Updated = DateTime.UtcNow;
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
