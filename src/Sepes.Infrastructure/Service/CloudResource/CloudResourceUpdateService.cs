using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Interface;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class CloudResourceUpdateService : CloudResourceServiceBase, ICloudResourceUpdateService
    {       

        public CloudResourceUpdateService(SepesDbContext db, IConfiguration config, IMapper mapper, ILogger<CloudResourceUpdateService> logger, IUserService userService)
         : base(db, config, mapper, logger, userService)
        {           
           
        }     

        public async Task<SandboxResourceDto> UpdateResourceGroup(int resourceId, SandboxResourceDto updated)
        {
            var currentUser = await _userService.GetCurrentUserAsync();

            var resource = await GetOrThrowInternalAsync(resourceId);
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
            var currentUser = await _userService.GetCurrentUserAsync();

            var resource = await GetOrThrowInternalAsync(resourceId);
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
        
        public async Task UpdateProvisioningState(int resourceId, string newProvisioningState)
        {
            var resource = await GetOrThrowInternalAsync(resourceId);

            if (resource.LastKnownProvisioningState != newProvisioningState)
            {
                var currentUser = await _userService.GetCurrentUserAsync();

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

            var resourceFromDb = await GetOrThrowInternalAsync(resourceId);          

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
