using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sepes.Infrastructure.Constants.CloudResource;
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
    public class CloudResourceOperationServiceBase
    {
        protected readonly SepesDbContext _db;
        protected readonly IMapper _mapper;
        protected readonly IUserService _userService;

        public CloudResourceOperationServiceBase(SepesDbContext db, IMapper mapper, IUserService userService)
        {
            _db = db;
            _mapper = mapper;
            _userService = userService;
        }

        public async Task<List<CloudResourceOperation>> GetUnfinishedOperations(int resourceId)
        {
            var preceedingOpsQueryable = GetPreceedingUnfinishedCreateOrUpdateOperationsQueryable(resourceId);
            return await preceedingOpsQueryable.ToListAsync();
        }

        public async Task<bool> HasUnstartedCreateOrUpdateOperation(int resourceId)
        {
            var preceedingOpsQueryable = GetPreceedingUnstartedCreateOrUpdateOperationsQueryable(resourceId);
            return await preceedingOpsQueryable.AnyAsync();
        }

        public async Task<bool> ExistsPreceedingUnfinishedOperationsAsync(CloudResourceOperationDto operationDto)
        {
            var querable = GetPreceedingUnfinishedCreateOrUpdateOperationsQueryable(operationDto.Resource.Id, operationDto.BatchId, operationDto.Created);
            return await querable.AnyAsync();
        }

        protected async Task<CloudResourceOperationDto> GetOperationDtoInternal(int id)
        {
            var itemFromDb = await GetResourceOperationOrThrowAsync(id);
            var itemDto = _mapper.Map<CloudResourceOperationDto>(itemFromDb);
            return itemDto;
        }

        protected async Task<CloudResource> GetResourceOrThrowAsync(int id)
        {
            var entityFromDb = await _db.CloudResources
                .Include(sr => sr.Operations)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (entityFromDb == null)
            {
                throw NotFoundException.CreateForEntity("AzureResource", id);
            }

            return entityFromDb;
        }

        protected async Task<CloudResourceOperation> GetResourceOperationOrThrowAsync(int id)
        {
            var entityFromDb = await _db.CloudResourceOperations
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

        protected async Task<CloudResourceOperation> GetExistingOperationReadyForUpdate(int id)
        {
            var currentUser = await _userService.GetCurrentUserAsync();

            var operationFromDb = await GetResourceOperationOrThrowAsync(id);
            operationFromDb.Updated = DateTime.UtcNow;
            operationFromDb.UpdatedBy = currentUser.UserName;

            return operationFromDb;
        }

        IQueryable<CloudResourceOperation> GetPreceedingUnfinishedCreateOrUpdateOperationsQueryable(int resourceId, string batchId = null, DateTime? createdEarlyerThan = null)
        {
            return _db.CloudResourceOperations
                .Where(o => o.CloudResourceId == resourceId
                && (batchId == null || (batchId != null && o.BatchId != batchId))
                && (!createdEarlyerThan.HasValue || (createdEarlyerThan.HasValue && o.Created < createdEarlyerThan.Value))
                && (o.OperationType == CloudResourceOperationType.CREATE || o.OperationType == CloudResourceOperationType.UPDATE)
                && (String.IsNullOrWhiteSpace(o.Status) || o.Status == CloudResourceOperationState.NEW || o.Status == CloudResourceOperationState.IN_PROGRESS)
                );
        }

        IQueryable<CloudResourceOperation> GetPreceedingUnstartedCreateOrUpdateOperationsQueryable(int resourceId, string batchId = null, DateTime? createdEarlyerThan = null)
        {
            return _db.CloudResourceOperations
                .Where(o => o.CloudResourceId == resourceId
                && (batchId == null || (batchId != null && o.BatchId != batchId))
                && (!createdEarlyerThan.HasValue || (createdEarlyerThan.HasValue && o.Created < createdEarlyerThan.Value))
                && (String.IsNullOrWhiteSpace(o.Status) || o.Status == CloudResourceOperationState.NEW)
                );
        }
    }
}
