using AutoMapper;
using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class CloudResourceOperationUpdateService : CloudResourceOperationServiceBase, ICloudResourceOperationUpdateService
    {
        public CloudResourceOperationUpdateService(SepesDbContext db, IMapper mapper, IUserService userService)
            :base(db, mapper, userService)
        {        
           
        }

        public async Task<CloudResourceOperationDto> SetInProgressAsync(int id, string requestId)
        {
            var operationFromDb = await GetExistingOperationReadyForUpdate(id);
            operationFromDb.TryCount++;
            operationFromDb.CarriedOutBySessionId = requestId;
            operationFromDb.Status = CloudResourceOperationState.IN_PROGRESS;
            await _db.SaveChangesAsync();

            return _mapper.Map<CloudResourceOperationDto>(operationFromDb);
        }

        public async Task<CloudResourceOperationDto> UpdateStatusAsync(int id, string status, string updatedProvisioningState = null, string errorMessage = null)
        {
            var operationFromDb = await GetExistingOperationReadyForUpdate(id);

            operationFromDb.Status = status;         

            if (updatedProvisioningState != null)
            {
                operationFromDb.Resource.LastKnownProvisioningState = updatedProvisioningState;
                operationFromDb.Resource.Updated = DateTime.UtcNow;
                operationFromDb.Resource.UpdatedBy = operationFromDb.UpdatedBy;

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

        public async Task<CloudResourceOperationDto> SetErrorMessageAsync(int id, Exception exception)
        {
            var messageBuilder = new StringBuilder(exception.Message);

            Exception curException = exception;

            while (curException.InnerException != null)
            {
                messageBuilder.AppendLine(curException.InnerException.Message);
                curException = curException.InnerException;
            } 
            
            return await SetErrorMessageAsync(id, messageBuilder.ToString());
        }

        public async Task<CloudResourceOperationDto> SetErrorMessageAsync(int id, string errorMessage)
        {
            var operationFromDb = await GetExistingOperationReadyForUpdate(id);           

            operationFromDb.LatestError = errorMessage;

            await _db.SaveChangesAsync();

            return _mapper.Map<CloudResourceOperationDto>(operationFromDb);
        }

        public async Task<CloudResourceOperationDto> SetDesiredStateAsync(int id, string desiredState)
        {
            var operationFromDb = await GetExistingOperationReadyForUpdate(id);
            operationFromDb.DesiredState = desiredState;
            await _db.SaveChangesAsync();
            return _mapper.Map<CloudResourceOperationDto>(operationFromDb);
        }    

        public async Task<CloudResourceOperationDto> TouchAsync(int id)
        {
            var operationFromDb = await GetExistingOperationReadyForUpdate(id);          
           
            await _db.SaveChangesAsync();

            return _mapper.Map<CloudResourceOperationDto>(operationFromDb);
        }

        public async Task<CloudResourceOperationDto> SetQueueInformationAsync(int id, string messageId, string popReceipt, DateTimeOffset nextVisibleOn)
        {           
            var operationFromDb = await GetExistingOperationReadyForUpdate(id);

            operationFromDb.QueueMessageId = messageId;
            operationFromDb.QueueMessagePopReceipt = popReceipt;
            operationFromDb.QueueMessageVisibleAgainAt = nextVisibleOn.UtcDateTime;        

            await _db.SaveChangesAsync();

            return _mapper.Map<CloudResourceOperationDto>(operationFromDb);
        }

        public async Task<CloudResourceOperationDto> ClearQueueInformationAsync(int id)
        {           
            var operationFromDb = await GetExistingOperationReadyForUpdate(id);

            operationFromDb.QueueMessageId = null;
            operationFromDb.QueueMessagePopReceipt = null;
            operationFromDb.QueueMessageVisibleAgainAt = null;        

            await _db.SaveChangesAsync();

            return _mapper.Map<CloudResourceOperationDto>(operationFromDb);
        }

        public async Task<CloudResourceOperationDto> ReInitiateAsync(int id)
        {           
            var operationFromDb = await GetExistingOperationReadyForUpdate(id);         

            //Increase max try count, that makes this item ready for re-start
            operationFromDb.MaxTryCount += CloudResourceConstants.RESOURCE_MAX_TRY_COUNT;

            await _db.SaveChangesAsync();

            return _mapper.Map<CloudResourceOperationDto>(operationFromDb);
        }

        public async Task<CloudResourceOperationDto> AbortAndAllowDependentOperationsToRun(int id, string errorMessage = null)
        {
            var operationFromDb = await GetExistingOperationReadyForUpdate(id);
            operationFromDb.Status = CloudResourceOperationState.ABANDONED;

            if(operationFromDb.DependantOnThisOperation != null)
            {
                foreach (var curDependent in operationFromDb.DependantOnThisOperation)
                {
                    curDependent.DependsOnOperationId = null;
                    curDependent.Updated = operationFromDb.Updated;
                    curDependent.UpdatedBy = operationFromDb.UpdatedBy;
                }
            }          

            await _db.SaveChangesAsync();

            return _mapper.Map<CloudResourceOperationDto>(operationFromDb);
        }


        public async Task<List<CloudResourceOperation>> AbortAllUnfinishedCreateOrUpdateOperationsAsync(int resourceId)
        {
            var unfinishedOps = await GetUnfinishedOperations(resourceId);

            if (unfinishedOps != null && unfinishedOps.Count > 0)
            {
                var currentUser = await _userService.GetCurrentUserAsync();

                foreach (var curOps in unfinishedOps)
                {
                    curOps.Status = CloudResourceOperationState.ABANDONED;
                    curOps.Updated = DateTime.UtcNow;
                    curOps.UpdatedBy = currentUser.UserName;
                }

                await _db.SaveChangesAsync();
            }

            return unfinishedOps;
        }

       
    }
}
