using AutoMapper;
using Sepes.Common.Constants.CloudResource;
using Sepes.Common.Dto;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Common.Util.Provisioning;
using System;
using System.Linq;
using System.Threading.Tasks;
using Sepes.Common.Interface;

namespace Sepes.Infrastructure.Service
{
    public class CloudResourceOperationCreateService : CloudResourceOperationServiceBase, ICloudResourceOperationCreateService
    {        
        readonly IRequestIdService _requestIdService;

        public CloudResourceOperationCreateService(SepesDbContext db, IMapper mapper, IUserService userService, IRequestIdService requestIdService)
            : base(db, mapper, userService)
        {          
            _requestIdService = requestIdService;
        }

        public async Task<CloudResourceOperationDto> AddAsync(int sandboxResourceId, CloudResourceOperationDto operationDto)
        {
            var sandboxResourceFromDb = await GetResourceOrThrowAsync(sandboxResourceId);
            var newOperation = _mapper.Map<CloudResourceOperation>(operationDto);

            sandboxResourceFromDb.Operations.Add(newOperation);
            await _db.SaveChangesAsync();
            return await GetOperationDtoInternal(newOperation.Id);
        }

        public async Task<CloudResourceOperationDto> CreateUpdateOperationAsync(int sandboxResourceId, string operationType = CloudResourceOperationType.UPDATE, int dependsOn = 0, string batchId = null, string desiredState = null)
        {
            var sandboxResourceFromDb = await GetResourceOrThrowAsync(sandboxResourceId);

            var currentUser = await _userService.GetCurrentUserAsync();

            if (dependsOn == 0)
            {
                var mustWaitFor = await CheckAnyIfOperationsToWaitFor(sandboxResourceFromDb, currentUser);

                if (mustWaitFor != null)
                {
                    dependsOn = mustWaitFor.Id;
                }
            }

            var newOperation = await CreateUpdateOperationAsync(
                ResourceOperationDescriptionUtils.CreateDescriptionForResourceOperation(sandboxResourceFromDb.ResourceType, operationType, sandboxResourceId),
               operationType,
               dependsOn,
               batchId,
               desiredState);

            sandboxResourceFromDb.Operations.Add(newOperation);
            await _db.SaveChangesAsync();
            return await GetOperationDtoInternal(newOperation.Id);
        }

        async Task<CloudResourceOperation> CreateUpdateOperationAsync(string description, string operationType, int dependsOn = 0, string batchId = null, string desiredState = null)
        {
            var updateOperation = await CreateBasicOperationAsync();

            updateOperation.Description = description;
            updateOperation.OperationType = operationType;
            updateOperation.BatchId = batchId;
            updateOperation.DependsOnOperationId = dependsOn != 0 ? dependsOn : default(int?);
            updateOperation.DesiredState = desiredState;           
            return updateOperation;
        }

        async Task<CloudResourceOperation> CreateBasicOperationAsync()
        {
            var currentUser = await _userService.GetCurrentUserAsync();

            var newOperation = new CloudResourceOperation()
            {
                Status = CloudResourceOperationState.NEW,
                CreatedBy = currentUser.UserName,
                CreatedBySessionId = _requestIdService.GetRequestId(),
                MaxTryCount = CloudResourceConstants.RESOURCE_MAX_TRY_COUNT
            };

            return newOperation;

        }

        async Task<CloudResourceOperation> CheckAnyIfOperationsToWaitFor(CloudResource resource, UserDto currentUser)
        {
            bool mostRecentOperation = true;

            foreach (var curOperation in resource.Operations.OrderByDescending(o => o.Created))
            {
                if (curOperation.OperationType == CloudResourceOperationType.DELETE)
                {
                    throw new Exception($"Error when adding operation to resource {resource.Id}, resource allready marked for deletion");
                }

                if (mostRecentOperation && curOperation.Status == CloudResourceOperationState.DONE_SUCCESSFUL)
                {
                    return null;
                }

                if (curOperation.OperationType == CloudResourceOperationType.UPDATE || curOperation.OperationType == CloudResourceOperationType.ENSURE_ROLES || curOperation.OperationType == CloudResourceOperationType.ENSURE_FIREWALL_RULES || curOperation.OperationType == CloudResourceOperationType.ENSURE_CORS_RULES)
                {
                    if (curOperation.Status != CloudResourceOperationState.DONE_SUCCESSFUL && curOperation.Status != CloudResourceOperationState.ABORTED && curOperation.Status != CloudResourceOperationState.ABANDONED)
                    {
                        //If very old, set to aborted and continue the search
                        if (curOperation.Updated.AddMinutes(2) < DateTime.UtcNow)
                        {
                            curOperation.Description += " (appeared to be failing)";
                            curOperation.Status = CloudResourceOperationState.ABANDONED;
                            curOperation.Updated = DateTime.UtcNow;
                            curOperation.UpdatedBy = currentUser.UserName;
                            await _db.SaveChangesAsync();
                        }
                        else
                        {
                            return curOperation;
                        }
                    }
                }

                if (curOperation.OperationType == CloudResourceOperationType.CREATE)
                {
                    if (curOperation.Status != CloudResourceOperationState.DONE_SUCCESSFUL && curOperation.Status != CloudResourceOperationState.ABORTED && curOperation.Status != CloudResourceOperationState.ABANDONED)
                    {
                        return curOperation;
                    }
                }

                mostRecentOperation = false;
            }

            return null;
        }

        public async Task<CloudResourceOperation> CreateDeleteOperationAsync(int sandboxResourceId, string description, string batchId = null)
        {
            var user = await _userService.GetCurrentUserAsync();

            var resourceFromDb = await GetResourceOrThrowAsync(sandboxResourceId);

            var deleteOperation = new CloudResourceOperation()
            {
                CreatedBy = user.UserName,
                BatchId = batchId,
                CloudResourceId = sandboxResourceId,
                OperationType = CloudResourceOperationType.DELETE,
                Status = CloudResourceOperationState.NEW,
                CreatedBySessionId = _requestIdService.GetRequestId(),
                MaxTryCount = CloudResourceConstants.RESOURCE_MAX_TRY_COUNT,
                Description = description
            };

            resourceFromDb.Operations.Add(deleteOperation);

            await _db.SaveChangesAsync();

            return deleteOperation;
        }
    }
}
