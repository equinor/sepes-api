using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Constants.CloudResource;
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
    public class SandboxResourceOperationService : ISandboxResourceOperationService
    {
        readonly SepesDbContext _db;
        readonly IMapper _mapper;
        readonly IUserService _userService;
        readonly IRequestIdService _requestIdService;

        public SandboxResourceOperationService(SepesDbContext db, IMapper mapper, IUserService userService, IRequestIdService requestIdService)
        {
            _db = db;
            _mapper = mapper;
            _userService = userService;
            _requestIdService = requestIdService;
        }

        public async Task<SandboxResourceOperationDto> AddAsync(int sandboxResourceId, SandboxResourceOperationDto operationDto)
        {
            var sandboxResourceFromDb = await GetSandboxResourceOrThrowAsync(sandboxResourceId);
            var newOperation = _mapper.Map<SandboxResourceOperation>(operationDto);

            sandboxResourceFromDb.Operations.Add(newOperation);
            await _db.SaveChangesAsync();
            return await GetByIdAsync(newOperation.Id);
        }

        public async Task<SandboxResourceOperationDto> CreateUpdateOperationAsync(int sandboxResourceId, int dependsOn = 0, string batchId = null)
        {
            var sandboxResourceFromDb = await GetSandboxResourceOrThrowAsync(sandboxResourceId);

            var currentUser = await _userService.GetCurrentUserFromDbAsync();

            if (dependsOn == 0)
            {
                var previousOperation = sandboxResourceFromDb.Operations.OrderByDescending(o => o.Created).FirstOrDefault();

                if (previousOperation != null)
                {
                    if(previousOperation.OperationType == CloudResourceOperationType.DELETE)
                    {
                        throw new Exception($"Error when adding operation to resource {sandboxResourceId}, resource allready marked for deletion");
                    }
                    else if (previousOperation.OperationType == CloudResourceOperationType.UPDATE)
                    {
                        //If previous operation is waiting for some other op, it's best this also gets in line
                        if (previousOperation.DependsOnOperationId.HasValue)
                        {
                            dependsOn = previousOperation.Id;
                        }                    
                    }
                    else
                    {
                        dependsOn = previousOperation.Id;
                    }
                }
            }

            var newOperation = new SandboxResourceOperation()
            {
                Description = AzureResourceUtil.CreateDescriptionForResourceOperation(sandboxResourceFromDb.ResourceType, CloudResourceOperationType.UPDATE, sandboxResourceId),
                BatchId = batchId,
                OperationType = CloudResourceOperationType.UPDATE,
                CreatedBy = currentUser.UserName,            
                CreatedBySessionId = _requestIdService.GetRequestId(),
                DependsOnOperationId = dependsOn != 0 ? dependsOn : default(int?),
                MaxTryCount = CloudResourceConstants.RESOURCE_MAX_TRY_COUNT
            };


            sandboxResourceFromDb.Operations.Add(newOperation);
            await _db.SaveChangesAsync();
            return await GetByIdAsync(newOperation.Id);
        }

        public async Task<SandboxResourceOperationDto> GetByIdAsync(int id)
        {
            var itemFromDb = await GetOrThrowAsync(id);
            var itemDto = _mapper.Map<SandboxResourceOperationDto>(itemFromDb);
            return itemDto;
        }

        async Task<SandboxResourceOperation> GetOrThrowAsync(int id)
        {
            var entityFromDb = await _db.SandboxResourceOperations
                .Include(o => o.DependsOnOperation)
                .ThenInclude(o => o.Resource)
                .Include(o => o.Resource)
                 .ThenInclude(r => r.Sandbox)
                         .ThenInclude(sb => sb.Study)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (entityFromDb == null)
            {
                throw NotFoundException.CreateForEntity("SandboxResourceOperation", id);
            }

            return entityFromDb;
        }

        public async Task<SandboxResourceOperationDto> UpdateStatusAsync(int id, string status, string updatedProvisioningState = null, string errorMessage = null)
        {
            var currentUser = _userService.GetCurrentUser();

            var itemFromDb = await GetOrThrowAsync(id);
            itemFromDb.Status = status;
            itemFromDb.Updated = DateTime.UtcNow;
            itemFromDb.UpdatedBy = currentUser.UserName;

            if (updatedProvisioningState != null)
            {
                itemFromDb.Resource.LastKnownProvisioningState = updatedProvisioningState;
                itemFromDb.Resource.Updated = DateTime.UtcNow;
                itemFromDb.Resource.UpdatedBy = currentUser.UserName;

                if (updatedProvisioningState == CloudResourceOperationState.DONE_SUCCESSFUL)
                {
                    itemFromDb.LatestError = null;
                }
            }

            if (!String.IsNullOrWhiteSpace(errorMessage))
            {
                itemFromDb.LatestError = errorMessage;
            }

            await _db.SaveChangesAsync();

           return _mapper.Map<SandboxResourceOperationDto>(itemFromDb);
        }

        //public async Task<SandboxResourceOperationDto> UpdateStatusAndIncreaseTryCountAsync(int id, string status, string errorMessage = null)
        //{
        //    var currentUser = _userService.GetCurrentUser();

        //    var itemFromDb = await GetOrThrowAsync(id);
        //    itemFromDb.Status = status;
        //    itemFromDb.TryCount++;
        //    itemFromDb.Updated = DateTime.UtcNow;
        //    itemFromDb.UpdatedBy = currentUser.UserName;

        //    if (!String.IsNullOrWhiteSpace(errorMessage))
        //    {
        //        itemFromDb.LatestError = errorMessage;
        //    }

        //    await _db.SaveChangesAsync();

        //    return await GetByIdAsync(itemFromDb.Id);
        //}

        public async Task<SandboxResourceOperationDto> SetInProgressAsync(int id, string requestId, string status)
        {
            var currentUser = _userService.GetCurrentUser();

            var itemFromDb = await GetOrThrowAsync(id);
            itemFromDb.TryCount ++;
            itemFromDb.CarriedOutBySessionId = requestId;
            itemFromDb.Status = status;
            itemFromDb.Updated = DateTime.UtcNow;
            itemFromDb.UpdatedBy = currentUser.UserName;
            await _db.SaveChangesAsync();

            return _mapper.Map<SandboxResourceOperationDto>(itemFromDb);
        }

        private async Task<SandboxResource> GetSandboxResourceOrThrowAsync(int id)
        {
            var entityFromDb = await _db.SandboxResources
                .Include(sr => sr.Operations)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (entityFromDb == null)
            {
                throw NotFoundException.CreateForEntity("AzureResource", id);
            }

            return entityFromDb;
        }

        public async Task<bool> ExistsPreceedingUnfinishedOperationsAsync(SandboxResourceOperationDto operationDto)
        {
            var querable = GetPreceedingUnfinishedCreateOrUpdateOperationsQueryable(operationDto.Resource.Id, operationDto.BatchId, operationDto.Created);
            return await querable.AnyAsync();
        }

        public async Task<bool> OperationIsFinishedAndSucceededAsync(int operationId)
        {
            var itemFromDb = await GetOrThrowAsync(operationId);

            return itemFromDb.Status == CloudResourceOperationState.DONE_SUCCESSFUL;
        }

        public async Task<List<SandboxResourceOperation>> GetUnfinishedOperations(int resourceId)
        {
            var preceedingOpsQueryable = GetPreceedingUnfinishedCreateOrUpdateOperationsQueryable(resourceId);
            return await preceedingOpsQueryable.ToListAsync();
        }

        public async Task<bool> HasUnstartedCreateOrUpdateOperation(int resourceId)
        {
            var preceedingOpsQueryable = GetPreceedingUnstartedCreateOrUpdateOperationsQueryable(resourceId);
            return await preceedingOpsQueryable.AnyAsync();
        }

        public async Task<List<SandboxResourceOperation>> AbortAllUnfinishedCreateOrUpdateOperations(int resourceId)
        {
            var unfinishedOps = await GetUnfinishedOperations(resourceId);

            if (unfinishedOps != null && unfinishedOps.Count > 0)
            {
                var currentUser = _userService.GetCurrentUser();

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

        public async Task<SandboxResourceOperation> GetUnfinishedDeleteOperation(int resourceId)
        {
            return await _db.SandboxResourceOperations
               .Where(o => o.SandboxResourceId == resourceId
               && (String.IsNullOrWhiteSpace(o.Status) || o.Status == CloudResourceOperationState.NEW || o.Status == CloudResourceOperationState.IN_PROGRESS)
               ).FirstOrDefaultAsync();
        }

        IQueryable<SandboxResourceOperation> GetPreceedingUnfinishedCreateOrUpdateOperationsQueryable(int resourceId, string batchId = null, DateTime? createdEarlyerThan = null)
        {
            return _db.SandboxResourceOperations
                .Where(o => o.SandboxResourceId == resourceId
                && (batchId == null || (batchId != null && o.BatchId != batchId))
                && (createdEarlyerThan.HasValue == false || (createdEarlyerThan.HasValue && o.Created < createdEarlyerThan.Value))
                && (String.IsNullOrWhiteSpace(o.Status) || o.Status == CloudResourceOperationState.NEW || o.Status == CloudResourceOperationState.IN_PROGRESS)
                );
        }

        IQueryable<SandboxResourceOperation> GetPreceedingUnstartedCreateOrUpdateOperationsQueryable(int resourceId, string batchId = null, DateTime? createdEarlyerThan = null)
        {
            return _db.SandboxResourceOperations
                .Where(o => o.SandboxResourceId == resourceId
                && (batchId == null || (batchId != null && o.BatchId != batchId))
                && (createdEarlyerThan.HasValue == false || (createdEarlyerThan.HasValue && o.Created < createdEarlyerThan.Value))
                && (String.IsNullOrWhiteSpace(o.Status) || o.Status == CloudResourceOperationState.NEW)
                );
        }


    }
}
