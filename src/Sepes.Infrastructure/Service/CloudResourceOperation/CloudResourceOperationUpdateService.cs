using AutoMapper;
using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Interface;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class CloudResourceOperationUpdateService : CloudResourceOperationServiceBase, ICloudResourceOperationUpdateService
    {       
        readonly IUserService _userService;

        public CloudResourceOperationUpdateService(SepesDbContext db, IMapper mapper, IUserService userService)
            :base(db, mapper)
        {        
            _userService = userService;
        }  
        
        public async Task<CloudResourceOperationDto> UpdateStatusAsync(int id, string status, string updatedProvisioningState = null, string errorMessage = null)
        {
            var currentUser = await _userService.GetCurrentUserAsync();

            var operationFromDb = await GetResourceOperationOrThrowAsync(id);
            operationFromDb.Status = status;
            operationFromDb.Updated = DateTime.UtcNow;
            operationFromDb.UpdatedBy = currentUser.UserName;

            if (updatedProvisioningState != null)
            {
                operationFromDb.Resource.LastKnownProvisioningState = updatedProvisioningState;
                operationFromDb.Resource.Updated = DateTime.UtcNow;
                operationFromDb.Resource.UpdatedBy = currentUser.UserName;

                if (updatedProvisioningState == CloudResourceOperationState.DONE_SUCCESSFUL)
                {
                    operationFromDb.LatestError = null;
                }
            }

            if (!String.IsNullOrWhiteSpace(errorMessage))
            {
                operationFromDb.LatestError = errorMessage;
            }

            await _db.SaveChangesAsync();

            return _mapper.Map<CloudResourceOperationDto>(operationFromDb);
        }

        public async Task<CloudResourceOperationDto> SetInProgressAsync(int id, string requestId)
        {
            var currentUser = await _userService.GetCurrentUserAsync();

            var operationFromDb = await GetResourceOperationOrThrowAsync(id);
            operationFromDb.TryCount++;
            operationFromDb.CarriedOutBySessionId = requestId;
            operationFromDb.Status = CloudResourceOperationState.IN_PROGRESS;
            operationFromDb.Updated = DateTime.UtcNow;
            operationFromDb.UpdatedBy = currentUser.UserName;
            await _db.SaveChangesAsync();

            return _mapper.Map<CloudResourceOperationDto>(operationFromDb);
        }

        public async Task<CloudResourceOperationDto> ReInitiateAsync(int id)
        {
            var currentUser = await _userService.GetCurrentUserAsync();
            var operationFromDb = await GetResourceOperationOrThrowAsync(id);

            operationFromDb.Updated = DateTime.UtcNow;
            operationFromDb.UpdatedBy = currentUser.UserName;

            //Increase max try count, that makes this item ready for re-start
            operationFromDb.MaxTryCount += CloudResourceConstants.RESOURCE_MAX_TRY_COUNT;

            await _db.SaveChangesAsync();

            return _mapper.Map<CloudResourceOperationDto>(operationFromDb);
        }
      

        public async Task<List<CloudResourceOperation>> AbortAllUnfinishedCreateOrUpdateOperations(int resourceId)
        {
            var unfinishedOps = await GetUnfinishedOperations(resourceId);

            if (unfinishedOps != null && unfinishedOps.Count > 0)
            {
                var currentUser = await _userService.GetCurrentUserAsync();

                foreach (var curOps in unfinishedOps)
                {
                    curOps.Status = CloudResourceOperationState.ABORTED;
                    curOps.Updated = DateTime.UtcNow;
                    curOps.UpdatedBy = currentUser.UserName;
                }

                await _db.SaveChangesAsync();
            }

            return unfinishedOps;
        }  
    }
}
