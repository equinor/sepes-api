using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service.DataModelService
{
    public class CloudResourceUpdateService : CloudResourceServiceBase, ICloudResourceUpdateService
    {       

        public CloudResourceUpdateService(SepesDbContext db, IConfiguration config, IMapper mapper, ILogger<CloudResourceUpdateService> logger, IUserService userService, ISandboxModelService sandboxModelService)
         : base(db, config, mapper, logger, userService, sandboxModelService)
        {           
           
        }     

        public async Task<CloudResourceDto> UpdateResourceGroup(int resourceId, CloudResourceDto updated)
        {
            var currentUser = await _userService.GetCurrentUserAsync();

            var resource = await GetInternalWithoutAccessCheckAsync(resourceId);           
            resource.ResourceGroupName = updated.ResourceName;
            resource.ResourceId = updated.ResourceId;
            resource.ResourceKey = updated.ResourceKey;
            resource.ResourceName = updated.ResourceName;
            resource.LastKnownProvisioningState = updated.ProvisioningState;
            resource.ParentResourceId = updated.ParentResourceId;
            resource.Updated = DateTime.UtcNow;
            resource.UpdatedBy = currentUser.UserName;
            await _db.SaveChangesAsync();
            
            return MapEntityToDto(resource);
        }

        public async Task<CloudResource> Update(int resourceId, CloudResource updated)
        {
            var currentUser = await _userService.GetCurrentUserAsync();

            var resource = await GetInternalWithoutAccessCheckAsync(resourceId);
            resource.ResourceId = updated.ResourceId;
            resource.ResourceKey = updated.ResourceKey;
            resource.ResourceName = updated.ResourceName;
            resource.ResourceType = updated.ResourceType;
            resource.LastKnownProvisioningState = updated.LastKnownProvisioningState;
            resource.ParentResourceId = updated.ParentResourceId;
            resource.ConfigString = updated.ConfigString;        
            resource.Updated = DateTime.UtcNow;
            resource.UpdatedBy = currentUser.UserName;
            await _db.SaveChangesAsync();
            
            return resource;
        }   
        
        public async Task UpdateProvisioningState(int resourceId, string newProvisioningState)
        {
            var resource = await GetInternalWithoutAccessCheckAsync(resourceId);

            if (resource.LastKnownProvisioningState != newProvisioningState)
            {
                var currentUser = await _userService.GetCurrentUserAsync();

                resource.LastKnownProvisioningState = newProvisioningState;
                resource.Updated = DateTime.UtcNow;
                resource.UpdatedBy = currentUser.UserName;
                await _db.SaveChangesAsync();
            }
        }

        public async Task<CloudResourceDto> UpdateResourceIdAndName(int resourceId, string resourceIdInForeignSystem, string resourceNameInForeignSystem)
        {
            if (String.IsNullOrWhiteSpace(resourceIdInForeignSystem))
            {
                throw new ArgumentNullException("resourceIdInForeignSystem", $"Provided empty foreign system resource id for resource {resourceId} ");
            }

            if (String.IsNullOrWhiteSpace(resourceNameInForeignSystem))
            {
                throw new ArgumentNullException("resourceNameInForeignSystem", $"Provided empty foreign system resource name for resource {resourceId} ");
            }

            var resourceFromDb = await GetInternalWithoutAccessCheckAsync(resourceId);          

            resourceFromDb.ResourceId = resourceIdInForeignSystem;

            if (resourceFromDb.ResourceName != resourceNameInForeignSystem)
            {
                resourceFromDb.ResourceName = resourceNameInForeignSystem;
            }           

            var currentUser = await _userService.GetCurrentUserAsync();

            resourceFromDb.Updated = DateTime.UtcNow;
            resourceFromDb.UpdatedBy = currentUser.UserName;

            await _db.SaveChangesAsync();

            return MapEntityToDto(resourceFromDb);

        }          
    }
}
